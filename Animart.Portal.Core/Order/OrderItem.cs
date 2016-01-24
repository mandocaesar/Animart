using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Supply;
using Animart.Portal.Users;

namespace Animart.Portal.Order
{
    public class OrderItem : CreationAuditedEntity<int, User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }
        public virtual SupplyItem Item { get; set; }
        public virtual int Quantity { get; set; }
        public virtual PurchaseOrder PurchaseOrder {get;set;}

    }
}
