using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Dtos
{
    public class ReservationDto
    {
        public Guid DepartureAirportId { get; set; }
        public Guid ArrivalAirportId { get; set; }
        [Range(1, int.MaxValue)]
        public int NumberOfTickets { get; set; }
    }
}
