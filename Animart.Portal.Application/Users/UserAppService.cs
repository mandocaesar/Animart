using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.User.Dto;
using Animart.Portal.Users;
using Animart.Portal.Users.Dto;

namespace Animart.Portal.Users
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
            var b = _userManager.Users;
            var a = b.ToList().MapTo<List<UserDto>>();
            return new ListResultOutput<UserDto>
            {
                Items = a
            };
        }
    }
}
