namespace ProductManager.Domain.Exceptions;

/// <summary>
/// Lançada quando um recurso (como um produto) não é encontrado pelo ID.
/// Mapeia para HTTP 404 Not Found no middleware de exceção.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string resource, object key)
        : base($"{resource} with id '{key}' was not found.") { }
}
