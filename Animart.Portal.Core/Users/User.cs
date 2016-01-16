using Abp.Authorization.Users;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Users
{
    public class User :AbpUser<Tenant, User>
    { 
        public override string ToString()
        {
            return string.Format("[User {0}] {1}", Id, UserName);
        }
    }
}
