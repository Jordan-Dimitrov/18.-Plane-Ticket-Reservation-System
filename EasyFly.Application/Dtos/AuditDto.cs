using EasyFly.Domain;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
