using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;

namespace Animart.Portal
{
    [DependsOn(typeof(PortalCoreModule), typeof(AbpAutoMapperModule))]
    public class PortalApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

        }
    }
}
