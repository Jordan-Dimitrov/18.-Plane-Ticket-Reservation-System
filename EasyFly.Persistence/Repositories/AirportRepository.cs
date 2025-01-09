using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class AirportRepository : IAirportRepository
    {
        public Task<bool> DeleteAsync(Airport value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Airport, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Airport>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<Airport?> GetByAsync(Expression<Func<Airport, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<Airport?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Airport>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Airport value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Airport value)
        {
            throw new NotImplementedException();
        }
    }
}
