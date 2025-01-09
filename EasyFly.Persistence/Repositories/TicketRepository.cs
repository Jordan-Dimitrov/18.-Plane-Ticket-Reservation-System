using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class TicketRepository : ITicketRepository
    {
        public Task<bool> DeleteAsync(Ticket value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Ticket, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket?> GetByAsync(Expression<Func<Ticket, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ticket>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Ticket value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Ticket value)
        {
            throw new NotImplementedException();
        }
    }
}
