using EasyFly.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Seat : SoftDeletableEntity
    {
        public Seat()
        {
            Id = Guid.NewGuid();
            Tickets = new List<Ticket>();
        }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(Constants.RowLength)]
        public int Row { get; set; }
        public SeatLetter SeatLetter { get; set; }
        [Required]
        [ForeignKey(nameof(Plane))]
        public Guid PlaneId { get; set; }
        public Plane Plane { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

    }
}
