namespace ProductManager.Domain.Exceptions;

/// <summary>
/// Lançada quando um produto com o mesmo SKU já existe.
/// Mapeia para HTTP 409 Conflict no middleware de exceção.
/// </summary>
public class DuplicateSkuException : DomainException
{
    public DuplicateSkuException(string sku)
        : base($"A product with SKU '{sku}' already exists.") { }
}
