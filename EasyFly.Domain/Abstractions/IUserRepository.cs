﻿using EasyFly.Domain.Models;

namespace EasyFly.Domain.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        Task<int> Count();
    }
}
