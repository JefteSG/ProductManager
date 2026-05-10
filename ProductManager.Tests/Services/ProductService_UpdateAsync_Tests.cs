using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManager.Application.DTOs;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Services;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Exceptions;
using ProductManager.Domain.Rules;

namespace ProductManager.Tests.Services;

public class ProductService_UpdateAsync_Tests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly ProductService _sut;

    public ProductService_UpdateAsync_Tests()
    {
        _sut = new ProductService(_repoMock.Object, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public async Task UpdateAsync_HappyPath_ReturnsUpdatedProduct()
    {
        // Arrange
        var existing = new Product { Id = 1, Name = "Old", Sku = "SKU-001", Category = "Outros", Price = 10m, StockQuantity = 5 };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-001", It.IsAny<CancellationToken>())).ReturnsAsync(existing); // same product — allowed
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new UpdateProductRequest("New Name", "SKU-001", "Outros", null, 200m, 20);

        // Act
        var result = await _sut.UpdateAsync(1, request);

        // Assert
        result.Name.Should().Be("New Name");
        result.Price.Should().Be(200m);
        result.StockQuantity.Should().Be(20);
        _repoMock.Verify(r => r.Update(existing), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ProductNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var request = new UpdateProductRequest("Name", "SKU-001", "Outros", null, 100m, 5);

        // Act
        var act = () => _sut.UpdateAsync(99, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task UpdateAsync_SkuTakenByAnotherProduct_ThrowsDuplicateSkuException()
    {
        // Arrange
        var current = new Product { Id = 1, Sku = "SKU-OLD" };
        var other = new Product { Id = 2, Sku = "SKU-TAKEN" };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(current);
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-TAKEN", It.IsAny<CancellationToken>())).ReturnsAsync(other); // different id

        var request = new UpdateProductRequest("Name", "SKU-TAKEN", "Outros", null, 100m, 5);

        // Act
        var act = () => _sut.UpdateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<DuplicateSkuException>()
            .WithMessage("*SKU-TAKEN*");
        _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_KeepingOwnSku_Succeeds()
    {
        // Arrange — the product keeps its own SKU; uniqueness check should allow it
        var existing = new Product { Id = 1, Sku = "SKU-MINE", Category = "Outros", Price = 10m, StockQuantity = 1 };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-MINE", It.IsAny<CancellationToken>())).ReturnsAsync(existing); // same id → allowed
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new UpdateProductRequest("Updated", "SKU-MINE", "Outros", null, 50m, 1);

        // Act
        var act = () => _sut.UpdateAsync(1, request);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_ElectronicsBelowMinPrice_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var existing = new Product { Id = 1, Sku = "SKU-001", Category = "Outros", Price = 10m };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-001", It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = new UpdateProductRequest(
            "Phone", "SKU-001", ProductCategories.Electronics, null,
            Price: ProductCategories.MinElectronicsPrice - 1m, StockQuantity: 1);

        // Act
        var act = () => _sut.UpdateAsync(1, request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>();
    }
}
