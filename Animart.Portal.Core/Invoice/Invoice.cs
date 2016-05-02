using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Order;
using Animart.Portal.Users;

namespace Animart.Portal.Invoice
{
    public class Invoice: CreationAuditedEntity<int,User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        public virtual string InvoiceNumber { get; set; }

        public virtual PurchaseOrder PurchaseOrder {get;set;}

        public virtual string ResiNumber { get; set; }

    }
}
