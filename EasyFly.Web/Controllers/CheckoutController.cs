using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using EasyFly.Domain.Abstractions;
using EasyFly.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace EasyFly.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : BaseController
    {
        private readonly IPaymentService _PaymentService;
        private readonly IConfiguration _Configuration;
        private readonly ITicketService _TicketService;
        private readonly IEmailService _EmailService;
        private readonly IEmailSender _EmailSender;
        public CheckoutController(IPaymentService paymentService, IAuditService auditService,
            IConfiguration configuration, ITicketService ticketService,
            IEmailSender emailSender, IEmailService emailService)
            : base(auditService)
        {
            _PaymentService = paymentService;
            _Configuration = configuration;
            _TicketService = ticketService;
            _EmailSender = emailSender;
            _EmailService = emailService;
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
                var username = User.Identity.IsAuthenticated
                   ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Valued Customer"
                   : "Valued Customer";

                var userEmail = User.Identity.IsAuthenticated
                    ? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty
                    : string.Empty;

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                string emailContent = _EmailService.BuildEmails(baseUrl, username, ticketIds);
                await _EmailSender.SendEmailAsync(userEmail, "Tickets", emailContent);
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
