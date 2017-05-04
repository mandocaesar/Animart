using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Animart.Portal.Users;

namespace Animart.Portal.Offer
{
    [Table("Offer")]
    public class Offer: CreationAuditedEntity<int, User>
    {
        public Offer()
        {
            OfferDetails = new List<OfferDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<OfferDetail> OfferDetails{ get; set; }
    }
}
