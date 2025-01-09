using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Flight
    {
        public Flight()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        [ForeignKey(nameof(DepartureAirport))]
        public Guid DepartureAirportId { get; set; }
        public Airport DepartureAirport { get; set; }
        [Required]
        [ForeignKey(nameof(ArrivalAirport))]
        public Guid ArrivalAirportId { get; set; }
        public Airport ArrivalAirport { get; set; }
        [Required]
        [ForeignKey(nameof(Plane))]
        public Guid PlaneId { get; set; }
        public Plane Plane { get; set; }
    }
}
