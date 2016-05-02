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
            //OrderItems = new List<OrderItem>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  Guid Id { get; set; }

        public  string Expedition { get; set; }

        public  string Province { get; set; }

        public  string City { get; set; }

        [MaxLength(5)]
        public  string PostalCode { get; set; }

        public  string Address { get; set; }

        public  string Status { get; set; }

        public  decimal TotalWeight { get; set; }

        public  decimal GrandTotal { get; set; }

        public  ICollection<OrderItem> OrderItems { get; set; }

        public string ReceiptNumber { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public User ModifiedBy { get; set; }
    }
}
