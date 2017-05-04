using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Animart.Portal.Shipment.Dto
{
    [AutoMapFrom(typeof(City))]
    public class CityDto: CreationAuditedEntityDto, IEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
