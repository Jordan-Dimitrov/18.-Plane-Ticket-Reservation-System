using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class PlaneRepository : IPlaneRepository
    {
        private readonly ApplicationDbContext _Context;

        public PlaneRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Plane value)
        {
            value.DeletedAt = DateTime.UtcNow;

            foreach (var item in value.Seats)
            {
                item.DeletedAt = DateTime.UtcNow;
            }

            foreach (var item in value.Flights)
            {
                item.DeletedAt = DateTime.UtcNow;
            }

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Plane, bool>> condition)
        {
            return await _Context.Planes.AnyAsync(condition);
        }

        public async Task<IEnumerable<Plane>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Planes.Include(x => x.Flights).Include(x => x.Seats);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Plane?> GetByAsync(Expression<Func<Plane, bool>> condition)
        {
            return await _Context.Planes.Include(x => x.Flights).Include(x => x.Seats).FirstOrDefaultAsync(condition);
        }

        public async Task<Plane?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Planes.Include(x => x.Flights).Include(x => x.Seats)
                .Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            return Math.Max(await _Context.Planes.CountAsync() / size, 1);
        }

        public async Task<IEnumerable<Plane>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Planes.Include(x => x.Flights).Include(x => x.Seats)
                .Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Plane value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Plane value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}