﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Flight : SoftDeletableEntity
    {
        public Flight()
        {
            Id = Guid.NewGuid();
            Tickets = new List<Ticket>();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(Constants.FlightNumberLength)]
        public string FlightNumber { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        [ForeignKey(nameof(DepartureAirport))]
        public Guid DepartureAirportId { get; set; }
        public Airport DepartureAirport { get; set; }
        [Required]
        [ForeignKey(nameof(ArrivalAirport))]
        public Guid ArrivalAirportId { get; set; }
        public Airport ArrivalAirport { get; set; }
        [Required]
        [ForeignKey(nameof(Plane))]
        public Guid PlaneId { get; set; }
        public Plane Plane { get; set; }
        [Required]
        public decimal TicketPrice { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
