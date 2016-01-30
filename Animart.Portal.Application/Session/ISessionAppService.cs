
using System.Threading.Tasks;
using Abp.Application.Services;
using Animart.Portal.Sessions.Dto;

namespace Animart.Portal.Sessions
{
    interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
