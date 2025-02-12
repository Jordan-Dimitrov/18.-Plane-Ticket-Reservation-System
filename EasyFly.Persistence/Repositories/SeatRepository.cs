using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace EasyFly.Persistence.Repositories
{
    internal class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _Context;

        public SeatRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<bool> DeleteAsync(Seat value)
        {
            value.DeletedAt = DateTime.UtcNow;

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Seat, bool>> condition)
        {
            return await _Context.Seats.AnyAsync(condition);
        }

        public async Task<IEnumerable<Seat>> GetAllAsync(bool trackChanges)
        {
            var query = _Context.Seats;
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<Seat?> GetByAsync(Expression<Func<Seat, bool>> condition)
        {
            return await _Context.Seats.FirstOrDefaultAsync(condition);
        }

        public async Task<Seat?> GetByIdAsync(Guid id, bool trackChanges)
        {
            var query = _Context.Seats.Where(x => x.Id == id);
            return await (trackChanges ? query.FirstOrDefaultAsync() : query.AsNoTracking().FirstOrDefaultAsync());
        }

        public async Task<int> GetPageCount(int size)
        {
            var count = (double)await _Context.Seats.CountAsync() / size;

            return (int)Math.Ceiling(count);
        }

        public async Task<IEnumerable<Seat>> GetPagedAsync(bool trackChanges, int page, int size)
        {
            var query = _Context.Seats.Skip((page - 1) * size).Take(size);
            return await (trackChanges ? query.ToListAsync() : query.AsNoTracking().ToListAsync());
        }

        public async Task<bool> InsertAsync(Seat value)
        {
            await _Context.AddAsync(value);
            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GenerateSeatsForPlane(int availableSeats, Guid planeId)
        {
            for (int i = 0; i < availableSeats; i++)
            {
                for (int j = 0; j < (int)SeatLetter.F; j++)
                {
                    Seat seat = new Seat()
                    {
                        Row = i,
                        SeatLetter = (SeatLetter)j,
                        PlaneId = planeId
                    };

                    await _Context.AddAsync(seat);
                }
            }

            return await _Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Seat value)
        {
            _Context.Update(value);
            return await _Context.SaveChangesAsync() > 0;
        }
    }
}