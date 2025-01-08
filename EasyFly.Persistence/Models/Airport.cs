
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Persistence.Models
{
    public class Airport
    {
        public Airport()
        {
            Id = Guid.NewGuid();
            Flights = new List<Flight>();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longtitude { get; set; }
        public ICollection<Flight> Flights { get; set; }
    }
}
