using EasyFly.Application.Dtos;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.ViewModels
{
    public class TicketDetailsViewModel
    {
        [Required]
        public Guid FlightId { get; set; }
        public Guid? ReturningFlightId { get; set; }
        [Required]
        public int RequiredSeats { get; set; }
        public List<ReserveTicketDto> Tickets { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal ReturningTicketPrice { get; set; }
    }
}