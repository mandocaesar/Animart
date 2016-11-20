using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(OrderItem))]
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        [AutoMapFrom(typeof(SupplyItem))]
        public SupplyItemDto Item { get; set; }

        [AutoMapFrom(typeof(Invoice.Invoice))]
        public InvoiceDto Invoice { get; set; }

        public string Status { get; set; }
        public int Quantity { get; set; }
        public int PriceAdjustment { get; set; }
        public int QuantityAdjustment { get; set;}
    }
}
