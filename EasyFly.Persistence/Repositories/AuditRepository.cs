using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _Context;

        public AuditRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Audit value)
        {
            value.DeletedAt = DateTime.UtcNow;

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Audit, bool>> condition)
        {
            return await _Context.Audits.AnyAsync(condition);
        }

        public async Task<IEnumerable<Audit>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Audits;
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<ICollection<Audit>?> GetAllByAsync(Expression<Func<Audit, bool>> condition)
        {
            return await _Context.Audits.Where(condition).ToListAsync();
        }

        public async Task<Audit?> GetByAsync(Expression<Func<Audit, bool>> condition)
        {
            return await _Context.Audits.FirstOrDefaultAsync(condition);
        }

        public async Task<Audit?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Audits.Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            return Math.Max(await _Context.Audits.CountAsync() / size, 1);
        }

        public async Task<IEnumerable<Audit>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Audits.Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Audit value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Audit value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}