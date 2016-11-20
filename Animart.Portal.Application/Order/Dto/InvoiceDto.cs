using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;

namespace Animart.Portal.Order.Dto
{
    [AutoMapFrom(typeof(Invoice.Invoice))]
    public class InvoiceDto : CreationAuditedEntityDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string ResiNumber { get; set; }
        public string Expedition { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
