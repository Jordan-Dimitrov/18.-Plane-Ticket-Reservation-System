using Microsoft.AspNetCore.Identity;

namespace EasyFly.Domain.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Ticket = new List<Ticket>();
            Audits = new List<Audit>();
        }

        public DateTime? DeletedAt { get; set; }
        public ICollection<Audit> Audits { get; set; }
        public ICollection<Ticket> Ticket { get; set; }

    }
}
