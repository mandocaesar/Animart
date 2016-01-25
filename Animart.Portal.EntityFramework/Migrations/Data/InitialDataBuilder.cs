using System;
using System.Linq;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Animart.Portal.Authorization;
using Animart.Portal.EntityFramework;
using Animart.Portal.MultiTenancy;
using Animart.Portal.Users;
using EntityFramework.DynamicFilters;

namespace Animart.Portal.Migrations.Data
{
    public class InitialDataBuilder
    {
        

        public void Build(PortalDbContext context)
        {
            context.DisableAllFilters();
           CreateUserAndRoles(context);
        }

        public void CreateDummySupplyItem(PortalDbContext context)
        {
            context.SupplyItems.Add(new Supply.SupplyItem()
            {
                Available = true,
                Code = "Test-123",
                InStock = 20,
                Name = "Gundamdam",
                Price = 20000,
                CreatorUser = context.Users.First(e=>e.Id == 1),
                CreatorUserId = 1,
                Weigth = 10,
                CreationTime = DateTime.Now 
            });
        }
        public void CreateUserAndRoles(PortalDbContext context)
        {
            
            //var adminRoleForTenancyOwner = context.Roles.FirstOrDefault(r => r.TenantId == null && r.Name == "Admin");
            //if (adminRoleForTenancyOwner == null)
            //{
            //    adminRoleForTenancyOwner = context.Roles.Add(new Role(null, "Admin", "Admin"));
            //    context.SaveChanges();
            //}

            //var adminUserForTenancyOwner = context.Users.FirstOrDefault(u => u.TenantId == null && u.UserName == "admin");


            //if (adminUserForTenancyOwner == null)
            //{
            //    adminUserForTenancyOwner = context.Users.Add(
            //        new User
            //        {
            //            TenantId = null,
            //            UserName = "admin",
            //            Name = "System",
            //            Surname = "Administrator",
            //            EmailAddress = "admin@animart.com",
            //            IsEmailConfirmed = true,
            //            Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            //        });

            //    context.SaveChanges();

            //    context.UserRoles.Add(new UserRole(adminUserForTenancyOwner.Id, adminRoleForTenancyOwner.Id));

            //    context.SaveChanges();
            //}

            //Default tenant

            var defaultTenant = context.Tenants.FirstOrDefault(t => t.TenancyName == "Default");
            if (defaultTenant == null)
            {
                defaultTenant = context.Tenants.Add(new Tenant("Default", "Default"));
                context.SaveChanges();
            }

            //Admin role for 'Default' tenant

            var adminRoleForDefaultTenant = context.Roles.FirstOrDefault(r => r.TenantId == defaultTenant.Id && r.Name == "Admin");
            if (adminRoleForDefaultTenant == null)
            {
                adminRoleForDefaultTenant = context.Roles.Add(new Role(defaultTenant.Id, "Admin", "Admin"));
                context.SaveChanges();

                //Permission definitions for Admin of 'Default' tenant
                //context.Permissions.Add(new RolePermissionSetting { RoleId = adminRoleForDefaultTenant.Id, Name = "CanDeleteAnswers", IsGranted = true });
                //context.Permissions.Add(new RolePermissionSetting { RoleId = adminRoleForDefaultTenant.Id, Name = "CanDeleteQuestions", IsGranted = true });
                context.SaveChanges();
            }

            //User role for 'Default' tenant

            var userRoleForDefaultTenant = context.Roles.FirstOrDefault(r => r.TenantId == defaultTenant.Id && r.Name == "User");
            if (userRoleForDefaultTenant == null)
            {
                context.Roles.Add(new Role(null, "Logistic", "Logistic"));
                context.Roles.Add(new Role(null, "Accounting", "Accounting"));
                context.Roles.Add(new Role(null, "BOD", "BOD"));
                userRoleForDefaultTenant = context.Roles.Add(new Role(defaultTenant.Id, "Retailer", "Retailer"));
                context.SaveChanges();

                //Permission definitions for User of 'Default' tenant
               // context.Permissions.Add(new RolePermissionSetting { RoleId = userRoleForDefaultTenant.Id, Name = "CanCreateQuestions", IsGranted = true });
                context.SaveChanges();
            }

            //Admin for 'Default' tenant

            var adminUserForDefaultTenant = context.Users.FirstOrDefault(u => u.TenantId == defaultTenant.Id && u.UserName == "admin");
            if (adminUserForDefaultTenant == null)
            {
                adminUserForDefaultTenant = context.Users.Add(
                    new User
                    {
                        TenantId = defaultTenant.Id,
                        UserName = "admin",
                        Name = "System",
                        Surname = "Administrator",
                        EmailAddress = "admin@animart.com",
                        IsEmailConfirmed = true,
                        Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
                    });
                context.SaveChanges();

               // context.UserRoles.Add(new UserRole(adminUserForDefaultTenant.Id, adminRoleForDefaultTenant.Id));
               // context.UserRoles.Add(new UserRole(adminUserForDefaultTenant.Id, userRoleForDefaultTenant.Id));
                context.SaveChanges();
            }


        }

    }
}
