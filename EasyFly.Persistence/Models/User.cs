using Microsoft.AspNetCore.Identity;

namespace EasyFly.Persistence.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Ticket = new List<Ticket>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public uint Phone { get; set; }
        public ICollection<Ticket> Ticket { get; set; }

    }
}
