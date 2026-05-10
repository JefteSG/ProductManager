namespace ProductManager.Domain.Exceptions;

/// <summary>
/// Lançada quando um produto viola uma regra de negócio (por exemplo, preço de Eletrônicos < 50).
/// Mapeia para HTTP 422 Unprocessable Entity no middleware de exceção.
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message) { }
}
