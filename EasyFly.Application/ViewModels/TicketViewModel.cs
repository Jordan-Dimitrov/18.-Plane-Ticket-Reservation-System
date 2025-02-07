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
    public class TicketViewModel
    {
        public Guid Id { get; set; }
        public SeatViewModel Seat { get; set; }
        public PersonType PersonType { get; set; }
        public UserDto User { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public Gender Gender { get; set; }
        public decimal Price { get; set; }
    }
}
