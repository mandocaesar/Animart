using System;
using Animart.Portal.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using Microsoft.AspNet.Identity;

namespace Animart.Test.UserManagement
{
    [TestFixture]
    public class UserRegistrationSpec : AnimartPortalTestBaseClass
    {
        private readonly IUserAppService _userAppService;
        private readonly UserManager _userManager;

        public UserRegistrationSpec()
        {
            _userAppService = LocalIocManager.Resolve<IUserAppService>();
            _userManager = LocalIocManager.Resolve<UserManager>();
        }

        [Test]
        public async void WhenUser_01RegisterItShouldReturnTrue()
        {
            var result = await _userManager.CreateAsync(new User()
            {
                TenantId = 1,
                UserName = "test1",
                Name = "testUser1",
                Surname = "test1",
                EmailAddress = "test@animart.com",
                IsEmailConfirmed = true,
                Password = new PasswordHasher().HashPassword("ZXasqw12"),

            });
            result.Succeeded.ShouldBeTrue();
        }

        [Test]
        public void WhenUser_02SetToHaveRoleShouldBeSucced()
        {
           var user =  _userManager.FindByEmail("test@animart.com");

            var result =  _userManager.AddToRoles(user.Id,"User");

            result.Succeeded.ShouldBeTrue();
        }

        [Test]
        public async void WhenUser_03LoginWithNewlyRegisterAccountShouldBeSuccess()
        {
            var result = await _userManager.LoginAsync("test1", "ZXasqw12", "Default");
            result.Result.ShouldBe(Abp.Authorization.Users.AbpLoginResultType.Success);
        }
    }
}
