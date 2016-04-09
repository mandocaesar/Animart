using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Animart.Portal.Authorization;

namespace Animart.Portal.Sessions.Dto
{
    [AutoMapFrom(typeof(Users.User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public ICollection<UserRole> Roles { get; set; }

        public string CurrentRole => Roles.FirstOrDefault().RoleId.ToString();
    }
}
