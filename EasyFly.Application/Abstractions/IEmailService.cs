using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.Abstractions
{
    public interface IEmailService
    {
        public Task SendEmailMessage(
              string recipientEmail,
              string baseUrl,
              string username,
              List<Guid> ticketIds);
        public string BuildEmails(string baseUrl, string username, List<Guid> ticketIds);
    }
}
