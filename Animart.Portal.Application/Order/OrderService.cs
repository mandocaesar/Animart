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
using Animart.Portal.Extension;
using Animart.Portal.Order.Dto;
using Animart.Portal.Shipment;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;
using AutoMapper;
using Animart.Portal.Users.Dto;

namespace Animart.Portal.Order
{
    [AbpAuthorize]
    public class OrderService : ApplicationService, IOrderService
    {
        private readonly IRepository<OrderItem, Guid> _orderItemRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly IRepository<SupplyItem, Guid> _supplyItemRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<ShipmentCost, Guid> _shipmentCostRepository;
        private readonly IRepository<City, Guid> _cityRepository;
        private readonly OrderDomainService _orderDomainService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private enum STATUS
        {
            REJECT=0,
            MARKETING = 1,
            ACCOUNTING = 2,
            PAYMENT=3,
            LOGISTIC = 4,
            DONE = 5,
            PAID = 6,
        }

        private enum TYPE
        {
            READYSTOCK = 0,
            PREORDER=1
        }


        public OrderService(IRepository<OrderItem, Guid> orderItemRepository, IRepository<Users.User, long> userRepository, OrderDomainService orderDomainService,
            IUnitOfWorkManager unitOfWorkManager, IRepository<PurchaseOrder, Guid> purchaseOrderRepository, IRepository<SupplyItem, Guid> supplyRepository, 
            IRepository<ShipmentCost, Guid> shipmentCostRepository, IRepository<City, Guid> cityRepository)
        {
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _orderDomainService = orderDomainService;
            _unitOfWorkManager = unitOfWorkManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplyItemRepository = supplyRepository;
            _shipmentCostRepository = shipmentCostRepository;
            _cityRepository = cityRepository;

        }


