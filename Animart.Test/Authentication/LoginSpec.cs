using System;
using Abp.Authorization.Users;
using Abp.UI;
using Animart.Portal.User;
using Animart.Portal.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using TestContext = NUnit.Framework.TestContext;

namespace Animart.Test
{
    /// <summary>
    /// Summary description for LoginSpec
    /// </summary>
    [TestFixture]
    public class LoginSpec:AnimartPortalTestBaseClass
    {
        
        private readonly IUserAppService _userAppService;
        private readonly UserManager _userManager;

        public LoginSpec()
        {
            _userAppService = LocalIocManager.Resolve<IUserAppService>();
            _userManager = LocalIocManager.Resolve<UserManager>();

        }

        [Test]
        public async void WhenUserLoginProvideCorrectUserNameAndPassword()
        {
            var a = await _userManager.LoginAsync("admin@animart.com", "qwe123", "1");
            a.ShouldNotBeNull();
        }

        [Test]
        public async void WhenUserLoginProvideInvalidUserNameOrPassword()
        {
            var loginResult = await _userManager.LoginAsync("admin@animart1.com", "qwe1232", "1");
            loginResult.Result.ShouldBeOneOf(AbpLoginResultType.InvalidUserNameOrEmailAddress,
                AbpLoginResultType.InvalidPassword);
        }
    }
}
