using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;
namespace Animart.Portal.Shipment
{
    [Table("ShipmentCost")]
    public  class ShipmentCost : CreationAuditedEntity<Guid, User>, IEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual City City { get; set; }
        public virtual string Expedition { get; set; }
        public virtual string Type { get; set; }
        public int First5Kilo { get; set; }
        public int NextKilo { get; set; }
    }
}
