using FluentValidation;
using ProductManager.Domain.Rules;

namespace ProductManager.Application.Validators;

/// <summary>
/// Regras de validação compartilhadas para solicitações de criação e atualização de produtos.
/// Elimina duplicação entre <see cref="CreateProductRequestValidator"/>
/// e <see cref="UpdateProductRequestValidator"/>.
/// </summary>
public abstract class ProductRequestValidatorBase<T> : AbstractValidator<T>
    where T : IProductRequest
{
    protected ProductRequestValidatorBase()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU must not exceed 100 characters.")
            .Matches(@"^[A-Za-z0-9\-_]+$").WithMessage("SKU may only contain letters, digits, hyphens and underscores.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .Must(c => ProductCategories.All.Contains(c))
            .WithMessage($"Category is invalid. Valid categories: {string.Join(", ", ProductCategories.All)}.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        // Regra de negócio: Preço mínimo para eletrônicos.
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(ProductCategories.MinElectronicsPrice)
            .When(x => x.Category == ProductCategories.Electronics)
            .WithMessage($"Products in the '{ProductCategories.Electronics}' category must have a price of at least R$ {ProductCategories.MinElectronicsPrice:F2}.");

        // Regra de negocio: Quantidade em estoque não pode ser negativa.
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}
