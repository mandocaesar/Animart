using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Users.Dto;

namespace Animart.Portal.Users
{
    public interface IUserAppService:IApplicationService
    {
        ListResultOutput<UserDto> GetUsers();
    }
}
