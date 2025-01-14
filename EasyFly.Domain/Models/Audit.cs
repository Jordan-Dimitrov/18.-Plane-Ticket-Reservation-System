using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyFly.Domain.Models
{
    public class Audit
    {
        public Audit()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public DateTime ModifiedAt { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [Required]
        [MaxLength(Constants.MessageLength)]
        public string Message { get; set; }
    }
}