        public OrderDashboardDto GetDashboard()
        {
            var result = new OrderDashboardDto();
            var userId = AbpSession.GetUserId();
            result.BDO = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "MARKETING" || e.Status == "ACCOUNTING");
            result.Delivered = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "LOGISTIC" || e.Status == "DONE");
            result.Waiting = _purchaseOrderRepository.Count(e => e.CreatorUserId == userId && e.Status == "PAYMENT");
            return result;
        }


        public OrderDashboardDto GetDashboardAdmin()
        {
            var result = new OrderDashboardDto();
            var userId = AbpSession.GetUserId();
            result.BDO = _purchaseOrderRepository.Count(e => e.Status == "MARKETING" || e.Status=="ACCOUNTING");
            result.Delivered = _purchaseOrderRepository.Count(e => e.Status == "LOGISTIC" || e.Status == "DONE");
            result.Waiting = _purchaseOrderRepository.Count(e => e.Status == "PAYMENT");
            result.Marketing = _purchaseOrderRepository.Count(e => e.Status == "MARKETING" );
            result.Accounting = _purchaseOrderRepository.Count(e =>e.Status == "ACCOUNTING");
            result.Done = _purchaseOrderRepository.Count(e => e.Status == "DONE");
            result.Delivery = _purchaseOrderRepository.Count(e => e.Status == "LOGISTIC");

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
                    var user = _userRepository.Get(result.CreatorUserId.Value).MapTo<UserDto>(); ;
                    
                    var _expedition = result.Expedition.Split('-')[0];
                    var _expeditionAdjustment = result.ExpeditionAdjustment.Split('-')[0];

                    var _city = result.City;
                    var _type = result.Expedition.Split('-')[1];
                    var _typeAdjustment = result.ExpeditionAdjustment.Split('-')[1];
                    var cityId = _cityRepository.Single(e => e.Name.ToLower() == _city.ToLower());
                    var shipment =
                        _shipmentCostRepository.GetAllList()
                            .FirstOrDefault(e => e.Expedition == _expedition && e.City == cityId && e.Type == _type);
                    var shipmentAdjustment =
                       _shipmentCostRepository.GetAllList()
                           .FirstOrDefault(e => e.Expedition == _expeditionAdjustment && e.City == cityId && e.Type == _typeAdjustment);

                    var firstKilo = (shipment != null) ? (shipment.FirstKilo) : 0;
                    var kiloQuantity = (shipment != null) ? (shipment.KiloQuantity) : 1;
                    var nextKilo = (shipment != null) ? (shipment.NextKilo) : 0;

                    var kiloQuantityAdjustment = (shipmentAdjustment != null) ? (shipmentAdjustment.KiloQuantity) : 0;
                    var firstKiloAdjustment = (shipmentAdjustment != null) ? (shipmentAdjustment.FirstKilo) : 0;
                    var nextKiloAdjustment =  (shipmentAdjustment != null) ? (shipmentAdjustment.NextKilo) : 0;

                    var orderItems = _orderItemRepository.GetAll().Where(e => e.PurchaseOrder.Id == result.Id).ToList();
                    result.Items = orderItems.Select(e=>e.MapTo<OrderItemDto>()).ToList();
                    var totalGram = result.Items.Sum(e => e.Item.Weight*e.QuantityAdjustment);
                    var totalKilo = (int) ((totalGram + 999)/1000);

                    result.TotalWeight = totalKilo ;
                    result.ShipmentCost = nextKilo;
                    result.ShipmentCostFirstKilo = firstKilo;
                    result.KiloQuantity = kiloQuantity;

                    result.ShipmentAdjustmentCost = nextKiloAdjustment;
                    result.ShipmentAdjustmentCostFirstKilo = firstKiloAdjustment;
                    result.KiloAdjustmentQuantity = kiloQuantityAdjustment;
                    
                    result.TotalShipmentCost = (nextKilo * Math.Max(totalKilo-kiloQuantity,0))+(firstKilo);
                    result.TotalAdjustmentShipmentCost = (nextKiloAdjustment * Math.Max(totalKilo - kiloQuantityAdjustment, 0)) + (firstKiloAdjustment);
                    result.CreatorUser = user;
                    result.CreationTime = result.CreationTime.ToUniversalTime();
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
                        orderItem.Quantity = supplyItem.InStock;
                        //return false;
                    }

                    //if (orderItem.Quantity == 0)
                    //{
                    //    return true;
                    //}

                    var item = new OrderItem()
                    {
                        Item = supplyItem,
                        PurchaseOrder = _purchaseOrderRepository.GetAllList().First(e => e.Id == poId),
                        Quantity = orderItem.Quantity,
                        QuantityAdjustment = orderItem.Quantity,
                        Name = orderItem.Name,
                        PriceAdjustment = supplyItem.Price,
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
                PhoneNumber = purchaseOrderItem.PhoneNumber,
                City = purchaseOrderItem.City.Trim(),
                Expedition = purchaseOrderItem.Expedition.Trim(),
                ExpeditionAdjustment = purchaseOrderItem.Expedition.Trim(),
                IsPreOrder = purchaseOrderItem.IsPreOrder,
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

        public bool Update(string id , Dto.OrderItemDto orderItem)
        {
            try
            {
                var item = _orderItemRepository.GetAll().FirstOrDefault(e=>e.Id == orderItem.Id);
                //item.Item = _supplyItemRepository.GetAll().FirstOrDefault(e=>e.Id == orderItem.Item.Id);
                item.PriceAdjustment = orderItem.PriceAdjustment;
                item.QuantityAdjustment = orderItem.QuantityAdjustment;

                var poid = Guid.Parse(id);
                item.PurchaseOrder = _purchaseOrderRepository.GetAll().FirstOrDefault(e=>e.Id == poid);
                item.PurchaseOrder.ModifiedBy = _userRepository.Get(AbpSession.GetUserId());
                item.PurchaseOrder.ModifiedOn = DateTime.Now;

                var total = 0;
               // var totalWeight = 0;
                var totalGram = 0;
                var items = _orderItemRepository.GetAll().Where(e => e.PurchaseOrder.Id == poid).ToList();

                //var expedition = item.PurchaseOrder.Expedition.Split('-');
                //var expeditionName = expedition[0].Trim();
                //var type = expedition[1].Trim();
                //var city = _cityRepository.Single(e => e.Name == item.PurchaseOrder.City);
                //var cost = _shipmentCostRepository.GetAllList().FirstOrDefault(e => e.Expedition == expeditionName && e.Type == type  && e.City == city);

                for (int i = 0; i < items.Count; i++)
                {
                    totalGram += (int)(items[i].QuantityAdjustment*items[i].Item.Weight);
                    var price = items[i].PriceAdjustment;// == 0 ? items[i].Item.Price : items[i].PriceAdjustment;
                    total += (price*items[i].QuantityAdjustment);// + (cost.First5Kilo * items[i].Quantity);
                }
                item.PurchaseOrder.TotalWeight = totalGram;

                item.PurchaseOrder.GrandTotal = total ;

                _orderItemRepository.Update(item);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool UpdatePO(string poId, List<Dto.OrderItemDto> orderItems)
        {
            try
            {
                foreach (var orderItem in orderItems)
                {
                    var item = _orderItemRepository.GetAll().FirstOrDefault(e => e.Id == orderItem.Id);
                    item.PriceAdjustment = orderItem.PriceAdjustment;
                    item.QuantityAdjustment = orderItem.QuantityAdjustment;

                    var poid = Guid.Parse(poId);
                    item.PurchaseOrder = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poid);
                    item.PurchaseOrder.ModifiedBy = _userRepository.Get(AbpSession.GetUserId());
                    item.PurchaseOrder.ModifiedOn = DateTime.Now;

                    var total = 0;
                    var totalGram = 0;
                    var items = _orderItemRepository.GetAll().Where(e => e.PurchaseOrder.Id == poid).ToList();
                    
                    for (int i = 0; i < items.Count; i++)
                    {
                        totalGram += (int)(items[i].QuantityAdjustment * items[i].Item.Weight);
                        var price = items[i].PriceAdjustment;
                        total += (price * items[i].QuantityAdjustment);
                    }
                    item.PurchaseOrder.TotalWeight = totalGram;

                    item.PurchaseOrder.GrandTotal = total;

                    _orderItemRepository.Update(item);
                }
               
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

        public List<PurchaseOrderDto> GetAllPurchaseOrderByUserId(int type,int num)
        {
            try
            {
                var uid = AbpSession.GetUserId();
                var list = new List<PurchaseOrder>();
                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.CreatorUserId == uid && e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.CreatorUserId == uid && !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    default:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.CreatorUserId == uid && !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                }
                
                switch (num)
                {

                    case (int)STATUS.REJECT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "REJECT").ToList();
                    case (int)STATUS.ACCOUNTING:
                    case (int)STATUS.MARKETING:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING" || w.Status == "ACCOUNTING").ToList();
                    case (int)STATUS.PAYMENT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAYMENT").ToList();
                    case (int)STATUS.PAID:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAID").ToList();
                    case (int)STATUS.LOGISTIC:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "LOGISTIC").ToList();
                    case (int)STATUS.DONE:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "DONE").ToList();                        
                    default:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING" || w.Status == "ACCOUNTING").ToList();
                }

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
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == POid);

                po.Status = status;
                GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                string message = "";
                string breakLine = "<br/>";
                switch (status.Trim().ToLower())
                {
                    case "payment":
                        message = "Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been update to \"PAYMENT\"." + breakLine + breakLine
                                  + "Please kindly make a bank wire transfer to our account, and upload the bank wire transfer receipt via our system."
                                  + breakLine + breakLine + "Thank you";
                        break;
                    case "logistic":
                        message = "Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been update to \"LOGISTIC\" for delivery." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "Thank you";
                        break;
                    default:
                        message = " Dear retailer," + breakLine + breakLine 
                                  + "Your purchase order with number: " + POid + " has been updated to \"" + status + "\"." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "Thank you";
                        break;
                }
                gmail.SendMessage("Purchase Order " + POid + " Has been updated",
                    message, po.CreatorUser.EmailAddress);
                _purchaseOrderRepository.Update(po);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForMarketing(int type,int num)
        {
            try
            {
                var list = new List<PurchaseOrder>();
                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.IsPreOrder).OrderByDescending(i=>i.CreationTime).ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    default:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                }
                //List<PurchaseOrder> list = new List<PurchaseOrder>();
                //PurchaseOrder temp;
                //foreach (var item in listData)
                //{
                //    temp =  new PurchaseOrder
                //    {
                //        Id=item.Id,
                //        Address=item.Address,
                //        GrandTotal=item.GrandTotal,
                //        IsPreOrder=item.IsPreOrder,
                //        ModifiedBy=item.ModifiedBy,
                //        ModifiedOn=item.ModifiedOn,
                //        ExpeditionAdjustment=item.ExpeditionAdjustment,
                //        City=item.City,
                //        CreationTime=item.CreationTime.ToUniversalTime(),
                //        CreatorUser=item.CreatorUser,
                //        CreatorUserId=item.CreatorUserId,
                //        Expedition=item.Expedition,
                //        OrderItems=item.OrderItems,
                //        PhoneNumber=item.PhoneNumber,
                //        PostalCode=item.PostalCode,
                //        Province=item.Province,
                //        ReceiptNumber=item.ReceiptNumber,
                //        Status=item.Status,
                //        TotalWeight= item.TotalWeight
                //    };
                //    listData.Add(temp);
                //}
                switch (num)
                {
                    case (int)STATUS.REJECT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "REJECT").ToList();
                    case (int)STATUS.ACCOUNTING:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "ACCOUNTING").ToList();
                    case (int)STATUS.MARKETING:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING").ToList();
                    case (int)STATUS.PAYMENT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAYMENT").ToList();
                    case (int)STATUS.PAID:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAID").ToList();
                    case (int)STATUS.LOGISTIC:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "LOGISTIC").ToList();
                    case (int)STATUS.DONE:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "DONE").ToList();
                    default:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING").ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForAccounting(int type,int num)
        {
            try
            {
                var list = new List<PurchaseOrder>();
                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    default:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                }
                switch (num)
                {
                    case (int)STATUS.REJECT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "REJECT").ToList();
                    case (int)STATUS.ACCOUNTING:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "ACCOUNTING").ToList();
                    case (int)STATUS.MARKETING:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING").ToList();
                    case (int)STATUS.PAYMENT:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAYMENT").ToList();
                    case (int)STATUS.PAID:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAID").ToList();
                    case (int)STATUS.LOGISTIC:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "LOGISTIC").ToList();
                    case (int)STATUS.DONE:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "DONE").ToList();
                    default:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "ACCOUNTING").ToList();
                }
                
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForLogistic(int type,int num)
        {
            try
            {
                var list = new List<PurchaseOrder>();
                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _purchaseOrderRepository.GetAll().Where(e => e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                    default:
                        list = _purchaseOrderRepository.GetAll().Where(e => !e.IsPreOrder).OrderByDescending(i => i.CreationTime).ToList();
                        break;
                }
                switch (num)
                {
                    //case (int)STATUS.REJECT:
                    //    return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "REJECT").ToList();
                    //case (int)STATUS.ACCOUNTING:
                    //    return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "ACCOUNTING").ToList();
                    //case (int)STATUS.MARKETING:
                    //    return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "MARKETING").ToList();
                    //case (int)STATUS.PAYMENT:
                    //    return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "PAYMENT").ToList();
                    case (int)STATUS.LOGISTIC:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "LOGISTIC").ToList();
                    case (int)STATUS.DONE:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "DONE").ToList();
                    default:
                        return list.Select(item => item.MapTo<PurchaseOrderDto>()).Where(w => w.Status == "LOGISTIC").ToList();
                }
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
                string breakLine = "<br/>";
                var poid = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poid);
                po.ReceiptNumber = receipt;
                po.Status = "DONE";
                _purchaseOrderRepository.Update(po);
                GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                gmail.SendMessage("Purchase Order " + po.Id.ToString() + " Has been updated", 
                    "Dear retailer,"+breakLine+breakLine+
                    "Your purchase order with number:" + po.Id.ToString() + " has been updated to \"" + po.Status +"\"."+breakLine+breakLine
                    + "Your shipment tracking number is: \"" + po.ReceiptNumber+"\"."+ breakLine
                    + "To track your shipment, use the corresponding couriers service website or contact by phone."+ breakLine+breakLine
                    + "Thank you",
                    po.CreatorUser.EmailAddress);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertExpeditionAdjustment(string id, string name)
        {
            try
            {
                string breakLine = "<br/>";
                var poid = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poid);
                po.ExpeditionAdjustment = name.Trim();
                _purchaseOrderRepository.Update(po);
                GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                gmail.SendMessage("Purchase Order " + po.Id.ToString() + " Has been updated", 
                    "Dear retailer,"+breakLine+breakLine
                    +"The expedition for your purchase order with number:" + po.Id.ToString() + " has been updated to \"" + name  +"\"."+breakLine+breakLine
                    +"Please kindly login to your account to check the status of the orders."+breakLine+breakLine
                    + "Thank you",
                    po.CreatorUser.EmailAddress);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
