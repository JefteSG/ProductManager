using ProductManager.Application.DTOs;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Mappings;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Exceptions;
using ProductManager.Domain.Rules;

namespace ProductManager.Application.Services;

/// <summary>
/// Orquestra todos os casos de uso de produtos e aplica as regras de negócio.
/// Este é o único local autoritativo para a lógica de domínio — os controladores
/// permanecem leves e lidam apenas com preocupações HTTP.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, ct);

        return new PagedResponse<ProductResponse>
        {
            Items = items.Select(p => p.ToResponse()),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        return product.ToResponse();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        // Regra de negócio: SKU deve ser único.
        await EnsureSkuIsUniqueAsync(request.Sku, excludeId: null, ct);

        // Regra de negócio: Preço mínimo para eletrônicos.
        ValidateElectronicsPrice(request.Category, request.Price);

        var product = Product.Create(
            request.Name,
            request.Sku,
            request.Category,
            request.Description,
            request.Price,
            request.StockQuantity);

        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Product created. Id={ProductId}, SKU={Sku}, Category={Category}",
            product.Id, product.Sku, product.Category);

        return product.ToResponse();
    }

    public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        // Regra de negócio: SKU deve ser único (exclui o produto atual da verificação de unicidade).
        await EnsureSkuIsUniqueAsync(request.Sku, excludeId: id, ct);

        // Regra de negócio: Preço mínimo para eletrônicos.
        ValidateElectronicsPrice(request.Category, request.Price);

        product.Update(
            request.Name,
            request.Sku,
            request.Category,
            request.Description,
            request.Price,
            request.StockQuantity);

        _repository.Update(product);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Product updated. Id={ProductId}, SKU={Sku}, Category={Category}",
            product.Id, product.Sku, product.Category);

        return product.ToResponse();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        _repository.Delete(product);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation("Product deleted. Id={ProductId}, SKU={Sku}", product.Id, product.Sku);
    }

    // ── Private helpers ─────────────────────────────────────────────────────────

    private async Task EnsureSkuIsUniqueAsync(string sku, int? excludeId, CancellationToken ct = default)
    {
        var existing = await _repository.GetBySkuAsync(sku, ct);
        if (existing is not null && existing.Id != excludeId)
            throw new DuplicateSkuException(sku);
    }

    private static void ValidateElectronicsPrice(string category, decimal price)
    {
        if (category == ProductCategories.Electronics && price < ProductCategories.MinElectronicsPrice)
            throw new BusinessRuleViolationException(
                $"Products in the '{ProductCategories.Electronics}' category must have a price of at least R$ {ProductCategories.MinElectronicsPrice:F2}.");
    }
}
