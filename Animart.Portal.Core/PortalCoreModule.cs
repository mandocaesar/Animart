using System.Reflection;
using Abp.Modules;

namespace Animart.Portal
{
    public class PortalCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
