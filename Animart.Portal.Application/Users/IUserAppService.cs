using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Animart.Portal.Users.Dto;
using Animart.Portal.Sessions.Dto;

namespace Animart.Portal.Users
{
    public interface IUserAppService:IApplicationService
    {
        ListResultOutput<UserDto> GetUsers();
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
