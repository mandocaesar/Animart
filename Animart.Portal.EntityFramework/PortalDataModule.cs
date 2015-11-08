using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Animart.Portal.EntityFramework;

namespace Animart.Portal
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(PortalCoreModule))]
    public class PortalDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<PortalDbContext>(null);
        }
    }
}
