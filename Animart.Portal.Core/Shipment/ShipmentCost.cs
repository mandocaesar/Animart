using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;
namespace Animart.Portal.Shipment
{
    [Table("ShipmentCost")]
    public  class ShipmentCost : CreationAuditedEntity<int, User>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual City City { get; set; }
        public virtual Expedition Expedition { get; set; }
        public virtual ExpeditionType Type { get; set; }
    }
}
