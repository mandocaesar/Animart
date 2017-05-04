using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Supply;
using Animart.Portal.Users;

namespace Animart.Portal.Offer
{
    [Table("OfferDetail")]
    public class OfferDetail : CreationAuditedEntity<int, User>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public SupplyItem item { get; set; }
        public virtual Portal.Offer.Offer Offer { get; set; }
    }
}
