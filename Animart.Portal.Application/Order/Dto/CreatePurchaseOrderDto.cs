using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;


namespace Animart.Portal.Order.Dto
{
    public class CreatePurchaseOrderDto :IInputDto
    {
        public string Expedition { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Status { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal GrandTotal { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
