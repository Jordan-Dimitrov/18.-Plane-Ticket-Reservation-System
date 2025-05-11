using EasyFly.Application.Dtos;

namespace EasyFly.Application.ViewModels
{
    public class UserPagedViewModel
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
