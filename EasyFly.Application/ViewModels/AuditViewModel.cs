using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class AuditViewModel
    {
        public Guid Id { get; set; }
        public DateTime ModifiedAt { get; set; }
        public UserDto User { get; set; }
        public string Message { get; set; }
    }
}
