using ProductManager.Domain.Entities;

namespace ProductManager.Application.Interfaces;

/// <summary>
/// Contrato de acesso a dados para produtos.
/// A camada de serviço depende desta interface, nunca do EF Core diretamente,
/// mantendo a lógica de negócio desacoplada da tecnologia de persistência.
/// </summary>
public interface IProductRepository
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    void Update(Product product);
    void Delete(Product product);
    Task SaveChangesAsync(CancellationToken ct);
}
