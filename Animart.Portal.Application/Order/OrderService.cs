﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Animart.Portal.Order.Dto;
using Animart.Portal.Supply;
using AutoMapper;

namespace Animart.Portal.Order
{
    [AbpAuthorize]
    public class OrderService : ApplicationService, IOrderService
    {
        private readonly IRepository<OrderItem, Guid> _orderItemRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly IRepository<SupplyItem, Guid> _supplyItemRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly OrderDomainService _orderDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrderService(IRepository<OrderItem, Guid> orderItemRepository, IRepository<Users.User, long> userRepository, OrderDomainService orderDomainService,
            IUnitOfWorkManager unitOfWorkManager, IRepository<PurchaseOrder, Guid> purchaseOrderRepository, IRepository<SupplyItem, Guid> suppluRepository)
        {
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _orderDomainService = orderDomainService;
            _unitOfWorkManager = unitOfWorkManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplyItemRepository = suppluRepository;

        }


        public OrderDashboardDto GetDashboard()
        {
            var result = new OrderDashboardDto();
            var userId = AbpSession.GetUserId();
            result.BDO = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "BOD");
            result.Delivered = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "Delivered");
            result.Waiting = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "Waiting");

            return result;
        }




        public PurchaseOrder GetSinglePurchaseOrder(string id)
        {
            try
            {
                Guid _id;
                if (Guid.TryParse(id, out _id))
                {
                    return _purchaseOrderRepository.Get(_id);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public bool CheckOrderItem(Dto.OrderItemInputDto orderItem)
        {
            var supplyItem =_supplyItemRepository.GetAllList().First(e => e.Id == orderItem.supplyItem);
            if (supplyItem.InStock < orderItem.Quantity)
            {
                return false;
            }
            return true;
        }

        public bool AddOrderItem(string id, OrderItemInputDto orderItem)
        {
            try
            {

                Guid poId = Guid.Parse(id);
                var supplyItem = _supplyItemRepository.GetAllList().First(e => e.Id == orderItem.supplyItem);
                if (supplyItem.InStock < orderItem.Quantity)
                {
                    return false;
                }

                var item = new OrderItem()
                {
                    Item = supplyItem,
                    PurchaseOrder = _purchaseOrderRepository.GetAllList().First(e => e.Id == poId),
                    Quantity = orderItem.Quantity,
                    CreationTime = DateTime.Now,
                    CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                    CreatorUserId = AbpSession.GetUserId(),
                };

                var _id = _orderItemRepository.InsertOrUpdateAndGetId(item);
                var po = _purchaseOrderRepository.GetAll().First(e => e.Id == poId);

                po.GrandTotal = po.OrderItems.Sum(e => e.Item.Price * e.Quantity);
                po.TotalWeight = po.OrderItems.Sum(e => e.Item.Weight * e.Quantity);
                _purchaseOrderRepository.Update(po);

                supplyItem.InStock -= orderItem.Quantity;
                _supplyItemRepository.Update(supplyItem);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateOrderItem(string id, OrderItemInputDto orderItem)
        {
            try
            {
                Guid _id = Guid.Empty;
                if (Guid.TryParse(id, out _id))
                {
                    var item = _orderItemRepository.Get(_id);
                    item = orderItem.MapTo<OrderItem>();
                    _orderItemRepository.Update(item);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteOrderItem(string id, string orderItemId)
        {
            try
            {
                Guid _id = Guid.Empty;
                if (Guid.TryParse(orderItemId, out _id))
                {
                    _orderItemRepository.Delete(_orderItemRepository.FirstOrDefault(_id));
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Guid> Create(Dto.CreatePurchaseOrderDto purchaseOrderItem)
        {

            _unitOfWorkManager.Begin();
           return await _purchaseOrderRepository.InsertAndGetIdAsync(new PurchaseOrder()
            {
                Address = purchaseOrderItem.Address,
                City = purchaseOrderItem.City.Trim(),
                Expedition = purchaseOrderItem.Expedition.Trim(),
                GrandTotal = purchaseOrderItem.GrandTotal,
                Province = purchaseOrderItem.Province,
                Status = purchaseOrderItem.Status,
                PostalCode = purchaseOrderItem.PostalCode,
                TotalWeight = purchaseOrderItem.TotalWeight,
                CreationTime = DateTime.Now,
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId()
            });
        }

        public bool Update(Dto.OrderItemInputDto orderItem)
        {
            try
            {
                var item = _orderItemRepository.Get(orderItem.Id);
                item.Item = _supplyItemRepository.Get(orderItem.supplyItem);
                item.Quantity = orderItem.Quantity;
                item.PurchaseOrder = _purchaseOrderRepository.Get(orderItem.PurchaseOrder);

                _orderItemRepository.Update(item);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool Delete(string Id)
        {
            try
            {
                Guid result = Guid.Empty;

                if (Guid.TryParse(Id, out result))
                {
                    var item = _orderItemRepository.Get(result);
                    _orderItemRepository.Delete(item);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public IEnumerable<PurchaseOrder> GetAllPurchaseOrderByUserId(int id)
        {
            try
            {
                return _purchaseOrderRepository.GetAllList().Where(e => e.CreatorUserId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<int> UpdateChart()
        {
            var uid = AbpSession.GetUserId();
            var a =  _purchaseOrderRepository.GetAll().Where(e=>e.CreatorUserId == uid)
                .GroupBy(e => e.CreationTime.Month)
                .Select(e => new { Month = e.Key, Count = e.Count() }).ToArray();

            List<int> result = new List<int>();
            for (int i = 0; i < 12; i++)
            {
                result.Add(0);
            }

            foreach (var item in a)
            {
                result[item.Month - 1] = item.Count;
            }

            return result;
        }
    }
}
