using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;

namespace Animart.Portal.Supply
{
    [Table("SupplyImage")]
    public class SupplyImage : CreationAuditedEntity<int, User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Guid Id { get; set; }
        public virtual Guid SupplyItemId { get; set; }
    }
}
