using EasyFly.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    internal class EmailService : IEmailService
    {
        private readonly IQrCodeService _QrCodeService;

        public EmailService(IQrCodeService qrCodeService)
        {
            _QrCodeService = qrCodeService;
        }
        public string BuildEmail(string baseUrl, string username, Guid ticketId)
        {
            string ticketUrl = $"{baseUrl}/Ticket/GetTicket?ticketId={ticketId}";

            byte[] qrCodeBytes = _QrCodeService.GenerateQRCode(ticketUrl, 300);

            string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
            string qrCodeImage = $"<img src='data:image/png;base64,{qrCodeBase64}' width='200' height='200'/>";

            var sb = new StringBuilder();
            sb.Append($"<h2>Hello {username},</h2>");
            sb.Append("<p>Thank you for booking your ticket! You can access your ticket using the link below:</p>");
            sb.Append($"<p><a href='{ticketUrl}'>View Your Ticket</a></p>");
            sb.Append("<p>Or scan the QR code below:</p>");
            sb.Append(qrCodeImage);
            sb.Append("<p>Safe travels!</p>");
            sb.Append("<p>Best regards,<br>EasyFly Team</p>");

            return sb.ToString();
        }
    }
}
