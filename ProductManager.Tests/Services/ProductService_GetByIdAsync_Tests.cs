using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Services;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Exceptions;

namespace ProductManager.Tests.Services;

public class ProductService_GetByIdAsync_Tests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly ProductService _sut;

    public ProductService_GetByIdAsync_Tests()
    {
        _sut = new ProductService(_repoMock.Object, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProductResponse()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Widget",
            Sku = "SKU-001",
            Category = "Outros",
            Price = 99m,
            StockQuantity = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Id.Should().Be(1);
        result.Name.Should().Be("Widget");
        result.Sku.Should().Be("SKU-001");
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act
        var act = () => _sut.GetByIdAsync(42);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*42*");
    }
}
