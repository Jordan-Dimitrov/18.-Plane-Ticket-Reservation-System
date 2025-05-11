using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyFly.Domain;
using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class FlightViewModel
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public Guid DepartureAirportId { get; set; }
        public AirportViewModel DepartureAirport { get; set; }
        public Guid ArrivalAirportId { get; set; }
        public AirportViewModel ArrivalAirport { get; set; }
        public Guid PlaneId { get; set; }
        public PlaneViewModel Plane { get; set; }
        public decimal TicketPrice { get; set; }
        public int FreeSeatCount { get; set; }
        public IEnumerable<FlightViewModel> ReturningFlights { get; set; }
    }
}
