using System;
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
    public class OrderService: ApplicationService,IOrderService
    {
        private readonly IRepository<OrderItem,Guid> _orderItemRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly IRepository<SupplyItem, Guid> _supplyItemRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly OrderDomainService _orderDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OrderService(IRepository<OrderItem, Guid> orderItemRepository , IRepository<Users.User, long> userRepository, OrderDomainService orderDomainService, 
            IUnitOfWorkManager unitOfWorkManager, IRepository<PurchaseOrder,Guid> purchaseOrderRepository)
        {
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _orderDomainService = orderDomainService;
            _unitOfWorkManager = unitOfWorkManager;
            _purchaseOrderRepository = purchaseOrderRepository;

        }

        public bool AddOrderItem(string id, OrderItemInputDto orderItem)
        {
            try
            {
                _orderItemRepository.InsertOrUpdate(new OrderItem()
                {
                    Item = _supplyItemRepository.Get(orderItem.supplyItem),
                    PurchaseOrder = _purchaseOrderRepository.Get(orderItem.PurchaseOrder),
                    Quantity = orderItem.Quantity,
                    CreationTime = DateTime.Now,
                    CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                    CreatorUserId = AbpSession.GetUserId()
                });
                return true;

            }
            catch (Exception)
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

        public async Task Create(Dto.CreatePurchaseOrderDto purchaseOrderItem)
        {

            await _purchaseOrderRepository.InsertOrUpdateAndGetIdAsync(new PurchaseOrder()
            {
                Address = purchaseOrderItem.Address,
                City = purchaseOrderItem.City,
                Expedition = purchaseOrderItem.Expedition,
                GrandTotal = purchaseOrderItem.GrandTotal,
                Province = purchaseOrderItem.Province,
                Status = purchaseOrderItem.Status,
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
    }
}
