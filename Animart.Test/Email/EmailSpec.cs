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
            mailService = new GmailExtension(GmailExtension.ANIMART_EMAILADDRESS, GmailExtension.ANIMART_PASSWORD);
        }

        [Test]
        public void WhenSendTestSendEmail()
        {
          result =  mailService.SendMessage("test", "test", "armand.caesar@gmail.com",false,null,null);     
        }

        [TestFixtureTearDown]
        public void ResultShouldReturnTrue()
        {
            result.ShouldBeTrue();
        }

    }
}
