using EasyFly.Domain;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class PlaneDto
    {
        [Required]
        [MaxLength(Constants.NameLength)]
        public string Name { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int AvailableSeats { get; set; }
    }
}
