namespace Ambev.DeveloperEvaluation.WebApi.Models;

public class Venda
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long Numero { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid FilialId { get; set; }
    public string FilialNome { get; set; } = string.Empty;
    public List<VendaItem> Itens { get; set; } = new();
    public decimal ValorTotal => Itens.Where(i => !i.IsCancelado).Sum(i => i.ValorTotal);
    public bool IsCancelada { get; set; } = false;
}