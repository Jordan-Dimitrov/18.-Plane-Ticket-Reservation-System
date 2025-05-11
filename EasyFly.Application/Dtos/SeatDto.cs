using EasyFly.Domain;
using EasyFly.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
