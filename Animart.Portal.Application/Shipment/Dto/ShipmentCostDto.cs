using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Animart.Portal.Shipment.Dto
{
    [AutoMapFrom(typeof(ShipmentCost))]
    public class ShipmentCostDto: CreationAuditedEntityDto, IEntityDto
    {
        public Guid Id { get; set; }

        public string City { get; set; }
        public string Expedition { get; set; }
        public string Type { get; set; }
        public int First5Kilo { get; set; }
        public int NextKilo { get; set; }

    }
}
