using System.Reflection;
using Abp.Modules;
using Abp.Zero;

namespace Animart.Portal
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class PortalCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
