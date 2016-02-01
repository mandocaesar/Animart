using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Animart.Portal.User.Dto
{
    [AutoMapFrom(typeof(Users.User))]
    public class RegisterUserInput:CreationAuditedEntityDto
    {
        public string FirstName { get; set;}
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
