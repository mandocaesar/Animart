﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Users.Dto;
using Animart.Portal.Sessions.Dto;
using Animart.Portal.User.Dto;

namespace Animart.Portal.Users
{
    public interface IUserAppService:IApplicationService
    {
        Task CreateUser(RegisterUserInput user);
        Task UpdateUser(UserDto user);
        Task UpdateUserRole(int userId, string role);
        List<UserDto> GetUsers();
        ListResultOutput<UserDto> GetUsersList();
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
