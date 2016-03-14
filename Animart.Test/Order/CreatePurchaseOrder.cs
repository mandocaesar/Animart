using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using Microsoft.AspNet.Identity;
using Animart.Portal.Users;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;
using Abp.Authorization;
using Animart.Portal.Order;
using Animart.Portal.Order.Dto;

namespace Animart.Test.Order
{
    [TestFixture]
    public class CreatePurchaseOrder:AnimartPortalTestBaseClass
    {
        private readonly IOrderService _orderService;
        private readonly ISupplyService _supplyService;
        private readonly UserManager _userManager;
        private PurchaseOrder _purchaseOrder;
        private PurchaseOrderDto _purchaseOrderDto;
        private CreatePurchaseOrderDto _createPurchaseOrderDto;
        

        public CreatePurchaseOrder()
        {
            _supplyService = LocalIocManager.Resolve<ISupplyService>();
            _orderService = LocalIocManager.Resolve<IOrderService>();
            _userManager = LocalIocManager.Resolve<UserManager>();
        }

        [TestFixtureSetUp]
        public void GivenPurchaseOrder()
        {
            _createPurchaseOrderDto = new CreatePurchaseOrderDto()
            {
                Address = "Test Address",
                City = "Test City",
                Expedition = "EXP-Reguler",
                GrandTotal = 10000,
                Province = "Test Province",
                Status = "Accounting",
                TotalWeight = 1
            };
            AbpSession.UserId = 1;
        }

        [Test]
        public async void When_001CreatePurchaseOrderShouldNotError()
        {
            await _orderService.Create(_createPurchaseOrderDto);
            _orderService.GetAllPurchaseOrderByUserId().ShouldNotBeNull();
        }


        [Test]
        public void WhenGetAllOrderItems()
        {
            var result = _orderService.GetAllPurchaseOrderByUserId().ToList();
            _purchaseOrderDto = result[0];
            result.ShouldNotBeNull();
        }

        [Test]
        public void When_002AddOrderItemShouldBeTrue()
        {
            var item = _supplyService.GetSingleByName("Gundamdam");
            var result = _orderService.GetAllPurchaseOrderByUserId().ToList();
            _purchaseOrderDto = result[0];

            _orderService.AddOrderItem(_purchaseOrder.Id.ToString(), new OrderItemInputDto()
            {
                PurchaseOrder = _purchaseOrder.Id,
                Quantity = 10,
                supplyItem = item.Id    
            }).ShouldBeTrue();
        }

        //[Test]
        //public void WhenDeleteOrderItemShouldBeTrue()
        //{

        //}
    }
}
