using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Dtos
{
    public class ReservationDto
    {
        public Guid DepartureAirportId { get; set; }
        public Guid ArrivalAirportId { get; set; }
        public int NumberOfTickets { get; set; }
    }
}
