﻿using EasyFly.Application.Dtos;
using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class TicketViewModel
    {
        public Guid Id { get; set; }
        public SeatViewModel Seat { get; set; }
        public Guid FlightId { get; set; }
        public PersonType PersonType { get; set; }
        public UserDto User { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public Gender Gender { get; set; }
        public decimal Price { get; set; }
        public LuggageSize LuggageSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReserved { get; set; }
        public FlightViewModel Flight { get; set; }
    }
}
