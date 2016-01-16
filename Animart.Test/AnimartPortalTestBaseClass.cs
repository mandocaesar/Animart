using System;
using System.Data.Common;
using System.Linq;
using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Animart.Portal.EntityFramework;
using Animart.Portal.Migrations.Data;

namespace Animart.Test
{
    public abstract class AnimartPortalTestBaseClass:AbpIntegratedTestBase
    {
        protected AnimartPortalTestBaseClass()
        {
            //Fake DbConnection using Effort!
            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );

            //Creating initial data
            UsingDbContext(context => new InitialDataBuilder().Build(context));

            AbpSession.TenantId = 1;
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            
        }

        protected void UsingDbContext()a

        
    }
}
