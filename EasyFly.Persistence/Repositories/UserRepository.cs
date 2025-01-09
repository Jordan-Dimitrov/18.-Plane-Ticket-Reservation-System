using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public Task<bool> DeleteAsync(User value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Expression<Func<User, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync(bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByAsync(Expression<Func<User, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id, bool trackChanges)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPageCount(int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertAsync(User value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User value)
        {
            throw new NotImplementedException();
        }
    }
}
