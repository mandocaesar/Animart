using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Order;
using Animart.Portal.Users;

namespace Animart.Portal.Supply
{
    [Table("Category")]
    public class Category : CreationAuditedEntity<int, User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public virtual string Name { get; set; }

        public virtual bool IsAvailable { get; set; }

        public Guid? ParentId { get; set; }
        public Category Parent { get; set; }
        public ICollection<SupplyItem> SupplyItems { get; set; }
    }
}
