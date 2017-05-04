using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using Microsoft.AspNet.Identity;
using Animart.Portal.Users;
using Animart.Portal.Supply;
using Animart.Portal.Supply.Dto;
using Abp.Authorization;

namespace Animart.Test.Supply
{
    [TestFixture]
    public class CreateSupplyTest:AnimartPortalTestBaseClass
    {
        private readonly ISupplyService _supplyService;
        private readonly IUserAppService _userAppService;
        private readonly UserManager _userManager;
        private SupplyItem _supplyItem;
        private SupplyItemDto _supplyItemDto;


        public CreateSupplyTest()
        {
            _supplyService = LocalIocManager.Resolve<ISupplyService>();
            _userAppService = LocalIocManager.Resolve<IUserAppService>();
            _userManager = LocalIocManager.Resolve<UserManager>();
        }

        [TestFixtureSetUp]
        public void GivenSupplyWithValidModel()
        {
            _supplyItemDto = new SupplyItemDto()
            {
                Available = true,
                Code = "Test-123",
                InStock = 20,
                Name = "Gundamdam",
                Price = 20000
            };
            AbpSession.UserId = 1;
        }

        [Test]
        public async void When_001SaveSupplyItemShouldSaveToDatabase()
        {
            await _supplyService.Create(_supplyItemDto);
            _supplyService.GetSupplyByName(new GetSupplyByNameInput()
            {
                MaxResultCount = 10,
                Name = "Gundamdam",
                SkipCount = 0,
                Sorting = "CreationTime DESC"
            }).ShouldNotBeNull();
            
        }

        [Test]
        public void When_002UpdateSupplyItemShouldBeTrue()
        {
            _supplyItemDto.Id = _supplyService.GetSingleByName("Gundamdam").Id;
            _supplyItemDto.InStock = 21;
            _supplyService.Update(_supplyItemDto).ShouldBeTrue();
        }

        [Test]
        public void When_003DeleteSupplyItemShouldBeTrue()
        {
            _supplyService.Delete(_supplyService.GetSingleByName("Gundamdam").Id);
        }
    }
}
