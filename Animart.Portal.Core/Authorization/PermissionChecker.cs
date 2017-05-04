using Abp.Authorization;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;

namespace Animart.Portal.Authorization
{
    public class PermissionChecker : PermissionChecker<Tenant, Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}