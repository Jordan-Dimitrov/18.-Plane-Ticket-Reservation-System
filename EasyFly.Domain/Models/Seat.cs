using EasyFly.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Seat
    {
        public Seat()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public int Row { get; set; }
        public SeatLetter SeatLetter { get; set; }
        [Required]
        [ForeignKey(nameof(Flight))]
        public Guid FlightId { get; set; }
        public Flight Flight { get; set; }

    }
}
