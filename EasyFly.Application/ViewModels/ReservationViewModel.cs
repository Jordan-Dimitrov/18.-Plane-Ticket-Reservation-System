using System;
using System.Collections.Generic;
using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class ReservationViewModel
    {
        public List<AirportViewModel> Airports { get; set; }
        public Guid DepartureAirportId { get; set; }
        public Guid ArrivalAirportId { get; set; }
        public ReservationDto Reservation { get; set; }
    }
}