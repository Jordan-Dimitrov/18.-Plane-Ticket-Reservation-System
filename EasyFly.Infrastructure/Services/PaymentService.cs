using EasyFly.Application.Abstractions;
using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace EasyFly.Infrastructure.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly IConfiguration _Configuration;
        public PaymentService(IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        public Session Checkout(CheckoutDto model, HostString host, string scheme)
        {
            StripeConfiguration.ApiKey = _Configuration["Stripe:SecretKey"];

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = model.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = model.ProductName,
                            Description = model.ProductDescription,
                        },
                        UnitAmountDecimal = model.Amount,
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = $"{scheme}://{host}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{scheme}://{host}/checkout/cancel",
                Metadata = new Dictionary<string, string>
                {
                    {"ticketIds", string.Join(",", model.Tickets) }
                }
            };
            var service = new SessionService();
            var session = service.Create(options);

            return session;
        }
    }
}
