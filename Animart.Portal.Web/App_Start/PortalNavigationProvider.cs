using Abp.Application.Navigation;
using Abp.Localization;

namespace Animart.Portal.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See .cshtml and .js files under App/Main/views/layout/header to know how to render menu.
    /// </summary>
    public class PortalNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        "Home",
                        new LocalizableString("HomePage", PortalConsts.LocalizationSourceName),
                        url: "#/",
                        icon: "fa fa-home",
                        requiresAuthentication: true
                        )
                )//.AddItem(
                 //    new MenuItemDefinition(
                 //        "About",
                 //        new LocalizableString("About", PortalConsts.LocalizationSourceName),
                 //        url: "#/about",
                 //        icon: "fa fa-info"
                 //        )
                 //)
                .AddItem(
                    new MenuItemDefinition(
                        "RetailerDashboard",
                        new LocalizableString("RetailerDashboard", PortalConsts.LocalizationSourceName),
                        url: "#/retailerDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessRetailer"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "HowToOrder",
                        new LocalizableString("HowToOrder", PortalConsts.LocalizationSourceName),
                        url: "/#/hwto",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessRetailer"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "MarketingDashboard",
                        new LocalizableString("MarketingDashboard", PortalConsts.LocalizationSourceName),
                        url: "#/marketingDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessMarketing"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "SupplyCategory",
                        new LocalizableString("Category", PortalConsts.LocalizationSourceName),
                        url: "#/category",
                        icon: "fa fa-sitemap",
                        requiredPermissionName: "CanAccessMarketing"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Supply",
                        new LocalizableString("Supply", PortalConsts.LocalizationSourceName),
                        url: "#/supply",
                        icon: "fa fa-archive",
                        requiredPermissionName: "CanAccessMarketing"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "LogisticDashboard",
                        new LocalizableString("LogisticDashboard", PortalConsts.LocalizationSourceName),
                        url: "#/logisticDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessLogistic"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Expedition",
                        new LocalizableString("Expedition", PortalConsts.LocalizationSourceName),
                        url: "#/expedition",
                        icon: "fa fa-truck",
                        requiredPermissionName: "CanAccessLogistic"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Cities",
                        new LocalizableString("Cities", PortalConsts.LocalizationSourceName),
                        url: "#/cities",
                        icon: "fa fa-building-o",
                        requiredPermissionName: "CanAccessLogistic"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "AccountingDashboard",
                        new LocalizableString("AccountingDashboard", PortalConsts.LocalizationSourceName),
                        url: "#/accountingDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessAccounting"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Administration",
                        new LocalizableString("Administration", PortalConsts.LocalizationSourceName),
                        requiredPermissionName: "CanAccessAdministrator",
                        icon: "fa fa-cogs"
                        ).AddItem(
                            new MenuItemDefinition(
                                "SupplyCategory",
                                new LocalizableString("Category", PortalConsts.LocalizationSourceName),
                                url: "#/category",
                                icon: "fa fa-sitemap",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Supply",
                                new LocalizableString("Supply", PortalConsts.LocalizationSourceName),
                                url: "#/supply",
                                icon: "fa fa-archive",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Users",
                                new LocalizableString("Users", PortalConsts.LocalizationSourceName),
                                url: "#/users",
                                icon: "fa fa-users",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Expedition",
                                new LocalizableString("Expedition", PortalConsts.LocalizationSourceName),
                                url: "#/expedition",
                                icon: "fa fa-truck",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Cities",
                                new LocalizableString("Cities", PortalConsts.LocalizationSourceName),
                                url: "#/cities",
                                icon: "fa fa-building-o",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        )
                );
        }
    }
}
