using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Authorization;

namespace Animart.Portal.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string Role { get; set; }

    }
}
