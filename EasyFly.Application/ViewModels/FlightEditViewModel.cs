using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class FlightEditViewModel
    {
        public FlightViewModel FlightViewModel { get; set; }
        public IEnumerable<AirportViewModel> Airports { get; set; }

    }
}
