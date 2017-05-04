using System;
using System.Linq;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.WebApi.Controllers;
using Animart.Portal.Order;
using Animart.Portal.Order.Dto;
using Animart.Portal.Shipment;
using Animart.Portal.Supply;
using Animart.Portal.Users.Dto;
using System.Web.Http;

namespace Animart.Portal.Api.Controllers
{
    public class InvoiceController : AbpApiController
    {

        private readonly IRepository<OrderItem, Guid> _orderItemRepository;
        private readonly IRepository<Users.User, long> _userRepository;
        private readonly IRepository<PurchaseOrder, Guid> _purchaseOrderRepository;
        private readonly IRepository<ShipmentCost, Guid> _shipmentCostRepository;
        private readonly IRepository<City, Guid> _cityRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public InvoiceController(IRepository<OrderItem, Guid> orderItemRepository, IRepository<Users.User, long> userRepository,
            IRepository<PurchaseOrder, Guid> purchaseOrderRepository, IUnitOfWorkManager unitOfWorkManager,
           IRepository<ShipmentCost, Guid> shipmentCostRepository, IRepository<City, Guid> cityRepository)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _shipmentCostRepository = shipmentCostRepository;
            _cityRepository = cityRepository;
        }

        [HttpGet]
        public string Test()
        {
            return "TEST";
        }

        [HttpGet]
        public InvoicePODto GetSingleInvoice(string id)
        {
            try
            {
                Guid _id;
                Guid invId;
                if (Guid.TryParse(id, out _id))
                {
                    invId = Guid.Parse(id);
                    _unitOfWorkManager.Begin();
                    var orderItems = _orderItemRepository.GetAll().Where(e => e.InvoiceId == invId).ToList();
                    var invoice = orderItems[0].Invoice;
                    _id = orderItems[0].PurchaseOrder.Id;
                    var result = _purchaseOrderRepository.GetAll().FirstOrDefault(e => e.Id == _id).MapTo<InvoicePODto>();
                    result.InvoiceNumber = invoice.InvoiceNumber;
                    result.ExpeditionAdjustment = invoice.Expedition;
                    result.ResiNumber = invoice.ResiNumber;
                    result.CreationTime = invoice.CreationTime;

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
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
