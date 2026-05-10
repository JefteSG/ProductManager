using Microsoft.EntityFrameworkCore;
using ProductManager.Application.Interfaces;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Data;

namespace ProductManager.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de produtos usando EF Core.
/// Todas as operações de banco de dados vivem aqui para que a lógica de negócios
/// na camada de serviço permaneça limpa de quaisquer preocupações de persistência.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Products.AsNoTracking().OrderBy(p => p.Id);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Products.FindAsync([id], ct);

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
        => await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Sku == sku, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
        => await _context.Products.AddAsync(product, ct);

    public void Update(Product product)
        => _context.Products.Update(product);

    public void Delete(Product product)
        => _context.Products.Remove(product);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
