using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace EasyFly.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : BaseController
    {
        private readonly IPaymentService _PaymentService;
        private readonly IConfiguration _Configuration;
        private readonly ITicketService _TicketService;

        public CheckoutController(IPaymentService paymentService, IAuditService auditService,
            IConfiguration configuration, ITicketService ticketService)
            : base(auditService)
        {
            _PaymentService = paymentService;
            _Configuration = configuration;
            _TicketService = ticketService;
        }
        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession([FromBody] CheckoutDto model)
        {
            var session = _PaymentService.Checkout(model, Request.Host, Request.Scheme);

            return Ok(new { sessionId = session.Id });
        }
        [Route("/checkout/success")]
        public async Task<IActionResult> Success(string session_Id)
        {
            if (string.IsNullOrEmpty(session_Id))
            {
                TempData["ErrorMessage"] = "Session ID is missing.";
                return RedirectToAction("Error");
            }

            StripeConfiguration.ApiKey = _Configuration["Stripe:SecretKey"];
            var service = new SessionService();
            var session = await service.GetAsync(session_Id);

            if (session.PaymentStatus == "paid")
            {
                var ticketIds = session.Metadata["ticketIds"].Split(',').Select(Guid.Parse).ToList();

                var response =  await _TicketService.UpdateTicketStatus(ticketIds);

                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.ErrorMessage;
                    return RedirectToAction("Error");
                }

                TempData["SuccessMessage"] = "Payment successful. Your tickets are now reserved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Payment failed. Please try again.";
            }

            return View();
        }
        [Route("/checkout/cancel")]
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
