using ProductManager.Application.Validators;

namespace ProductManager.Application.DTOs;

public record CreateProductRequest(
    string Name,
    string Sku,
    string Category,
    string? Description,
    decimal Price,
    int StockQuantity
) : IProductRequest;
