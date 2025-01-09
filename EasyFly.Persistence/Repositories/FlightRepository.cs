using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class FlightRepository : IFlightRepository
    {
        public Task<bool> DeleteAsync(Flight value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Flight, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Flight>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<Flight?> GetByAsync(Expression<Func<Flight, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<Flight?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Flight>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Flight value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Flight value)
        {
            throw new NotImplementedException();
        }
    }
}
