using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;

namespace Animart.Portal.Authorization
{
    public class RoleStore : AbpRoleStore<Tenant, Role, User>
    {
        public RoleStore(
            IRepository<Role> roleRepository, 
            IRepository<UserRole, long> userRoleRepository, 
            IRepository<RolePermissionSetting, long> rolePermissionSettingRepository)
            : base(
                roleRepository, 
                userRoleRepository, 
                rolePermissionSettingRepository)
        {
        }
    }
}
