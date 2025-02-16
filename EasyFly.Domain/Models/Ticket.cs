using EasyFly.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Ticket : SoftDeletableEntity
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
        public Flight Flight { get; set; }
        [Required]
        [ForeignKey(nameof(Flight))]
        public Guid FlightId { get; set; }
        public PersonType PersonType { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [Required]
        [MaxLength(Constants.NameLength)]
        public string PersonFirstName { get; set; }
        [Required]
        [MaxLength(Constants.NameLength)]
        public string PersonLastName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public LuggageSize LuggageSize { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool Reserved { get; set; }
    }
}
