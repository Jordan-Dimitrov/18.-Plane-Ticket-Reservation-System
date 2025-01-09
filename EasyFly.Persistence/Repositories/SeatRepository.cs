using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class SeatRepository : ISeatRepository
    {
        public Task<bool> DeleteAsync(Seat value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Seat, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Seat>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<Seat?> GetByAsync(Expression<Func<Seat, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<Seat?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Seat>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Seat value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Seat value)
        {
            throw new NotImplementedException();
        }
    }
}
