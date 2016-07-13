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
        public Guid Id { get; set; }
        public virtual SupplyItem Item { get; set; }
        public virtual string Name { get; set; }
        public virtual int Price { get; set; }
        public virtual int PriceAdjustment { get; set; }
        public virtual int Quantity { get; set; }
        public virtual int QuantityAdjustment { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

    }
}
