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

public class ProductService_CreateAsync_Tests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly ProductService _sut;

    public ProductService_CreateAsync_Tests()
    {
        _sut = new ProductService(_repoMock.Object, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_HappyPath_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new CreateProductRequest("Widget", "SKU-001", "Outros", null, 100m, 10);
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-001", It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Name.Should().Be("Widget");
        result.Sku.Should().Be("SKU-001");
        result.Category.Should().Be("Outros");
        result.Price.Should().Be(100m);
        result.StockQuantity.Should().Be(10);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateSku_ThrowsDuplicateSkuException()
    {
        // Arrange
        var existing = new Product { Id = 1, Sku = "SKU-001" };
        _repoMock.Setup(r => r.GetBySkuAsync("SKU-001", It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = new CreateProductRequest("Widget", "SKU-001", "Outros", null, 100m, 10);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateSkuException>()
            .WithMessage("*SKU-001*");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ElectronicsBelowMinPrice_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var request = new CreateProductRequest(
            "Cheap Phone", "SKU-002", ProductCategories.Electronics, null,
            Price: ProductCategories.MinElectronicsPrice - 1m, StockQuantity: 5);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleViolationException>()
            .WithMessage($"*{ProductCategories.Electronics}*");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ElectronicsAtMinPrice_Succeeds()
    {
        // Arrange
        _repoMock.Setup(r => r.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new CreateProductRequest(
            "Phone", "SKU-003", ProductCategories.Electronics, null,
            Price: ProductCategories.MinElectronicsPrice, StockQuantity: 5);

        // Act
        var act = () => _sut.CreateAsync(request);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
