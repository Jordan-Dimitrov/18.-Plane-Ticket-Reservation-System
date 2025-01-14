using System.ComponentModel.DataAnnotations;

namespace EasyFly.Domain.Models
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
        [MaxLength(Constants.NameLength)]
        public string Name { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longtitude { get; set; }
        public ICollection<Flight> Flights { get; set; }
    }
}
