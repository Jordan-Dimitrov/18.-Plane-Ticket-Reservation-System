using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyFly.Domain;

namespace EasyFly.Application.Dtos
{
    public class FlightDto
    {
        [Required]
        [MaxLength(Constants.FlightNumberLength)]
        public string FlightNumber { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        public Guid DepartureAirportId { get; set; }
        [Required]
        public Guid ArrivalAirportId { get; set; }
        [Required]
        public Guid PlaneId { get; set; }
        [Required]
        public decimal TicketPrice { get; set; }
    }
}
