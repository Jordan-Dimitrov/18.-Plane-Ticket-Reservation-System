using EasyFly.Domain;
using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Dtos
{
    public class TicketDto
    {
        [Required]
        public Guid SeatId { get; set; }
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
        [Range(0, 99999)]
        public decimal Price { get; set; }
        [Required]
        public LuggageSize LuggageSize { get; set; }
        [Required]
        public Guid FlightId { get; set; }
    }
}
