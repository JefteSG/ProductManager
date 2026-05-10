namespace ProductManager.Application.DTOs;

public record ProductResponse(
    int Id,
    string Name,
    string Sku,
    string Category,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
