using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.User.Dto;
using Animart.Portal.Users;

namespace Animart.Portal.User
{
    public class UserAppService:ApplicationService, IUserAppService
    {
        private readonly UserManager _userManager;

        public UserAppService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public ListResultOutput<UserDto> GetUsers()
        {
            return new ListResultOutput<UserDto>
            {
                Items = _userManager.Users.ToList().MapTo<List<UserDto>>()
            };
        }
    }
}
