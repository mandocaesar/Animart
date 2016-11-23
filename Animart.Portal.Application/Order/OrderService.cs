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
        private readonly IRepository<Invoice.Invoice, Guid> _invoiceRepository;
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
            IUnitOfWorkManager unitOfWorkManager, IRepository<PurchaseOrder, Guid> purchaseOrderRepository, IRepository<Invoice.Invoice, Guid> invoiceRepository, IRepository<SupplyItem, Guid> supplyRepository, 
            IRepository<ShipmentCost, Guid> shipmentCostRepository, IRepository<City, Guid> cityRepository)
        {
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _orderDomainService = orderDomainService;
            _unitOfWorkManager = unitOfWorkManager;
            _purchaseOrderRepository = purchaseOrderRepository;
            _invoiceRepository = invoiceRepository;
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
                    result.CreationTime = result.CreationTime;
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public InvoicePODto GetSingleInvoice(string id)
        {
            try
            {
                Guid _id;
                Guid invId;
                if (Guid.TryParse(id, out _id))
                {
                    invId = Guid.Parse(id);

                    var orderItems = _orderItemRepository.GetAll().Where(e => e.InvoiceId == invId).ToList();
                    var invoice = orderItems[0].Invoice;
                    //var result = _purchaseOrderRepository.GetAll().FirstOrDefault(e=>e.OrderItems.FirstOrDefault().InvoiceId == invId).MapTo<InvoicePODto>();
                    _id = orderItems[0].PurchaseOrder.Id;
                    var result = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == _id).MapTo<InvoicePODto>();
                    result.InvoiceNumber = invoice.InvoiceNumber;
                    result.ExpeditionAdjustment = invoice.Expedition;
                    result.ResiNumber = invoice.ResiNumber;
                    result.CreationTime = invoice.CreationTime;

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
                    var nextKiloAdjustment = (shipmentAdjustment != null) ? (shipmentAdjustment.NextKilo) : 0;

                    result.Items = orderItems.Select(e => e.MapTo<OrderItemDto>()).ToList();
                    var totalGram = result.Items.Sum(e => e.Item.Weight * e.QuantityAdjustment);
                    var totalKilo = (int)((totalGram + 999) / 1000);

                    result.TotalWeight = totalKilo;
                    result.ShipmentCost = nextKilo;
                    result.ShipmentCostFirstKilo = firstKilo;
                    result.KiloQuantity = kiloQuantity;

                    result.ShipmentAdjustmentCost = nextKiloAdjustment;
                    result.ShipmentAdjustmentCostFirstKilo = firstKiloAdjustment;
                    result.KiloAdjustmentQuantity = kiloQuantityAdjustment;

                    result.TotalShipmentCost = (nextKilo * Math.Max(totalKilo - kiloQuantity, 0)) + (firstKilo);
                    result.TotalAdjustmentShipmentCost = (nextKiloAdjustment * Math.Max(totalKilo - kiloQuantityAdjustment, 0)) + (firstKiloAdjustment);
                    result.CreatorUser = user;
                    //result.CreationTime = result.CreationTime.ToUniversalTime();
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
                    //})

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

        public bool AddOrderItemToInvoice(string id, List<InvoiceInputDto> invoiceInput)
        {
            try
            {
                Guid invId = Guid.Parse(id);
                List<Guid> ids = new List<Guid>();

                var inv = _invoiceRepository.GetAll().First(e => e.Id == invId);

                foreach (var orderItem in invoiceInput.Where(i=>i.Checked))
                {
                    bool isInvoice = false;
                    var order = _orderItemRepository.GetAll().First(i => i.Id == orderItem.Id);
                    isInvoice = order.InvoiceId.HasValue;
                    if (isInvoice)
                    {
                        ids.Add(order.InvoiceId.Value);
                    }
                    order.InvoiceId = invId;
                    order.Invoice = inv;
                    _orderItemRepository.Update(order);
                }
                inv.GrandTotal = invoiceInput.Where(i => i.Checked).Sum(e => e.PriceAdjustment * e.QuantityAdjustment);
                inv.TotalWeight = invoiceInput.Where(i => i.Checked).Sum(e => e.Item.Weight * e.QuantityAdjustment);
                _invoiceRepository.Update(inv);

                ids = ids.Distinct().ToList();
                if (ids.Count > 0)
                {
                    foreach (var item in ids)
                    {
                        var query = _invoiceRepository.GetAll().First(e => e.Id == item);

                        if (query.OrderItems.Count==0)
                            _invoiceRepository.Delete(item);
                        else
                        {
                            query.GrandTotal = query.OrderItems.Sum(e => e.PriceAdjustment * e.QuantityAdjustment);
                            query.TotalWeight = query.OrderItems.Sum(e => e.Item.Weight * e.QuantityAdjustment);
                            _invoiceRepository.Update(query);
                        }

                    }
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
            DateTime date = DateTime.Now;
            string code = purchaseOrderItem.IsPreOrder? "PO" : "RS";
            string year = date.Year.ToString();
            string month = "00"+date.Month;
            string day = "00"+date.Day;
            string dateStr = year.Substring(2)
                             + month.Substring(month.Length-2)
                             + day.Substring(day.Length-2);
            var data = _purchaseOrderRepository.GetAll().
                Where(i => i.Code.Substring(0,8) == code + dateStr);
            string max = "0";
            if (data.Count() > 0) { 
                max = data.Max(i=>i.Code);
                max = max.Substring(max.Length - 5);
            }

            int num;
            int.TryParse(max, out num);

            max = ("00000" + ++num);
            code = code + dateStr + "-" + max.Substring(max.Length-5);

            _unitOfWorkManager.Begin();
            return await _purchaseOrderRepository.InsertAndGetIdAsync(new PurchaseOrder()
            {
                Code = code,
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
                CreationTime = date,
                CreatorUser = _userRepository.Get(AbpSession.GetUserId()),
                CreatorUserId = AbpSession.GetUserId()
            });
        }
        public async Task<Guid> CreateInvoice(Dto.CreatePurchaseOrderDto purchaseOrderItem)
        {
            DateTime date = DateTime.Now;
            string code = purchaseOrderItem.IsPreOrder ? "INV-PO" : "INV-RS";
            string year = date.Year.ToString();
            string month = "00" + date.Month;
            string day = "00" + date.Day;
            string dateStr = year.Substring(2)
                             + month.Substring(month.Length - 2)
                             + day.Substring(day.Length - 2);
            var data = _invoiceRepository.GetAll().
                Where(i => i.InvoiceNumber.Substring(0, 12) == code + dateStr);
            string max = "0";
            if (data.Count()>0)
            {
                max = data.Max(i => i.InvoiceNumber);
                max = max.Substring(max.Length - 5);
            }

            int num;
            int.TryParse(max, out num);

            max = ("00000" + ++num);
            code = code + dateStr + "-" + max.Substring(max.Length - 5);

            _unitOfWorkManager.Begin();
            return await _invoiceRepository.InsertAndGetIdAsync(new Invoice.Invoice()
            {
                InvoiceNumber = code,
                Expedition = purchaseOrderItem.ExpeditionAdjustment.Trim(),
                CreationTime = date,
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

        //INI YANG BELOOM KE BAWAH
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
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/"+id
                                  + breakLine + breakLine + "Thank you";
                        break;
                    case "logistic":
                        message = "Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been update to \"LOGISTIC\" for delivery." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/" + id
                                  + breakLine + breakLine + "Thank you";
                        break;
                    default:
                        message = " Dear retailer," + breakLine + breakLine 
                                  + "Your purchase order with number: " + POid + " has been updated to \"" + status + "\"." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/" + id
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

        public bool UpdateOrderItemStatus(string id, string status, List<InvoiceInputDto> orderItems)
        {
            try
            {
                var POid = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == POid);
                po.Status = status;
                _purchaseOrderRepository.Update(po);


                for (int i = 0; i < orderItems.Count(e=>e.Checked); i++)
                {
                    var order = orderItems[i];
                    var data = _orderItemRepository.FirstOrDefault(e=>e.Id== order.Id);
                    data.Status = status;
                    _orderItemRepository.Update(data);
                }

                GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                string message = "";
                string breakLine = "<br/>";
                switch (status.Trim().ToLower())
                {
                    case "payment":
                        message = "Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been update to \"PAYMENT\"." + breakLine + breakLine
                                  + "Please kindly make a bank wire transfer to our account, and upload the bank wire transfer receipt via our system."
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/" + id
                                  + breakLine + breakLine + "Thank you";
                        break;
                    case "logistic":
                        message = "Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been update to \"LOGISTIC\" for delivery." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/" + id
                                  + breakLine + breakLine + "Thank you";
                        break;
                    default:
                        message = " Dear retailer," + breakLine + breakLine
                                  + "Your purchase order with number: " + POid + " has been updated to \"" + status + "\"." + breakLine + breakLine
                                  + "Please kindly login to your account to check the status of the orders."
                                  + breakLine + breakLine + "To check your order details and invoice, you can check it via link: http://shop.animart.co.id/#/orderDetail/" + id
                                  + breakLine + breakLine + "Thank you";
                        break;
                }
                gmail.SendMessage("Purchase Order " + POid + " Has been updated",
                    message, po.CreatorUser.EmailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private PurchaseOrderDto ConvertStatusToPurchaseOrder(PurchaseOrderDto purchase,string status)
        {
            purchase.Status = status;
            return purchase;
        }

        public List<PurchaseOrderDto> GetAllPurchaseOrderForMarketing(int type,int num)
        {
            try
            {
                var list = new List<PurchaseOrder>();
                string status = "";
                switch (num)
                {
                    case (int)STATUS.REJECT:
                        status = "REJECT"; break;
                    case (int)STATUS.ACCOUNTING:
                        status = "ACCOUNTING"; break;
                    case (int)STATUS.MARKETING:
                        status = "MARKETING"; break;
                    case (int)STATUS.PAYMENT:
                        status = "PAYMENT"; break;
                    case (int)STATUS.PAID:
                        status = "PAID"; break;
                    case (int)STATUS.LOGISTIC:
                        status = "LOGISTIC"; break;
                    case (int)STATUS.DONE:
                        status = "DONE"; break;
                    default:
                        status = "MARKETING"; break;
                }

                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    default:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                }
              
                return list.Select(item => ConvertStatusToPurchaseOrder(item.MapTo<PurchaseOrderDto>(), status)).ToList();
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
                string status = "";
                switch (num)
                {
                    case (int)STATUS.REJECT:
                        status = "REJECT"; break;
                    case (int)STATUS.ACCOUNTING:
                        status = "ACCOUNTING"; break;
                    case (int)STATUS.MARKETING:
                        status = "MARKETING"; break;
                    case (int)STATUS.PAYMENT:
                        status = "PAYMENT"; break;
                    case (int)STATUS.PAID:
                        status = "PAID"; break;
                    case (int)STATUS.LOGISTIC:
                        status = "LOGISTIC"; break;
                    case (int)STATUS.DONE:
                        status = "DONE"; break;
                    default:
                        status = "ACCOUNTING"; break;
                }

                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    default:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                }

                return list.Select(item => ConvertStatusToPurchaseOrder(item.MapTo<PurchaseOrderDto>(), status)).ToList();

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
                string status = "";
                switch (num)
                {
                    case (int)STATUS.LOGISTIC:
                        status = "LOGISTIC"; break;
                    case (int)STATUS.DONE:
                        status = "DONE"; break;
                    default:
                        status = "LOGISTIC"; break;
                }

                switch (type)
                {
                    case (int)TYPE.PREORDER:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    case (int)TYPE.READYSTOCK:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                    default:
                        list = _orderItemRepository.GetAll()
                            .Where(i => i.Status == status && !i.PurchaseOrder.IsPreOrder)
                            .Select(i => i.PurchaseOrder).Distinct().OrderByDescending(i => i.CreationTime)
                            .ToList();
                        break;
                }

                return list.Select(item => ConvertStatusToPurchaseOrder(item.MapTo<PurchaseOrderDto>(), status)).ToList();
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

        public bool InsertInvoiceReceiptNumber(string id, string receipt, List<OrderItemDto> orderItems)
        {
            try
            {
                var poId = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poId);
                orderItems= orderItems.Where(e => e.Checked).ToList();
                for (int i = 0; i < orderItems.Count; i++)
                {
                    var orderId = orderItems[i].Id;
                    var ord =_orderItemRepository.FirstOrDefault(e=>e.Id==orderId);
                    ord.Status = "DONE";
                    _orderItemRepository.Update(ord);
                }
                var invoices = orderItems.Select(e => e.Invoice).Distinct().ToList();
                for (int i = 0; i < invoices.Count(); i++)
                {
                    var invId = invoices[i].Id;
                    var inv = _invoiceRepository.FirstOrDefault(e=>e.Id== invId);
                    if (inv != null)
                    {
                        inv.ResiNumber = receipt;
                        _invoiceRepository.Update(inv);
                        string breakLine = "<br/>";
                        GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                        gmail.SendMessage("Invoice Number " + inv.InvoiceNumber + " Has been updated",
                        "Dear retailer," + breakLine + breakLine +
                        "Your invoice with number:" + inv.InvoiceNumber + " has been updated to \"" + "DONE" + "\"." + breakLine + breakLine
                        + "Your shipment tracking number is: \"" + inv.ResiNumber + "\"." + breakLine
                        + "To track your shipment, use the corresponding couriers service website or contact by phone." + breakLine + breakLine
                        + "Thank you",
                        po.CreatorUser.EmailAddress);
                    }
                }
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
                var poId = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poId);
                //var exAdjustment = _shipmentCostRepository.GetAll().FirstOrDefault(
                //    e => e.Expedition.ToLower() == name.Trim().ToLower());
                if ( po!=null)//exAdjustment != null &&
                {
                    po.ExpeditionAdjustment = name.Trim();
                    _purchaseOrderRepository.Update(po);

                    string breakLine = "<br/>";
                    GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                    gmail.SendMessage("Purchase Order " + po.Id.ToString() + " Has been updated",
                        "Dear retailer," + breakLine + breakLine
                        + "The expedition for your purchase order with number:" + po.Id.ToString() +
                        " has been updated to \"" + name + "\"." + breakLine + breakLine
                        + "Please kindly login to your account to check the status of the orders." + breakLine + breakLine
                        + "Thank you",
                        po.CreatorUser.EmailAddress);
                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateExpeditionAdjustment(string id, string name, List<OrderItemDto> orderItems)
        {
            try
            {
                var poId = Guid.Parse(id);
                var po = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == poId);

                var invoices = orderItems.Where(e => e.Checked).Select(e => e.Invoice).Distinct().ToList();
                for (int i = 0; i < invoices.Count(); i++)
                {
                    var invId = invoices[i].Id;
                    var inv = _invoiceRepository.FirstOrDefault(e => e.Id == invId);
                    if (inv != null)
                    {
                        inv.Expedition = name.Trim();
                        _invoiceRepository.Update(inv);
                        string breakLine = "<br/>";
                        GmailExtension gmail = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
                        gmail.SendMessage("Invoice Order " + inv.InvoiceNumber + " Has been updated",
                            "Dear retailer," + breakLine + breakLine
                            + "The expedition for your purchase order with number:" + inv.InvoiceNumber +
                            " has been updated to \"" + name + "\"." + breakLine + breakLine
                            + "Please kindly login to your account to check the status of the orders." + breakLine + breakLine
                            + "Thank you",
                            po.CreatorUser.EmailAddress);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
