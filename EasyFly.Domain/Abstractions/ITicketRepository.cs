using EasyFly.Domain.Models;

namespace EasyFly.Domain.Abstractions
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetPagedByFlightIdAsync(Guid flightId, bool trackChanges, int page, int size);
        Task<IEnumerable<Ticket>> GetPagedByUserIdAsync(string userId, bool trackChanges, int page, int size);
        Task<bool> InsertBulkAsync(List<Ticket> value);
        Task<bool> UpdateTicketStatus(List<Guid> value);
        Task<bool> RemoveUnreservedTickets();
        Task<int> Count();

    }
}
