using EasyFly.Application.Abstractions;
using EasyFly.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class EmailServiceTests
    {
        private Mock<IQrCodeService> _qrCodeServiceMock;
        private Mock<IConfiguration> _configurationMock;
        private EmailService _emailService;

        [SetUp]
        public void Setup()
        {
            _qrCodeServiceMock = new Mock<IQrCodeService>();
            _configurationMock = new Mock<IConfiguration>();
            _emailService = new EmailService(_qrCodeServiceMock.Object, _configurationMock.Object);
        }

        [Test]
        public void BuildEmails_ShouldReturnEmailWithCorrectStructure_WhenTicketIdsProvided()
        {
            string baseUrl = "http://example.com";
            string username = "TestUser";
            var ticketIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            byte[] dummyQrCode = Encoding.UTF8.GetBytes("dummyQRCode");
            _qrCodeServiceMock.Setup(q => q.GenerateQRCode(It.IsAny<string>(), 300)).Returns(dummyQrCode);
            string emailContent = _emailService.BuildEmails(baseUrl, username, ticketIds);
            Assert.IsTrue(emailContent.Contains($"<h2>Hello {username},</h2>"));
            Assert.IsTrue(emailContent.Contains("Thank you for booking your tickets!"));
            foreach (var ticketId in ticketIds)
            {
                string ticketUrl = $"{baseUrl}/Ticket/GetTicket?ticketId={ticketId}";
                Assert.IsTrue(emailContent.Contains(ticketUrl));
                Assert.IsTrue(emailContent.Contains($"<strong>Ticket ID:</strong> {ticketId}"));
                string qrCodeImageMarkup = $"<img src='data:image/png;base64,{Convert.ToBase64String(dummyQrCode)}'";
                Assert.IsTrue(emailContent.Contains(qrCodeImageMarkup));
            }
            Assert.IsTrue(emailContent.Contains("Safe travels!"));
            Assert.IsTrue(emailContent.Contains("Best regards"));
        }

        [Test]
        public void BuildEmails_ShouldReturnEmailWithoutTickets_WhenTicketIdsEmpty()
        {
            string baseUrl = "http://example.com";
            string username = "TestUser";
            var ticketIds = new List<Guid>();
            string emailContent = _emailService.BuildEmails(baseUrl, username, ticketIds);
            Assert.IsTrue(emailContent.Contains($"<h2>Hello {username},</h2>"));
            Assert.IsFalse(emailContent.Contains("<hr>"));
            Assert.IsTrue(emailContent.Contains("Safe travels!"));
            Assert.IsTrue(emailContent.Contains("Best regards"));
        }
    }
}
