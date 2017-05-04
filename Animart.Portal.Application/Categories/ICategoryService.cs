using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Web.Models;
using Animart.Portal.Categories.Dto;

namespace Animart.Portal.Categories
{
    public interface ICategoryService:IApplicationService
    {
        [DontWrapResult]
        List<CategoryDto> GetCategories();
        [DontWrapResult]
        List<CategoryDto> GetCategoriesForFilter();
        [DontWrapResult]
        Task Create(CategoryDto categoryItem);
        [DontWrapResult]
        bool Update(CategoryDto categoryItem);
        [DontWrapResult]
        bool Delete(Guid id);
        [DontWrapResult]
        CategoryDto Category(Guid id);

    }
}
