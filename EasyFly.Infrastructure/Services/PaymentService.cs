using Stripe.Checkout;
using Stripe.Forwarding;
using Stripe;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using EasyFly.Application.Dtos;
using EasyFly.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EasyFly.Infrastructure.Services
{
    public class PaymentService : IPaymentService
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
                        UnitAmount = model.Amount,
                    },
                    Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = $"{scheme}://{host}/checkout/success",
                CancelUrl = $"{scheme}://{host}/checkout/cancel",
            };
            var service = new SessionService();
            var session = service.Create(options);

            return session;
        }
    }
}
