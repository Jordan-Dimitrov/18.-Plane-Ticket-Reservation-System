using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class PlaneRepository : IPlaneRepository
    {
        public Task<bool> DeleteAsync(Plane value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<Plane, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plane>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<Plane?> GetByAsync(Expression<Func<Plane, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<Plane?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plane>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(Plane value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Plane value)
        {
            throw new NotImplementedException();
        }
    }
}
