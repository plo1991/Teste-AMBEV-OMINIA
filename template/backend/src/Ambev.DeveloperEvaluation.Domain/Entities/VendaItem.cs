namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class VendaItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal PercentualDesconto { get; set; } = 0m;
    public decimal ValorDesconto => Math.Round(PrecoUnitario * Quantidade * PercentualDesconto, 2);
    public decimal ValorTotal => Math.Round(PrecoUnitario * Quantidade - ValorDesconto, 2);
    public bool IsCancelado { get; set; } = false;
}