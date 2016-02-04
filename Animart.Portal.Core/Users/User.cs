using System;
using Abp.Authorization.Users;
using Abp.Extensions;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.Users
{
    public class User :AbpUser<Tenant, User>
    {
        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }
    }
}
