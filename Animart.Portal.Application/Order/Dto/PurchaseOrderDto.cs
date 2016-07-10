using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaseOrderDto:CreationAuditedEntityDto, IEntityDto
    {
        public Guid Id { get; set; }
        
        public string Expedition { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string Status { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal GrandTotal { get; set; }
        public decimal ShipmentCost { get; set; }
        public decimal TotalShipmentCost { get; set; }

        public string ReceiptNumber { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }
}
