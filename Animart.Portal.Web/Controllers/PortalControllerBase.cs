using Abp.Web.Mvc.Controllers;

namespace Animart.Portal.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class PortalControllerBase : AbpController
    {
        protected PortalControllerBase()
        {
            LocalizationSourceName = PortalConsts.LocalizationSourceName;
        }
    }
}