using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _Context;

        public FlightRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Flight value)
        {
            value.DeletedAt = DateTime.UtcNow;

            foreach (var item in value.Tickets)
            {
                item.DeletedAt = DateTime.UtcNow;
            }

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Flight, bool>> condition)
        {
            return await _Context.Flights.AnyAsync(condition);
        }

        public async Task<IEnumerable<Flight>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Flights;
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Flight?> GetByAsync(Expression<Func<Flight, bool>> condition)
        {
            return await _Context.Flights.FirstOrDefaultAsync(condition);
        }

        public async Task<Flight?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Flights.Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            var count = (double)await _Context.Flights.CountAsync() / size;

            return (int)Math.Ceiling(count);
        }

        public async Task<IEnumerable<Flight>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Flights.Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<IEnumerable<Flight>> GetPagedByArrivalAirportIdAsync(Guid airpordId, bool trackChanges, int page, int size)
        {
            var query = _Context.Flights.Where(x => x.ArrivalAirportId == airpordId).Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<IEnumerable<Flight>> GetPagedByDepartingAirportIdAsync(Guid airpordId, bool trackChanges, int page, int size)
        {
            var query = _Context.Flights.Where(x => x.DepartureAirportId == airpordId).Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }


        public async Task<bool> InsertAsync(Flight value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Flight value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}