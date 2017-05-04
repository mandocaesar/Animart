using Abp.Authorization.Roles;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;

namespace Animart.Portal.Authorization
{
    public class Role : AbpRole<Tenant, User>
    {
        public Role()
        {

        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {

        }
    }
}