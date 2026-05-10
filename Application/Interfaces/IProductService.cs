using ProductManager.Application.DTOs;

namespace ProductManager.Application.Interfaces;

/// <summary>
/// Contrato de serviço para casos de uso de produtos.
/// Os controladores dependem desta interface, nunca do serviço concreto,
/// o que torna os controladores trivialmente testáveis via mocks.
/// </summary>
public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<ProductResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct);
    Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
