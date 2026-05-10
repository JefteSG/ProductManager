using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Data;

/// <summary>
/// Single EF Core DbContext para a aplicação.
/// Mantém todas as configurações de entidade em um único lugar usando a Fluent API
/// para que a entidade de domínio permaneça livre de anotações de infraestrutura.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(100);

            // Índice único aplicado no nível do banco de dados como segunda linha de defesa
            // (a camada de serviço verifica primeiro, mas o índice previne condições de corrida).
            entity.HasIndex(p => p.Sku).IsUnique();

            entity.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Description)
                .HasMaxLength(1000);

            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(p => p.StockQuantity)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.Property(p => p.UpdatedAt)
                .IsRequired();
        });
    }
}
