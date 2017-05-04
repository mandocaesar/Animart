using System;
using System.Data;
using System.Reflection;
using Abp.Modules;
using Abp.Zero;
using System.Transactions;

namespace Animart.Portal
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class PortalCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
