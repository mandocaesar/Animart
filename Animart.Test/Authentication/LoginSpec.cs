using System;
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
        public LoginSpec()
        {
            _userAppService = LocalIocManager.Resolve<IUserAppService>();

        }

        [Test]
        public void WhenUserLoginMustProvideUserNameAndPassword()
        {
            var test = _userAppService.GetUsers();
            test.Items.Count.ShouldBe(1);
            
        }
    }
}
