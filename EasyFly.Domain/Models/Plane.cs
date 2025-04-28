using System.ComponentModel.DataAnnotations;

namespace EasyFly.Domain.Models
{
    public class Plane : SoftDeletableEntity
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
        [MaxLength(Constants.NameLength)]
        public string Name { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Flight> Flights { get; set; }

    }
}
