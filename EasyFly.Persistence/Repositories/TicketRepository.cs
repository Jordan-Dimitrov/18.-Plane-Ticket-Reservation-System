using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyFly.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _Context;

        public TicketRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<int> Count()
        {
            return await _Context.Tickets.Where(x => x.Reserved == true).CountAsync();
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
            var query = _Context.Tickets.Include(x => x.Seat).ThenInclude(s => s.Plane)
                .Include(x => x.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(x => x.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(x => x.Flight).ThenInclude(f => f.Plane)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Ticket?> GetByAsync(Expression<Func<Ticket, bool>> condition)
        {
            return await _Context.Tickets.Include(x => x.Seat).ThenInclude(s => s.Plane)
                .Include(x => x.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(x => x.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(x => x.Flight).ThenInclude(f => f.Plane)
                .Include(x => x.User)
                .FirstOrDefaultAsync(condition);
        }

        public async Task<Ticket?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Tickets.Include(x => x.Seat).ThenInclude(s => s.Plane)
                .Include(x => x.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(x => x.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(x => x.Flight).ThenInclude(f => f.Plane)
                .Include(x => x.User)
                .Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCountWithFilter(
         int size,
         string? search,
         string? typeFilter,
         string? luggageFilter)
        {
            var total = await BaseFilterQuery(true, search, typeFilter, luggageFilter).CountAsync();
            return (int)Math.Ceiling((double)total / size);
        }

        public async Task<int> GetPageCount(int size)
        {
            var count = (double)await _Context.Tickets.CountAsync() / size;

            return (int)Math.Ceiling(count);
        }

        public async Task<IEnumerable<Ticket>> GetPagedWithFilterAsync(bool trackChanges, int page, int size, string? search, string? typeFilter, string? luggageFilter)
        {
            return await BaseFilterQuery(trackChanges, search, typeFilter, luggageFilter)
                            .Skip((page - 1) * size)
                            .Take(size)
                            .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Tickets.Include(x => x.Seat).ThenInclude(s => s.Plane)
                .Include(x => x.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(x => x.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(x => x.Flight).ThenInclude(f => f.Plane)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }


        public async Task<IEnumerable<Ticket>> GetPagedByFlightIdAsync(Guid flightId, bool trackChanges, int page, int size,
            string? search,
            string? typeFilter,
            string? luggageFilter)
        {
            var query = BaseFilterQuery(trackChanges, search, typeFilter, luggageFilter)
                .Where(x => x.FlightId == flightId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<IEnumerable<Ticket>> GetPagedByUserIdAsync(string userId, bool trackChanges,
            int page, int size, string? search,
            string? typeFilter,
            string? luggageFilter)
        {
            var query = BaseFilterQuery(trackChanges, search, typeFilter, luggageFilter)
               .Where(x => x.UserId == userId)
               .OrderByDescending(x => x.CreatedAt)
               .Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Ticket value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> InsertBulkAsync(List<Ticket> value)
        {
            await _Context.AddRangeAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUnreservedTickets()
        {
            var entities = await _Context.Tickets
                .Where(x => x.Reserved == false && x.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .ToListAsync();

            _Context.RemoveRange(entities);

            return await _Context.SaveChangesAsync() > 0;

        }

        public async Task<bool> UpdateAsync(Ticket value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTicketStatus(List<Guid> value)
        {
            var tickets = await _Context.Tickets
                .Where(x => value.Contains(x.Id))
                .ToListAsync();

            foreach (var item in tickets)
            {
                item.Reserved = true;
            }

            _Context.UpdateRange(tickets);
            return await _Context.SaveChangesAsync() > 0;
        }

        private IQueryable<Ticket> BaseFilterQuery(
            bool trackChanges,
            string? search,
            string? typeFilter,
            string? luggageFilter)
        {
            var q = _Context.Tickets
                .Include(t => t.Seat).ThenInclude(s => s.Plane)
                .Include(t => t.Flight).ThenInclude(f => f.DepartureAirport)
                .Include(t => t.Flight).ThenInclude(f => f.ArrivalAirport)
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(t =>
                    t.PersonFirstName.Contains(search) ||
                    t.PersonLastName.Contains(search) ||
                    t.Flight.FlightNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                if (Enum.TryParse<PersonType>(typeFilter, out var pt))
                    q = q.Where(t => t.PersonType == pt);
            }

            if (!string.IsNullOrWhiteSpace(luggageFilter))
            {
                if (Enum.TryParse<LuggageSize>(luggageFilter, out var ls))
                    q = q.Where(t => t.LuggageSize == ls);
            }

            return trackChanges ? q : q.AsNoTracking();
        }

    }
}