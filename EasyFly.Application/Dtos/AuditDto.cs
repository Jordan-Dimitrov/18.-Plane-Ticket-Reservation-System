using EasyFly.Domain;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class AuditDto
    {
        public DateTime ModifiedAt { get; set; }
        [Required]
        [MaxLength(Constants.MessageLength)]
        public string Message { get; set; }
    }
}
