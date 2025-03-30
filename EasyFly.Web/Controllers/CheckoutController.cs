using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EasyFly.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : BaseController
    {
        private readonly IPaymentService _PaymentService;
        public CheckoutController(IPaymentService paymentService, IAuditService auditService)
            : base(auditService)
        {
            _PaymentService = paymentService;
        }
        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession([FromBody] CheckoutDto model)
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
