using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class SeatPagedViewModel
    {
        public IEnumerable<SeatViewModel> Seats { get; set; }
        public int TotalPages { get; set; }
    }
}
