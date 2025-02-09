using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class PlaneViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
