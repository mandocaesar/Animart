using System;
using Animart.Portal.Extension;
using NUnit.Framework;
using Shouldly;

namespace Animart.Test.Email
{
    [TestFixture]
    public class EmailSpec
    {
        private GmailExtension mailService;
        private bool result;

        [TestFixtureSetUp]
        public void GivenValidEmailAddressAndPassword()
        {
            this.result = true;
            mailService = new GmailExtension("marketing@animart.co.id", "GOSALES2015");
        }

        [Test]
        public void WhenSendTestSendEmail()
        {
          result =  mailService.SendMessage("test", "test", "armand.caesar@gmail.com");     
        }

        [TestFixtureTearDown]
        public void ResultShouldReturnTrue()
        {
            result.ShouldBeTrue();
        }

    }
}
