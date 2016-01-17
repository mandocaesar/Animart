using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;

namespace Animart.Test.UserManagement
{
    [TestFixture]
    public class UserRegistrationSpec
    {
        [Test]
        public void WhenUserRegsiter()
        {
            var a = true;
            a.ShouldBe(true);

        }
    }
}
