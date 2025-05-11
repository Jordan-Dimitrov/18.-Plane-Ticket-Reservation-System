using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class ReservationDto
    {
        [Required]
        public Guid DepartureAirportId { get; set; }
        [Required]
        public Guid ArrivalAirportId { get; set; }
        [Range(1, int.MaxValue)]
        public int NumberOfTickets { get; set; }
        [Required]
        public DateOnly Departure { get; set; }
    }
}
