using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class PayDto
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public long Amount { get; set; }
        public string? Currency { get; set; }
    }
}
