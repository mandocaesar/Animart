using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Animart.Portal.Supply.Dto
{
    public class ItemAvailableDto
    {
        public int Idx { get; set; }
        public bool IsAvailable { get; set; }
    }
}
