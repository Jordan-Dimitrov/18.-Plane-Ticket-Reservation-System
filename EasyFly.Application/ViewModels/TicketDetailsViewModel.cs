using EasyFly.Application.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.ViewModels
{
    public class TicketDetailsViewModel
    {
        [Required]
        public Guid FlightId { get; set; }
        [Required]
        public int RequiredSeats { get; set; }
        public List<ReserveTicketDto> Tickets { get; set; }
    }
}