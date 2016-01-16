using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;

namespace Animart.Portal.Shipment
{
    [Table("Expedition")]
    public class Expedition : CreationAuditedEntity<int, User>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual ExpeditionType Type { get; set; }

    }
}
