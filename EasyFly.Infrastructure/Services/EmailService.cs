using EasyFly.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IQrCodeService _QrCodeService;

        public EmailService(IQrCodeService qrCodeService)
        {
            _QrCodeService = qrCodeService;
        }
        public string BuildEmails(string baseUrl, string username, List<Guid> ticketIds)
        {
            var sb = new StringBuilder();

            sb.Append($"<h2>Hello {username},</h2>");
            sb.Append("<p>Thank you for booking your tickets! You can access them using the links below:</p>");

            foreach (var ticketId in ticketIds)
            {
                string ticketUrl = $"{baseUrl}/Ticket/GetTicket?ticketId={ticketId}";
                byte[] qrCodeBytes = _QrCodeService.GenerateQRCode(ticketUrl, 300);
                string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
                string qrCodeImage = $"<img src='data:image/png;base64,{qrCodeBase64}' width='200' height='200'/>";

                sb.Append("<hr>");
                sb.Append($"<p><strong>Ticket ID:</strong> {ticketId}</p>");
                sb.Append($"<p><a href='{ticketUrl}'>View Your Ticket</a></p>");
                sb.Append("<p>Or scan the QR code below:</p>");
                sb.Append(qrCodeImage);
            }

            sb.Append("<p>Safe travels!</p>");
            sb.Append("<p>Best regards,<br>EasyFly Team</p>");

            return sb.ToString();
        }

    }
}
