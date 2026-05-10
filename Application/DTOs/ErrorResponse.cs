namespace ProductManager.Application.DTOs;

/// <summary>
/// Envelope de erro padronizado para que todos os erros da API tenham uma forma consistente.
/// </summary>
public record ErrorResponse(string Message, int StatusCode);
