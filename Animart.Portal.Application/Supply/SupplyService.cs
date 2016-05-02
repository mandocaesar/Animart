using System;
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
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly SupplyDomainService _supplyDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public SupplyService(IRepository<SupplyItem, Guid> supplyItemRepository, IRepository<Users.User, long> userRepository, SupplyDomainService supplyDomainService, IUnitOfWorkManager unitOfWorkManager)
        {
           
            _supplyItemRepository = supplyItemRepository;
            _userRepository = userRepository;
            _supplyDomainService = supplyDomainService;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public List<SupplyItemDto> GetSupplies()
        {
            var supplies = _supplyItemRepository.GetAll().Where(e => e.Available).ToList();

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
                AvailableUntil = e.AvailableUntil
            }).ToList();
        }


        public SuppliesDTO GetSuppliesRetailer()
        {
            var result = new SuppliesDTO();
            var supplies = _supplyItemRepository.GetAll().Where(e=>e.Available).ToList();

            result.Supply = supplies.Where(e=>!e.IsPo).Select(e=> new SupplyItemDto
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
                HasImage = e.HasImage
            }).ToList();

            result.PoSupply = supplies.Where(e => e.IsPo && e.AvailableUntil.Date >= DateTime.Now.Date).Select(e => new SupplyItemDto
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
                AvailableUntil = e.AvailableUntil
            }).ToList();

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
                await _supplyItemRepository.InsertAsync(new SupplyItem()
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
                    AvailableUntil = supplyItem.AvailableUntil == DateTime.MinValue ? DateTime.Now : supplyItem.AvailableUntil
                });
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

                item.Name = supplyItem.Name;
                item.InStock = supplyItem.InStock;
                item.Price = supplyItem.Price;
                item.Code = supplyItem.Code;
                item.Available = supplyItem.Available;
                item.Weight = supplyItem.Weight;
                item.Description = supplyItem.Description;
                item.HasImage = supplyItem.HasImage;
                item.IsPo = supplyItem.IsPO;
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
                    Weight = a.Weight
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
