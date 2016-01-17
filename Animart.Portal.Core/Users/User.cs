using Abp.Authorization.Users;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Users
{
    public class User :AbpUser<Tenant, User>
    { 
       
    }
}
