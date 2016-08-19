using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Animart.Portal.Categories.Dto;
using Animart.Portal.Supply;

namespace Animart.Portal.Categories
{
    [AbpAuthorize]
    public class CategoryService : PortalAppServiceBase, ICategoryService
    {
        private readonly IRepository<Category, Guid> _categoryRepository;
        
        public CategoryService(IRepository<Category, Guid> categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }
        
        public List<CategoryDto> GetCategories()
        {
            var categories = _categoryRepository.GetAll().OrderBy(i=>i.Name).ToList();

            return categories.Select(e => new CategoryDto
            {
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                Name = e.Name,
                ParentId = e.ParentId,
                ParentName = (e.Parent!=null)?(e.Parent.Name):("")
            }).ToList();
        }

        public List<CategoryDto> GetCategoriesForFilter()
        {
            var categories = _categoryRepository.GetAll()
                .OrderBy(i => i.Parent.Parent.Parent.Parent.Name)
                .ThenBy(i => i.Parent.Parent.Parent.Name)
                .ThenBy(i => i.Parent.Parent.Name).
                ThenBy(i => i.Parent.Name).ThenBy(i => i.Name).ToList();
            categories.Add(new Category {Parent=null,ParentId=null,Name="- All Categories -"});
            //categories = categories.OrderBy(i=>i.Parent.Parent.Name).
            //    ThenBy(i => i.Parent.Name).ThenBy(i=>i.Name).ToList();

            return categories.Select(e => new CategoryDto
            {
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                Name = this.ToFilter(e),
                ParentId = e.ParentId,
                ParentName = (e.Parent != null) ? (e.Parent.Name) : ("")
            }).OrderBy(i=>i.Name).ToList();
        }

        private string ToFilter(Category cat)
        {
            var name = cat.Name;
            var parent = cat.Parent;
            while (parent != null)
            {
                name = parent.Name + " > " + name;
                parent = parent.Parent;
            }
            return name;
        }

        public async Task Create(CategoryDto categoryItem)
        {
            try
            {
                await _categoryRepository.InsertAsync(new Category()
                {
                    Name = categoryItem.Name,
                    ParentId = categoryItem.ParentId
                });
            }
            catch (Exception ex)
            {
                    
                throw ex;
            }
  
        }

        public bool Update(CategoryDto categoryItem)
        {
            try
            {
                var item = _categoryRepository.Single(e => e.Id == categoryItem.Id);

                item.Name = categoryItem.Name ?? item.Name;
                item.ParentId = categoryItem.ParentId;
                _categoryRepository.Update(item);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var item = _categoryRepository.Single(e => e.Id == id);
                _categoryRepository.Delete(item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CategoryDto Category(Guid id)
        {
            try
            {
                var a = _categoryRepository.FirstOrDefault(e => e.Id == id);
                return new CategoryDto()
                {
                    CreationTime = a.CreationTime,
                    CreatorUserId = a.CreatorUserId,
                    Id = a.Id,
                    Name = a.Name,
                    ParentId = a.ParentId,
                    ParentName = (a.Parent != null) ? (a.Parent.Name) : ("")
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
