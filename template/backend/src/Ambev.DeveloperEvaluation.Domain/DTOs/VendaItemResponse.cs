namespace Ambev.DeveloperEvaluation.Domain.DTOs;

public record VendaItemResponse(
    Guid Id,
    Guid ProdutoId,
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal PercentualDesconto,
    decimal ValorDesconto,
    decimal ValorTotal,
    bool IsCancelado
);

public record VendaResponse(
    Guid Id,
    long Numero,
    DateTime Data,
    Guid ClienteId,
    string ClienteNome,
    Guid FilialId,
    string FilialNome,
    decimal ValorTotal,
    bool IsCancelada,
    List<VendaItemResponse> Itens
);