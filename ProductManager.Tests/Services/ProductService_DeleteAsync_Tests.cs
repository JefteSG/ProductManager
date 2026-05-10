using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Services;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Exceptions;

namespace ProductManager.Tests.Services;

public class ProductService_DeleteAsync_Tests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly ProductService _sut;

    public ProductService_DeleteAsync_Tests()
    {
        _sut = new ProductService(_repoMock.Object, NullLogger<ProductService>.Instance);
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_DeletesAndSaves()
    {
        // Arrange
        var product = new Product { Id = 1, Sku = "SKU-001" };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        _repoMock.Verify(r => r.Delete(product), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        // Act
        var act = () => _sut.DeleteAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
        _repoMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
