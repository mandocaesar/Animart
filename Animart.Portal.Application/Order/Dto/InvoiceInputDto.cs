using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(OrderItem))]
    public class InvoiceInputDto : CreationAuditedEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PriceAdjustment { get; set; }
        public int Quantity { get; set; }
        public int QuantityAdjustment { get; set; }
        public string Status { get; set; }
        public bool Checked { get; set; }
        public SupplyItemDto Item { get; set; }
    }
}
