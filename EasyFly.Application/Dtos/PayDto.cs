using System.ComponentModel.DataAnnotations;

namespace EasyFly.Application.Dtos
{
    public class PayDto
    {
        [Required]
        [Display(Name = "Cardholder Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumder { get; set; }

        [Required]
        [Display(Name = "Expiration Month")]
        public string Month { get; set; }

        [Required]
        [Display(Name = "Expiration Year")]
        public string Year { get; set; }

        [Required]
        public string CVC { get; set; }

        [Required]
        public int Amount { get; set; }
    }
}
