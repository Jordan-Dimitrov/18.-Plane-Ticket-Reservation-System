using EasyFly.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface IPaymentService
    {
        public Session Checkout(CheckoutDto model, HostString host, string scheme);
    }
}
