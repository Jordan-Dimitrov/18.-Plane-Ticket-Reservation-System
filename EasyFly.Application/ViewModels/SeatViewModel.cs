using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class SeatViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public SeatLetter SeatLetter { get; set; }
        public PlaneViewModel Plane { get; set; }
    }
}
