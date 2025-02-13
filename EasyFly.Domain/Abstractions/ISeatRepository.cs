using EasyFly.Domain.Models;

namespace EasyFly.Domain.Abstractions
{
    public interface ISeatRepository : IRepository<Seat>
    {
        Task<bool> GenerateSeatsForPlane(int availableSeats, Guid planeId);
        Task<IEnumerable<Seat>> GetPagedForFlightAsync(Guid flightId, bool trackChanges, int page, int size);
    }
}
