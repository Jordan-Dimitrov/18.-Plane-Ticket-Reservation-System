using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;

namespace EasyFly.Application.Abstractions
{
    public interface IPaymentService
    {
        public Session Checkout(CheckoutDto model, HostString host, string scheme);
    }
}
