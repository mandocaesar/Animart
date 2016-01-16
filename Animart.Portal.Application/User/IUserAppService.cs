using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.User.Dto;

namespace Animart.Portal.User
{
    public interface IUserAppService:IApplicationService
    {
        ListResultOutput<UserDto> GetUsers();
    }
}
