using System.Data.Entity.Migrations;
using System;
using System.Linq;
using Abp.Extensions;
using Microsoft.AspNet.Identity;

namespace Animart.Portal.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Portal.EntityFramework.PortalDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Portal";
        }

        protected override void Seed(Portal.EntityFramework.PortalDbContext context)
        {
            try
            {
                // This method will be called every time after migrating to the latest version.
                // You can add any seed data here...
                //if (context.Roles.Count(e => e.Name == "Admin") < 1)
                //{
                //context.Roles.Add(new Model.Role()
                //{
                //    Name = "Admin",
                //    CreatedOn = DateTime.Now.Date,
                //    CreatedBy = "Admin",
                //});

                //context.SaveChanges();
                //}

                //if (context.Users.Count(e => e.UserName == "Admin") < 1)
                ////{
                //var role = context.Roles.First(e => e.Name == "Admin");
                //  var password = new PasswordHasher().HashPassword("ZXasqw12");
                //context.Users.Add(new Model.User()
                //{
                //    FirstName = "Admin",
                //    LastName = "Admin",
                //    role = role,
                //    CreatedOn = DateTime.Now.Date,
                //    CreatedBy = "Admin",
                //    UserName = "Admin",
                //    Password = password
                //});
                //context.SaveChanges();
                //}
            }
            catch (Exception e)
            {

                Console.Write(e);
            }
       
        }
    }
}
