using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Order;
using Animart.Portal.Users;

namespace Animart.Portal.Supply
{
    [Table("SupplyItem")]
    public class SupplyItem : CreationAuditedEntity<int, User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(10)]
        public virtual string Code { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }

        [Required]
        public virtual int Price { get; set; }

        public virtual bool Available { get; set; }

        public virtual int InStock { get; set; }

        public virtual decimal Weight { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
