using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Runtime.Caching;
using Abp.Zero.Configuration;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;

namespace Animart.Portal.Authorization
{
    public class RoleManager : AbpRoleManager<Tenant, Role, User>
    {
        public RoleManager(
            RoleStore roleStore, 
            IPermissionManager permissionManager, 
            IRoleManagementConfig roleManagementConfig, 
            ICacheManager cacheManager) 
            : base(
                roleStore, 
                permissionManager, 
                roleManagementConfig,
                cacheManager)
        {
        }
    }
}
