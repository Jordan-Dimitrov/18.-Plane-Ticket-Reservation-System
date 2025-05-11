using EasyFly.Domain;
using EasyFly.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class ReserveTicketDto
    {
        [Required]
        public PersonType PersonType { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(Constants.NameLength)]
        public string PersonFirstName { get; set; }
        [Required]
        [MaxLength(Constants.NameLength)]
        public string PersonLastName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public LuggageSize LuggageSize { get; set; }
        [Required]
        public Guid FlightId { get; set; }
    }
}
