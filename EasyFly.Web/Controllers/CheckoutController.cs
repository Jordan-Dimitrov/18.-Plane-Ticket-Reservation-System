using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using EasyFly.Application.Dtos;
using EasyFly.Application.Abstractions;

namespace EasyFly.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : Controller
    {
        private readonly IPaymentService _PaymentService;
        public CheckoutController(IPaymentService paymentService)
        {
            _PaymentService = paymentService;
        }
        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession([FromBody] PayDto model)
        {
            var session = _PaymentService.Checkout(model, Request.Host, Request.Scheme);
            
            return Ok(new { sessionId = session.Id });
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
