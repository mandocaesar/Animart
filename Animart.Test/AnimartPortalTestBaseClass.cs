using System;
using System.Data.Common;
using System.Linq;
using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;
using Animart.Portal;
using Castle.MicroKernel.Registration;
using Animart.Portal.EntityFramework;
using Animart.Portal.Migrations.Data;

namespace Animart.Test
{
    public abstract class AnimartPortalTestBaseClass : AbpIntegratedTestBase
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
        //   UsingDbContext(context => new InitialDataBuilder().Build(context));

            AbpSession.TenantId = 1;
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<PortalCoreModule>();
            modules.Add<PortalApplicationModule>();
            modules.Add<PortalDataModule>();

        }

        protected void UsingDbContext(Action<PortalDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<PortalDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        public T UsingDbContext<T>(Func<PortalDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<PortalDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }
            return result;
        }


    }
}
