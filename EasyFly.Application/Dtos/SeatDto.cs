using EasyFly.Domain.Enums;
using EasyFly.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyFly.Domain;

namespace EasyFly.Application.Dtos
{
    public class SeatDto
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(Constants.RowLength)]
        public int Row { get; set; }
        public SeatLetter SeatLetter { get; set; }
        [Required]
        public Guid PlaneId { get; set; }
    }
}
