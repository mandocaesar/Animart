using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Animart.Portal.Web.App_Start;

namespace Animart.Portal.Web
{
    [DependsOn(typeof(PortalDataModule), typeof(PortalApplicationModule), typeof(PortalWebApiModule))]
    public class PortalWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Add Authentication Provider
            Configuration.Authorization.Providers.Add<PortalAuthorizationProvider>();

            //Add/remove languages for your application
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flag-england", true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flag-tr"));
            Configuration.Localization.Languages.Add(new LanguageInfo("zh-CN", "简体中文", "famfamfam-flag-cn"));

            //Add/remove localization sources here
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    PortalConsts.LocalizationSourceName,
                    new XmlFileLocalizationDictionaryProvider(
                        HttpContext.Current.Server.MapPath("~/Localization/Portal")
                        )
                    )
                );

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<PortalNavigationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            EnableCors();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private static void EnableCors()
        {
            //This method enables cross origin request

            var cors = new EnableCorsAttribute("*", "*", "*");
            GlobalConfiguration.Configuration.EnableCors(cors);

            //Then, we can call getTasks method from any web site like that:

            /*
             
                 $.ajax({
                    url: 'http://localhost:6247/api/services/tasksystem/task/GetTasks',
                    type: "POST",
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify({})
                }).done(function(result) {
                    console.log(result);
                });
             
             */
        }
    }
}
