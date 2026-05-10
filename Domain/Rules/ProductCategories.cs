namespace ProductManager.Domain.Rules;

/// <summary>
/// Local central para constantes de categorias.
/// Evita strings mágicas espalhadas pelo código e facilita
/// futuras adições de categorias em um único arquivo.
/// </summary>
public static class ProductCategories
{
    public const string Electronics = "Eletronicos";
    public const decimal MinElectronicsPrice = 50m;

    public static readonly IReadOnlyList<string> All =
    [
        "Eletronicos",
        "Roupas",
        "Alimentos",
        "Livros",
        "Esportes",
        "Outros"
    ];
}
