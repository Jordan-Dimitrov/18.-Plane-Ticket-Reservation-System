using EasyFly.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Dtos
{
    public class PlaneDto
    {
        [Required]
        [MaxLength(Constants.NameLength)]
        public string Name { get; set; }
        public int AvailableSeats { get; set; }
    }
}
