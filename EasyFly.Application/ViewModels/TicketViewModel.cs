using EasyFly.Application.Dtos;
using EasyFly.Domain.Enums;

namespace EasyFly.Application.ViewModels
{
    public class TicketViewModel
    {
        public Guid Id { get; set; }
        public SeatViewModel Seat { get; set; }
        public Guid FlightId { get; set; }
        public PersonType PersonType { get; set; }
        public UserDto User { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public Gender Gender { get; set; }
        public decimal Price { get; set; }
        public LuggageSize LuggageSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReserved { get; set; }
        public FlightViewModel Flight { get; set; }
    }
}
