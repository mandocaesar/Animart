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
using Animart.Portal.Extension;
using Animart.Portal.Order.Dto;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;
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
            result.Delivered = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "LOGISTIC");
            result.Waiting = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "ACCOUNTING");

            return result;
        }


        public OrderDashboardDto GetDashboardAdmin()
        {
            var result = new OrderDashboardDto();
            var userId = AbpSession.GetUserId();
            result.BDO = _purchaseOrderRepository.Count(e => e.Status == "BOD");
            result.Delivered = _purchaseOrderRepository.Count(e => e.Status == "LOGISTIC");
            result.Waiting = _purchaseOrderRepository.Count(e => e.Status == "ACCOUNTING");

            return result;
        }

        public PurchaseOrderDto GetSinglePurchaseOrder(string id)
        {
            try
            {
                Guid _id;
                if (Guid.TryParse(id, out _id))
                {
                    _id = Guid.Parse(id);

                    var result = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == _id).MapTo<PurchaseOrderDto>();
                  //  result.OrderItems = new List<OrderItem>();
                    var orderItems = _orderItemRepository.GetAll().Where(e => e.PurchaseOrder.Id == result.Id).ToList();
                    result.Items = orderItems.Select(e=>e.MapTo<OrderItemDto>()).ToList();

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool CheckOrderItem(Dto.OrderItemInputDto orderItem)
        {
            var supplyItem = _supplyItemRepository.GetAllList().First(e => e.Id == orderItem.SupplyItem);
            if (supplyItem.InStock < orderItem.Quantity)
            {
                return false;
            }
            return true;
        }

        public bool AddOrderItem(string id, List<OrderItemInputDto> orderItems)
        {
            try
            {
                foreach (var orderItem in orderItems)
                {
                    Guid poId = Guid.Parse(id);
                    var supplyItem = _supplyItemRepository.GetAllList().First(e => e.Id == orderItem.SupplyItem);
                    if (supplyItem.InStock < orderItem.Quantity)
                    {
                        return false;
                    }

                    if (orderItem.Quantity == 0)
                    {
                        return true;
                    }

                    var item = new OrderItem()
                    {
                        Item = supplyItem,
                        PurchaseOrder = _purchaseOrderRepository.GetAllList().First(e => e.Id == poId),
                        Quantity = orderItem.Quantity,
                        Name = orderItem.Name,
                        PriceAdjustment = 0,
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
                }
               

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

        public bool Update(string id , Dto.OrderItemInputDto orderItem)
        {
            try
            {
                var item = _orderItemRepository.GetAll().FirstOrDefault(e=>e.Id == orderItem.Id);
                item.Item = _supplyItemRepository.GetAll().FirstOrDefault(e=>e.Id == orderItem.SupplyItem);
                item.PriceAdjustment = orderItem.PriceAdjusment;
                item.Quantity = orderItem.Quantity;

                var poid = Guid.Parse(id);
                item.PurchaseOrder = _purchaseOrderRepository.GetAll().FirstOrDefault(e=>e.Id == poid);

                item.PurchaseOrder.ModifiedBy = _userRepository.Get(AbpSession.GetUserId());
                item.PurchaseOrder.ModifiedOn = DateTime.Now;

                var total = 0;
                var items = item.PurchaseOrder.OrderItems.ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    var price = items[i].PriceAdjustment == 0 ? items[i].Item.Price : items[i].PriceAdjustment;
                    total += price*items[i].Quantity;
                }

                item.PurchaseOrder.GrandTotal = total;

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

        public List<PurchaseOrderDto> GetAllPurchaseOrderByUserId()
        {
            try
            {
                var uid = AbpSession.GetUserId();
                var list = _purchaseOrderRepository.GetAll().Where(e => e.CreatorUserId == uid).ToList();
                var a = list.Select(item => item.MapTo<PurchaseOrderDto>()).ToList();
                return a;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<int> UpdateChart()
        {
            var uid = AbpSession.GetUserId();
            var a = _purchaseOrderRepository.GetAll().Where(e => e.CreatorUserId == uid)
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

        public bool UpdatePurchaseOrderStatus(string id, string status)
        {
            try
            {
                var POid = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e=>e.Id == POid);
                
                po.Status = status;
                GmailExtension gmail = new GmailExtension("marketing@animart.co.id","GOSALES2015");
                gmail.SendMessage("Purchase Order " + POid + " Has been updated"," Hello, your purchase order with id " + POid + "has been updated to " + status +" please login to have look on it ",
                    po.CreatorUser.EmailAddress);
                _purchaseOrderRepository.Update(po);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForMarketing()
        {
            try
            {
                var list = _purchaseOrderRepository.GetAll().Where(e => e.Status == "MARKETING").ToList();
                var result = list.Select(item => item.MapTo<PurchaseOrderDto>()).ToList();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForAccounting()
        {
            try
            {
                var list = _purchaseOrderRepository.GetAll().Where(e => e.Status == "ACCOUNTING").ToList();
                var result = list.Select(item => item.MapTo<PurchaseOrderDto>()).ToList();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForLogistic()
        {
            try
            {
                var list = _purchaseOrderRepository.GetAll().Where(e => e.Status == "LOGISTIC").ToList();
                var result = list.Select(item => item.MapTo<PurchaseOrderDto>()).ToList();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool InsertReceiptNumber(string id, string receipt)
        {
            try
            {
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == Guid.Parse(id));
                po.ReceiptNumber = receipt;
                po.Status = "Delivered";
                _purchaseOrderRepository.Update(po);
                GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                gmail.SendMessage("Purchase Order " + po.Id.ToString() + " Has been updated", " Hello, your purchase order with id " + po.Id.ToString() + "has been updated to " + po.Status + " with receipt number" + po.ReceiptNumber + " please login to have look on it ",
                    po.CreatorUser.EmailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
