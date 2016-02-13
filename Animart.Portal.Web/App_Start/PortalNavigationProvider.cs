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
                ).AddItem(
                    new MenuItemDefinition(
                        "About",
                        new LocalizableString("About", PortalConsts.LocalizationSourceName),
                        url: "#/about",
                        icon: "fa fa-info"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "RetailerDashboard",
                        new LocalizableString("Dashboard", PortalConsts.LocalizationSourceName),
                        url: "#/retailerDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessRetailer"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "BodDashboard",
                        new LocalizableString("Dashboard", PortalConsts.LocalizationSourceName),
                        url: "#/bodDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessBOD"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "LogisticDashboard",
                        new LocalizableString("Dashboard", PortalConsts.LocalizationSourceName),
                        url: "#/logisticDashboard",
                        icon: "fa fa-info",
                        requiredPermissionName: "CanAccessLogistic"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "AcountingDashboard",
                        new LocalizableString("Dashboard", PortalConsts.LocalizationSourceName),
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
                                icon: "fa fa-users",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "Cities",
                                new LocalizableString("Cities", PortalConsts.LocalizationSourceName),
                                url: "#/cities",
                                icon: "fa fa-users",
                                requiredPermissionName: "CanAccessAdministrator"
                                )
                        )
                );
        }
    }
}
