using System.ComponentModel.DataAnnotations;

namespace EasyFly.Persistence.Models
{
    public class Plane
    {
        public Plane()
        {
            Id = Guid.NewGuid();
            Seats = new List<Seat>();
            Flights = new List<Flight>();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Flight> Flights { get; set; }

    }
}
