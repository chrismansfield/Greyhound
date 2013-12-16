using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greyhound.Tests
{
    [TestClass]
    public class MessageKeyTests
    {
        [TestMethod]
        public void Equals_MessageKeyAreEqual_True()
        {
            var sut1 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));
            var sut2 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));

            var result = sut1 == sut2;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_BusNameIsDifferent_False()
        {
            var sut1 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));
            var sut2 = new MessageKey("Another Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));

            var result = sut1 == sut2;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_KeyIsDifferent_False()
        {
            var sut1 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));
            var sut2 = new MessageKey("Dummy Busname", new Guid("1AC2A46C-70B4-44AB-AC0D-99A6E238BCFD"));

            var result = sut1 == sut2;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_EverythingIsDifferent_False()
        {
            var sut1 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));
            var sut2 = new MessageKey("Another Dummy Busname", new Guid("1AC2A46C-70B4-44AB-AC0D-99A6E238BCFD"));

            var result = sut1 == sut2;

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_KeyAreEqualAndBusNameIsAny_True()
        {
            var sut1 = new MessageKey("Dummy Busname", new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));
            var sut2 = new MessageKey(MessageKey.AnyBusName, new Guid("DF9298DF-5735-458B-82B9-2BD95FF93700"));

            var result = sut1 == sut2;

            Assert.IsTrue(result);
        }
    }
}
