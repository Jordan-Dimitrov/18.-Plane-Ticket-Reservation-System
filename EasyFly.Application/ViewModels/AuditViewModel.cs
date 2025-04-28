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
    public class AuditViewModel
    {
        public Guid Id { get; set; }
        public DateTime ModifiedAt { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
    }
}
