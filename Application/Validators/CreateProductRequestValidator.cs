using ProductManager.Application.DTOs;

namespace ProductManager.Application.Validators;

/// <summary>
/// Validador FluentValidation para criação de produtos.
/// Regras que envolvem múltiplos campos (Eletrônicos + preço) vivem na classe base em vez
/// de em atributos porque DataAnnotations não consegue expressar condições que abrangem
/// múltiplas propriedades.
/// </summary>
public class CreateProductRequestValidator : ProductRequestValidatorBase<CreateProductRequest>
{
    public CreateProductRequestValidator() : base() { }
}
