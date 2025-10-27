using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface IVendaRepository
    {
        Venda Add(Venda venda);
        Venda? Get(Guid id);
        IEnumerable<Venda> GetAll();
        Venda Update(Venda venda);
        void Remove(Guid id);
        long GetNextNumber();
    }
}