﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Animart.Portal.Supply.Dto;
using Animart.Portal.Users;
using AutoMapper;

namespace Animart.Portal.Supply
{
    [AbpAuthorize]
    public class SupplyService : PortalAppServiceBase, ISupplyService
    {
        private readonly IRepository<SupplyItem, Guid> _supplyItemRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly SupplyDomainService _supplyDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SupplyService(IRepository<SupplyItem, Guid> supplyItemRepository,
            IRepository<Category, Guid> categoryRepository,
            IRepository<Users.User, long> userRepository, SupplyDomainService supplyDomainService, IUnitOfWorkManager unitOfWorkManager)
        {

            _categoryRepository = categoryRepository;
            _supplyItemRepository = supplyItemRepository;
            _userRepository = userRepository;
            _supplyDomainService = supplyDomainService;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public ItemAvailableDto IsAvailable(Guid id,int idx)
        {
            var supplies = _supplyItemRepository.GetAll().FirstOrDefault(i=>i.Id==id);
            if (supplies == null)
                return new ItemAvailableDto {Idx = idx, IsAvailable = false,Date = DateTime.UtcNow };
            if(supplies.IsPo)
                return new ItemAvailableDto { Idx = idx, IsAvailable = (supplies.AvailableUntil > DateTime.UtcNow), Date = DateTime.UtcNow };

            return new ItemAvailableDto { Idx = idx, IsAvailable = true, Date = DateTime.UtcNow };
        }

        public string CategoryToName(SupplyItem item)
        {
            if (item.CategoryId != null)
            {
                var data = _categoryRepository.FirstOrDefault(i => i.Id == item.CategoryId);
                return data.Name;
            }
            return "";
        }

        public List<SupplyItemDto> GetSupplies()
        {
            var supplies = _supplyItemRepository.GetAll().ToList();

            return supplies.Select(e => new SupplyItemDto
            {
                Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                IsPO = e.IsPo,
                AvailableUntil = e.AvailableUntil,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();
        }

        public List<SupplyItemDto> GetSuppliesByCategoryId(Guid id)
        {
            var supplies = _supplyItemRepository.GetAll().Where(i=>i.CategoryId==id).ToList();

            return supplies.Select(e => new SupplyItemDto
            {
                Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                IsPO = e.IsPo,
                AvailableUntil = e.AvailableUntil,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();
        }

        private bool CategoryWillBeShown(SupplyItem s)
        {
                if (s.CategoryId == null)
                    return true;
                else
                {
                    return _categoryRepository.GetAll().FirstOrDefault(e=>e.Id==s.CategoryId).IsAvailable;
                }
        }

        public SuppliesDTO GetSuppliesRetailer(bool latest)
        {
            var result = new SuppliesDTO();
            var supplies = _supplyItemRepository.GetAll().Where(e=>e.Available ).ToList();
            
            result.Supply = supplies.Where(e=>!e.IsPo && CategoryWillBeShown(e))
                .OrderByDescending(i => i.CreationTime).Select(e=> new SupplyItemDto
            {   Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                IsPO = e.IsPo,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();

           
            result.PoSupply = supplies.Where(e => e.IsPo && CategoryWillBeShown(e)
                && e.AvailableUntil>= DateTime.UtcNow)
                .OrderByDescending(i => i.CreationTime).Select(e => new SupplyItemDto
            {
                Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                AvailableUntil = e.AvailableUntil,
                IsPO = e.IsPo,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();
            if (!latest)
                result.PoSupply = result.PoSupply.OrderBy(i => i.AvailableUntil).ToList();


            return result;
        }
        
        public SuppliesDTO GetSuppliesRetailerByCategoryId(Guid id,bool latest)
        {
            var result = new SuppliesDTO();
            var supplies = _supplyItemRepository.GetAll().Where(e => e.Available && e.CategoryId==id).ToList();

            result.Supply = supplies.Where(e => !e.IsPo).Select(e => new SupplyItemDto
            {
                Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                IsPO = e.IsPo,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();

            result.PoSupply = supplies.Where(e => e.IsPo && e.AvailableUntil >= DateTime.UtcNow).Select(e => new SupplyItemDto
            {
                Available = e.Available,
                Code = e.Code,
                Id = e.Id,
                CreationTime = e.CreationTime,
                CreatorUserId = e.CreatorUserId,
                InStock = e.InStock,
                Name = e.Name,
                Price = e.Price,
                Weight = e.Weight,
                Description = e.Description,
                HasImage = e.HasImage,
                AvailableUntil = e.AvailableUntil,
                IsPO = e.IsPo,
                CategoryId = e.CategoryId,
                Category = this.CategoryToName(e)
            }).ToList();

            if (!latest)
                result.PoSupply = result.PoSupply.OrderBy(i => i.AvailableUntil).ToList();

            return result;
        }

        public PagedResultOutput<SupplyItem> GetSupplyByName(GetSupplyByNameInput input)
        {
            if (input.MaxResultCount <= 0)
            {
                input.MaxResultCount = 10;
            }

            var supplyCount = _supplyItemRepository.Count(e=>e.Name.Contains(input.Name));
            var supplies =
                _supplyItemRepository
                    .GetAll().Where(e=>e.Name.Contains(input.Name))
                    .Include(q => q.CreatorUser)
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();

            return new PagedResultOutput<SupplyItem>
            {
                TotalCount = supplies.Count,
                Items = supplies
            };
        }

        public async Task Create(SupplyItemDto supplyItem)
        {
            try
            {
                if (supplyItem.AvailableUntil < new DateTime(1990, 1, 1))
                    supplyItem.AvailableUntil = DateTime.Now;
                var data3 = supplyItem.AvailableUntil;
                var data = supplyItem.AvailableUntil.ToUniversalTime();
                var data2 = supplyItem.AvailableUntil.ToLocalTime();
              
                var supp = new SupplyItem()
                {
                    Available = supplyItem.Available,
                    Code = supplyItem.Code,
                    InStock = supplyItem.InStock,
                    Name = supplyItem.Name,
                    Price = supplyItem.Price,
                    CreationTime = DateTime.Now,
                    CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                    CreatorUserId = AbpSession.GetUserId(),
                    Weight = supplyItem.Weight,
                    Description = supplyItem.Description,
                    IsPo = supplyItem.IsPO,
                    CategoryId = supplyItem.CategoryId,
                    AvailableUntil = supplyItem.AvailableUntil
                };
               var x =  await _supplyItemRepository.InsertAndGetIdAsync(supp);
                var b = x;
            }
            catch (Exception ex)
            {
                    
                throw ex;
            }
  
        }

        public bool Update(SupplyItemDto supplyItem)
        {
            try
            {
                var item = _supplyItemRepository.Single(e => e.Id == supplyItem.Id);

                var x = supplyItem.AvailableUntil;
                var data = supplyItem.AvailableUntil.ToUniversalTime();
                var data2 = supplyItem.AvailableUntil.ToLocalTime();
                var data3 = item.AvailableUntil.ToUniversalTime();
                var data4 = item.AvailableUntil.ToLocalTime();
                item.Name = supplyItem.Name ?? item.Name;
                item.InStock = supplyItem.InStock;
                item.Price = supplyItem.Price;
                item.Code = supplyItem.Code ?? item.Code;
                item.Available = supplyItem.Available;
                item.Weight = supplyItem.Weight;
                item.Description = supplyItem.Description ?? item.Description;
                item.HasImage = supplyItem.HasImage;
                item.IsPo = supplyItem.IsPO;
                item.CategoryId = supplyItem.CategoryId;
                item.AvailableUntil = supplyItem.AvailableUntil;

                _supplyItemRepository.Update(item);
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
                var item = _supplyItemRepository.Single(e => e.Id == id);
            _supplyItemRepository.Delete(item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveImage(Guid supplyItemId)
        {
            try
            {
                _supplyDomainService.SupplyImageRepository.Insert(new SupplyImage
                {
                    SupplyItemId = supplyItemId,
                    CreationTime = DateTime.Now,
                    CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                    CreatorUserId = AbpSession.GetUserId()
                });

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStatus(Guid id)
        {
            try
            {
                var item = _supplyItemRepository.Single(e => e.Id == id);
                item.Available = !item.Available;
                _supplyItemRepository.Update(item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateInStock(Guid id, int stock)
        {
            try
            {
                var item = _supplyItemRepository.Single(e => e.Id == id);
                item.InStock = stock;
                _supplyItemRepository.Update(item);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public SupplyItemDto Supply (Guid id)
        {
            try
            {
                var a = _supplyItemRepository.FirstOrDefault(e => e.Id == id);
                if (a.CategoryId != null)
                {
                    var data = _categoryRepository.FirstOrDefault(e => e.Id == a.CategoryId).IsAvailable;
                    a.Available = a.Available && data;
                }
                var data3 = a.AvailableUntil.ToLocalTime();
                var data2 = a.AvailableUntil.ToUniversalTime();
                return new SupplyItemDto()
                {
                    Available = a.Available,
                    Code = a.Code,
                    CreationTime = a.CreationTime,
                    CreatorUserId = a.CreatorUserId,
                    Description = a.Description,
                    HasImage = a.HasImage,
                    Id = a.Id,
                    InStock = a.InStock,
                    IsPO = a.IsPo,
                    Name = a.Name,
                    Price = a.Price,
                    Weight = a.Weight,
                    AvailableUntil = a.AvailableUntil,
                    Category = this.CategoryToName(a),
                    CategoryId = a.CategoryId
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SupplyItem GetSingleByName(string name)
        {
            return _supplyItemRepository.FirstOrDefault(e => e.Name.ToLower().Contains(name.ToLower()));
        }
    }
}
