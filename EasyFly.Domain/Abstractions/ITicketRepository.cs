using EasyFly.Domain.Models;

namespace EasyFly.Domain.Abstractions
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetPagedByFlightIdAsync(Guid flightId, bool trackChanges, int page, int size, string? search, string? typeFilter, string? luggageFilter);
        Task<IEnumerable<Ticket>> GetPagedByUserIdAsync(string userId, bool trackChanges, int page, int size, string? search, string? typeFilter, string? luggageFilter);
        Task<IEnumerable<Ticket>> GetPagedWithFilterAsync(bool trackChanges, int page, int size, string? search, string? typeFilter, string? luggageFilter);
        Task<int> GetPageCountWithFilter(int size, string? search, string? typeFilter, string? luggageFilter);
        Task<int> GetPageCountForUser(int size, string userId, string? search, string? typeFilter, string? luggageFilter);
        Task<int> GetPageCountForFlight(int size, Guid flightId, string? search, string? typeFilter, string? luggageFilter);
        Task<bool> InsertBulkAsync(List<Ticket> value);
        Task<bool> UpdateTicketStatus(List<Guid> value);
        Task<bool> RemoveUnreservedTickets();
        Task<int> Count();

    }
}
