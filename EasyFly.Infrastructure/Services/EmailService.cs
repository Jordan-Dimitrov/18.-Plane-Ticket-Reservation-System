using EasyFly.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace EasyFly.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IQrCodeService _QrCodeService;
        private readonly IConfiguration _configuration;

        public EmailService(IQrCodeService qrCodeService, IConfiguration configuration)
        {
            _QrCodeService = qrCodeService;
            _configuration = configuration;
        }
        public async Task SendEmailMessage(
              string recipientEmail,
              string baseUrl,
              string username,
              List<Guid> ticketIds)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var mail = new MailMessage
            {
                From = new MailAddress(emailSettings["Sender"]),
                Subject = "Your EasyFly Tickets",
                IsBodyHtml = true
            };
            mail.To.Add(recipientEmail);

            var sb = new StringBuilder();
            sb.Append($"<h2>Hello {username},</h2>");
            sb.Append("<p>Thank you for booking your tickets! You can view each ticket online, and we've attached the QR codes below:</p>");

            foreach (var ticketId in ticketIds)
            {
                string ticketUrl = $"{baseUrl}/Ticket/GetTicket?ticketId={ticketId}";

                byte[] qrBytes = _QrCodeService.GenerateQRCode(ticketUrl, 300);

                var qrStream = new MemoryStream(qrBytes);

                var attachment = new Attachment(qrStream, $"{ticketId}.png", "image/png");
                mail.Attachments.Add(attachment);

                sb.Append("<hr>");
                sb.Append($"<p><strong>Ticket ID:</strong> {ticketId}</p>");
                sb.Append($"<p><a href=\"{ticketUrl}\">View Your Ticket Online</a></p>");
                sb.Append($"<p>QR code attached as <em>{ticketId}.png</em></p>");
            }

            sb.Append("<p>Safe travels!</p>");
            sb.Append("<p>Best regards,<br/>EasyFly Team</p>");

            mail.Body = sb.ToString();

            using var smtp = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(
                    emailSettings["Username"],
                    emailSettings["Password"]
                ),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
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
