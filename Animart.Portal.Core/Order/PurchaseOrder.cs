using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;

namespace Animart.Portal.Order
{
    [Table("PurchaseOrder")]
    public class PurchaseOrder : CreationAuditedEntity<int, User>,IEntity<Guid>
    {
        public PurchaseOrder()
        {
            OrderItems = new List<OrderItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        public virtual string Expedition { get; set; }

        public virtual string Province { get; set; }

        public virtual string City { get; set; }

        public virtual string Address { get; set; }

        public virtual string Status { get; set; }

        public virtual decimal TotalWeight { get; set; }

        public virtual decimal GrandTotal { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
