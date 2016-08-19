using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace Animart.Portal.Categories.Dto
{
    [AutoMapFrom(typeof(Supply.Category))]
    public class CategoryDto: CreationAuditedEntityDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string ParentName { get; set; }

        public Guid? ParentId { get; set; }
        
    }
}
