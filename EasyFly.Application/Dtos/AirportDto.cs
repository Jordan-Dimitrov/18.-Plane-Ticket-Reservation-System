using EasyFly.Domain;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class AirportDto
    {
        [Required]
        [MaxLength(Constants.NameLength)]
        public string Name { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longtitude { get; set; }
    }
}
