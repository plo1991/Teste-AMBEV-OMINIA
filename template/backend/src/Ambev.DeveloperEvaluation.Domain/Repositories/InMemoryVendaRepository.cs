using Ambev.DeveloperEvaluation.Domain.Entities;
using System.Collections.Concurrent;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public class InMemoryVendaRepository : IVendaRepository
    {
        private readonly ConcurrentDictionary<Guid, Venda> _store = new();
        private long _sequence = 0;

        public Venda Add(Venda venda)
        {
            venda.Numero = GetNextNumber();
            _store[venda.Id] = venda;
            return venda;
        }

        public Venda? Get(Guid id) => _store.TryGetValue(id, out var v) ? v : null;

        public IEnumerable<Venda> GetAll() => _store.Values.OrderBy(v => v.Numero);

        public Venda Update(Venda venda)
        {
            _store[venda.Id] = venda;
            return venda;
        }

        public void Remove(Guid id) => _store.TryRemove(id, out _);

        public long GetNextNumber() => System.Threading.Interlocked.Increment(ref _sequence);
    }
}