using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.ViewModels
{
    public class HomeViewModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int RegisteredUsersCount { get; set; }
        [Range(0, int.MaxValue)]
        public int TicketCount { get; set; }
    }
}
