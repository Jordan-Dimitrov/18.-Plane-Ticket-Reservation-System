using EasyFly.Persistence.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Persistence.Models
{
    public class Ticket
    {
        public Ticket()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public Seat Seat { get; set; }
        [Required]
        [ForeignKey(nameof(Seat))]
        public Guid SeatId { get; set; }
        public PersonType PersonType { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
    }
}
