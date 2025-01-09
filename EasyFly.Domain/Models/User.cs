using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EasyFly.Domain.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Ticket = new List<Ticket>();
        }

        [Required]
        public string Address { get; set; }
        [Required]
        public uint Phone { get; set; }
        public ICollection<Ticket> Ticket { get; set; }
    }
}
