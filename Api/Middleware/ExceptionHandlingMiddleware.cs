using System.Net;
using System.Text.Json;
using FluentValidation;
using ProductManager.Application.DTOs;
using ProductManager.Domain.Exceptions;

namespace ProductManager.Api.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções.
/// Converte exceções de domínio tipadas e falhas de validação em respostas de erro JSON padronizadas,
/// para que nenhuma exceção não tratada chegue ao cliente como uma página HTML ou um corpo 500 vazio.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            // Falhas de validação do FluentValidation — coletadas de todas as regras em uma única resposta.
            var messages = ex.Errors.Select(e => e.ErrorMessage);
            _logger.LogWarning("Validation failed: {Errors}", string.Join("; ", messages));
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, string.Join(" | ", messages));
        }
        catch (DuplicateSkuException ex)
        {
            _logger.LogWarning("Duplicate SKU attempt: {Message}", ex.Message);
            await WriteErrorAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogWarning("Business rule violation: {Message}", ex.Message);
            await WriteErrorAsync(context, HttpStatusCode.UnprocessableEntity, ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Resource not found: {Message}", ex.Message);
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(
            new ErrorResponse(message, (int)statusCode),
            JsonOptions);

        await context.Response.WriteAsync(body);
    }
}
