﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class FlightPagedViewModel
    {
        public IEnumerable<FlightViewModel> Flights { get; set; }
        public IEnumerable<AirportViewModel> Airports { get; set; }
        public IEnumerable<PlaneViewModel> Planes { get; set; }
        public Guid? SelectedPlaneId { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
