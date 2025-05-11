using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class HomeViewModel
    {
        [Required]
        [Range(0,int.MaxValue)]
        public int RegisteredUsersCount { get; set; }
        [Range(0, int.MaxValue)]
        public int TicketCount { get; set; }
    }
}
