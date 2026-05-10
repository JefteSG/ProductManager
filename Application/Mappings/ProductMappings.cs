using ProductManager.Application.DTOs;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Mappings;

/// <summary>
/// Mapeamento manual entre entidades de domínio e DTOs.
/// Mantém o AutoMapper fora da árvore de dependências — para o tamanho deste projeto,
/// mapeamentos explícitos são mais legíveis e fáceis de depurar.
/// </summary>
public static class ProductMappings
{
    public static ProductResponse ToResponse(this Product product)
        => new(
            product.Id,
            product.Name,
            product.Sku,
            product.Category,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt
        );
}
