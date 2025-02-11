using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _Context;

        public TicketRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Ticket value)
        {
            value.DeletedAt = DateTime.UtcNow;

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Ticket, bool>> condition)
        {
            return await _Context.Tickets.AnyAsync(condition);
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Tickets;
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Ticket?> GetByAsync(Expression<Func<Ticket, bool>> condition)
        {
            return await _Context.Tickets.FirstOrDefaultAsync(condition);
        }

        public async Task<Ticket?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Tickets.Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            return Math.Max(await _Context.Tickets.CountAsync() / size, 1);
        }

        public async Task<IEnumerable<Ticket>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Tickets.Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Ticket value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Ticket value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}