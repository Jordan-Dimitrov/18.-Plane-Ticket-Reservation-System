using EasyFly.Application.Dtos;
using EasyFly.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.ViewModels
{
    public class AirportViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
    }
}
