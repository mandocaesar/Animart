﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Order;
using Animart.Portal.Users;

namespace Animart.Portal.Invoice
{
    public class Invoice: CreationAuditedEntity<int,User>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual PurchaseOrder PurchaseOrder {get;set;}

        public Invoice()
        {
        }

        public virtual decimal Total()
        {
            var total = 0;
            foreach (var po in PurchaseOrder.OrderItems)
            {
                total = po.Quantity * po.Item.Price;
            }
            return total;
        }

    }
}