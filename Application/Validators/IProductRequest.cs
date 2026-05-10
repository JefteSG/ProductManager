namespace ProductManager.Application.Validators;

/// <summary>
/// Interface marcador que expõe as propriedades compartilhadas por solicitações de criação e atualização de produtos.
/// Permite que <see cref="ProductRequestValidatorBase{T}"/> defina regras uma vez para ambos os DTOs.
/// </summary>
public interface IProductRequest
{
    string Name { get; }
    string Sku { get; }
    string Category { get; }
    decimal Price { get; }
    int StockQuantity { get; }
}
