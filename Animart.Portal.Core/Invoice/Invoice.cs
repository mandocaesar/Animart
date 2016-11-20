using System;
using System.Collections.Generic;
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

        public virtual string Expedition { get; set; }

        public virtual string ResiNumber { get; set; }
        public virtual decimal TotalWeight { get; set; }
        public virtual decimal GrandTotal { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }


    }
}
