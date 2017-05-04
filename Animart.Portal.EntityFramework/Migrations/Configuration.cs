using System.Data.Entity.Migrations;
using System;
using System.Linq;
using Abp.Extensions;
using Animart.Portal.Migrations.Data;
using Microsoft.AspNet.Identity;

namespace Animart.Portal.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Portal.EntityFramework.PortalDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Portal";
        }

        protected override void Seed(Portal.EntityFramework.PortalDbContext context)
        {
            try
            {
                new InitialDataBuilder().Build(context);
            }
            catch (Exception e)
            {

                Console.Write(e);
            }
       
        }
    }
}
