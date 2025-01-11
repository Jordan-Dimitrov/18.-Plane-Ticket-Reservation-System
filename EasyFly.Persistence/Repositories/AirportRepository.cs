using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _Context;

        public AirportRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Airport value)
        {
            _Context.Remove(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Airport, bool>> condition)
        {
            return await _Context.Airports.AnyAsync(condition);
        }

        public async Task<IEnumerable<Airport>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Airports;
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Airport?> GetByAsync(Expression<Func<Airport, bool>> condition)
        {
            return await _Context.Airports.FirstOrDefaultAsync(condition);
        }

        public async Task<Airport?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Airports.Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            return Math.Max(await _Context.Airports.CountAsync() / size, 1);
        }

        public async Task<IEnumerable<Airport>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Airports.Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Airport value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Airport value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}