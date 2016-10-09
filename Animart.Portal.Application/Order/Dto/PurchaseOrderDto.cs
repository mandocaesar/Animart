using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Users.Dto;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(PurchaseOrder))]
    public class PurchaseOrderDto:CreationAuditedEntityDto, IEntityDto
    {
        public Guid Id { get; set; }
        
        public string Expedition { get; set; }
        public string ExpeditionAdjustment { get; set; }

        public bool IsPreOrder { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string PostalCode { get; set; }

        public string Status { get; set; }

        public int TotalWeight { get; set; }
        public int KiloQuantity { get; set; }
        public int KiloAdjustmentQuantity { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal ShipmentCostFirstKilo { get; set; }
        public decimal ShipmentAdjustmentCostFirstKilo { get; set; }

        public decimal ShipmentCost { get; set; }
        public decimal ShipmentAdjustmentCost { get; set; }

        public decimal TotalShipmentCost { get; set; }
        public decimal TotalAdjustmentShipmentCost { get; set; }

        public string ReceiptNumber { get; set; }

        public List<OrderItemDto> Items { get; set; }
        public UserDto CreatorUser { get; set; }
       
    }
}
