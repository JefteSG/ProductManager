namespace ProductManager.Application.DTOs;

/// <summary>
/// Envelope genérico paginado retornado por endpoints de listagem.
/// TotalPages é calculado para que o cliente nunca precise calculá-lo.
/// </summary>
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
