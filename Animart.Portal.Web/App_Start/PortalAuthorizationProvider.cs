using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abp.Authorization;

namespace Animart.Portal.Web.App_Start
{
    public class PortalAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("CanAccessAdministrator");
            context.CreatePermission("CanAccessLogistic");
            context.CreatePermission("CanAccessAccounting");
            context.CreatePermission("CanAccessMarketing");
            context.CreatePermission("CanAccessRetailer");
        }
    }
}