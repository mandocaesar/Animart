using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Supply;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(OrderItem))]
    public class OrderItemInputDto:CreationAuditedEntityDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrder { get; set; }
        public Guid SupplyItem { get; set; }
        public string Name { get; set; }
        public int PriceAdjusment { get; set; }
        public int Quantity { get; set; }
    }
}
