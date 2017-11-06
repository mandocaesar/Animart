using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Animart.Portal.Supply.Dto
{
    [AutoMapFrom(typeof(SupplyItem))]
    public class SupplyItemDto: CreationAuditedEntityDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public bool Available { get; set; }

        public int InStock { get; set; }

        public bool HasImage { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }

        public bool IsPO { get; set; }

        public string Category { get; set; }

        public Guid? CategoryId { get; set; }
        //[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTimeOffset AvailableUntil { get; set; }
    }
    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
            //base.DateTimeStyles = System.Globalization.DateTimeStyles.AdjustToUniversal;
        }
    }
}
