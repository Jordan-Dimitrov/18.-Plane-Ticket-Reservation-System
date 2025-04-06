using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class FlightReservationsViewModel
    {
        public FlightPagedViewModel DepartingFlights { get; set; }
        public FlightPagedViewModel ReturningFlights { get; set; }
    }
}
