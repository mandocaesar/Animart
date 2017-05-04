using Abp.Web.Mvc.Views;

namespace Animart.Portal.Web.Views
{
    public abstract class PortalWebViewPageBase : PortalWebViewPageBase<dynamic>
    {

    }

    public abstract class PortalWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected PortalWebViewPageBase()
        {
            LocalizationSourceName = PortalConsts.LocalizationSourceName;
        }
    }
}