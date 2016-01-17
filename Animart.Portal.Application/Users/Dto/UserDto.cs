using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Authorization;

namespace Animart.Portal.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto: EntityDto<long>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public Role Roles { get; set; }
        
    }
}
