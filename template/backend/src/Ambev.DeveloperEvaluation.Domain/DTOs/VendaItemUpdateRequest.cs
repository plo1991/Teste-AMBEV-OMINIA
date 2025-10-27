namespace Ambev.DeveloperEvaluation.Domain.DTOs;

public record VendaItemUpdateRequest(Guid? Id, Guid ProdutoId, string ProdutoNome, int Quantidade, decimal PrecoUnitario);
public record VendaUpdateRequest(
    DateTime? Data,
    Guid ClienteId,
    string ClienteNome,
    Guid FilialId,
    string FilialNome,
    List<VendaItemUpdateRequest> Itens
);