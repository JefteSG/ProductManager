namespace ProductManager.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Factory method para criar um novo produto. Garante que as propriedades de auditoria
    /// sejam sempre definidas corretamente e que o objeto nunca esteja em um estado parcialmente construído.
    /// </summary>
    public static Product Create(
        string name, string sku, string category, string? description,
        decimal price, int stockQuantity)
    {
        var now = DateTime.UtcNow;
        return new Product
        {
            Name = name,
            Sku = sku,
            Category = category,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    /// <summary>
    /// Aplica uma atualização a todos os campos mutáveis e atualiza <see cref="UpdatedAt"/>.
    /// Mantém a lógica de mutação dentro da entidade em vez de espalhada pelo serviço.
    /// </summary>
    public void Update(
        string name, string sku, string category, string? description,
        decimal price, int stockQuantity)
    {
        Name = name;
        Sku = sku;
        Category = category;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
