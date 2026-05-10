namespace ProductManager.Domain.Exceptions;

/// <summary>
/// Classe base para todas as violações de regras de negócio em nível de domínio.
/// Capturar esse tipo no middleware de exceção permite um tratamento uniforme
/// de todos os erros de domínio sem vazar detalhes de infraestrutura.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
