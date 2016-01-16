﻿using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Animart.Portal.Authorization;
using Animart.Portal.Order;
using Animart.Portal.Supply;
using Animart.Portal.Users;
using Animart.Portal.Invoice;
using Animart.Portal.MultiTenancy;

namespace Animart.Portal.EntityFramework
{
    public class PortalDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for each Entity...

        //Example:
        public virtual IDbSet<SupplyItem> SupplyItems { get; set; }
        public virtual IDbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual IDbSet<OrderItem> OrderItems { get; set; }

        public virtual IDbSet<Invoice.Invoice> Invoices { get; set; }


        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public PortalDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in PortalDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of PortalDbContext since ABP automatically handles it.
         */
        public PortalDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}