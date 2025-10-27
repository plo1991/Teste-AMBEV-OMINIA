namespace Ambev.DeveloperEvaluation.Domain.DTOs;

public record VendaItemRequest(Guid ProdutoId, string ProdutoNome, int Quantidade, decimal PrecoUnitario);
public record VendaCreateRequest(
    Guid ClienteId,
    string ClienteNome,
    Guid FilialId,
    string FilialNome,
    List<VendaItemRequest> Itens
);