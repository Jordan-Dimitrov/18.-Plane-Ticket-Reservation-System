using EasyFly.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyFly.Application.ViewModels
{
    public class UserPagedViewModel
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
