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
  //  [AbpAuthorize]
    public class SupplyService : ApplicationService ,ISupplyService
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
             await _supplyItemRepository.InsertAsync(new SupplyItem()
            {
                Available = supplyItem.Available,
                Code = supplyItem.Code,
                InStock = supplyItem.InStock,
                Name = supplyItem.Name,
                Price = supplyItem.Price,
                CreationTime = DateTime.Now,
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId()

            });
            
        }

        public bool Update(SupplyItemDto supplyItem)
        {
            try
            {
                var item = _supplyItemRepository.Single(e => e.Id == supplyItem.Id);
                //  item = (SupplyItem)supplyItem;
                item.Name = supplyItem.Name;
                item.InStock = supplyItem.InStock;
                item.Price = supplyItem.Price;
                item.Code = supplyItem.Code;
                item.Available = supplyItem.Available;

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

        public SupplyItem GetSupplyItemById(Guid id)
        {
            try
            {
                return _supplyItemRepository.FirstOrDefault(e => e.Id == id);
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
