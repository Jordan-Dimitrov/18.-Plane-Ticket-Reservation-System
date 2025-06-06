﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class PlanePagedViewModel
    {
        public IEnumerable<PlaneViewModel> Planes { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
