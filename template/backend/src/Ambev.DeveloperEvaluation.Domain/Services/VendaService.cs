using Ambev.DeveloperEvaluation.Domain.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.WebApi.Services
{
    public class VendaService
    {
        private readonly IVendaRepository _repo;
        private readonly ILogger<VendaService> _logger;

        public VendaService(IVendaRepository repo, ILogger<VendaService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // Regras de desconto:
        // 4-9 unidades => 10%
        // 10-20 unidades => 20%
        // >20 => inválido
        private decimal CalcularPercentualDesconto(int quantidade)
        {
            if (quantidade >= 10 && quantidade <= 20) return 0.20m;
            if (quantidade >= 4 && quantidade < 10) return 0.10m;
            return 0m;
        }

        private void ValidarItem(VendaItem item)
        {
            if (item.Quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero");
            if (item.Quantidade > 20) throw new ArgumentException("Não é possível vender mais de 20 itens idênticos");
            // Regras de desconto aplicadas em calcular percentual
        }

        public Venda CriarVenda(VendaCreateRequest req)
        {
            var venda = new Venda
            {
                Data = DateTime.UtcNow,
                ClienteId = req.ClienteId,
                ClienteNome = req.ClienteNome,
                FilialId = req.FilialId,
                FilialNome = req.FilialNome,
                Itens = req.Itens.Select(i =>
                {
                    var item = new VendaItem
                    {
                        ProdutoId = i.ProdutoId,
                        ProdutoNome = i.ProdutoNome,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    };
                    item.PercentualDesconto = CalcularPercentualDesconto(item.Quantidade);
                    ValidarItem(item);
                    return item;
                }).ToList()
            };

            var created = _repo.Add(venda);

            _logger.LogInformation("Evento: VendaCriada - Id: {Id}, Numero: {Numero}, ValorTotal: {ValorTotal}", created.Id, created.Numero, created.ValorTotal);

            return created;
        }

        public IEnumerable<Venda> Listar() => _repo.GetAll();

        public Venda? Obter(Guid id) => _repo.Get(id);

        public Venda Atualizar(Guid id, VendaUpdateRequest req)
        {
            var existing = _repo.Get(id) ?? throw new KeyNotFoundException("Venda não encontrada");

            if (existing.IsCancelada) throw new InvalidOperationException("Não é possível modificar uma venda cancelada");

            // Atualizar campos básicos
            if (req.Data.HasValue) existing.Data = req.Data.Value;
            existing.ClienteId = req.ClienteId;
            existing.ClienteNome = req.ClienteNome;
            existing.FilialId = req.FilialId;
            existing.FilialNome = req.FilialNome;

            // Mapear itens: se Id fornecido, atualizar; se não, criar novo
            var novosItens = new List<VendaItem>();
            foreach (var itemReq in req.Itens)
            {
                VendaItem item;
                if (itemReq.Id.HasValue)
                {
                    item = existing.Itens.FirstOrDefault(i => i.Id == itemReq.Id.Value) ?? new VendaItem { Id = itemReq.Id.Value };
                }
                else
                {
                    item = new VendaItem();
                }

                item.ProdutoId = itemReq.ProdutoId;
                item.ProdutoNome = itemReq.ProdutoNome;
                item.Quantidade = itemReq.Quantidade;
                item.PrecoUnitario = itemReq.PrecoUnitario;

                item.PercentualDesconto = CalcularPercentualDesconto(item.Quantidade);
                ValidarItem(item);

                // Se item estava cancelado e agora volta com quantidade >0, reativa-lo
                if (item.IsCancelado && !itemReq.Id.HasValue)
                    item.IsCancelado = false;

                novosItens.Add(item);
            }

            existing.Itens = novosItens;

            var updated = _repo.Update(existing);

            _logger.LogInformation("Evento: VendaModificada - Id: {Id}, Numero: {Numero}, ValorTotal: {ValorTotal}", updated.Id, updated.Numero, updated.ValorTotal);

            return updated;
        }

        public void CancelarVenda(Guid id)
        {
            var existing = _repo.Get(id) ?? throw new KeyNotFoundException("Venda não encontrada");
            if (existing.IsCancelada) return;
            existing.IsCancelada = true;
            _repo.Update(existing);
            _logger.LogInformation("Evento: VendaCancelada - Id: {Id}, Numero: {Numero}", existing.Id, existing.Numero);
        }

        public void CancelarItem(Guid vendaId, Guid itemId)
        {
            var existing = _repo.Get(vendaId) ?? throw new KeyNotFoundException("Venda não encontrada");
            var item = existing.Itens.FirstOrDefault(i => i.Id == itemId) ?? throw new KeyNotFoundException("Item não encontrado");
            if (item.IsCancelado) return;
            item.IsCancelado = true;
            _repo.Update(existing);
            _logger.LogInformation("Evento: ItemCancelado - VendaId: {VendaId}, ItemId: {ItemId}, ProdutoId: {ProdutoId}", vendaId, itemId, item.ProdutoId);
        }

        public VendaResponse ToResponse(Venda venda)
        {
            return new VendaResponse(
                venda.Id,
                venda.Numero,
                venda.Data,
                venda.ClienteId,
                venda.ClienteNome,
                venda.FilialId,
                venda.FilialNome,
                decimal.Round(venda.ValorTotal, 2),
                venda.IsCancelada,
                venda.Itens.Select(i => new VendaItemResponse(
                    i.Id,
                    i.ProdutoId,
                    i.ProdutoNome,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.PercentualDesconto,
                    i.ValorDesconto,
                    i.ValorTotal,
                    i.IsCancelado
                )).ToList()
            );
        }
    }
}