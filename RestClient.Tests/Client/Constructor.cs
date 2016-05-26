using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestClient.Tests.Client
{
    [TestClass]
    public class Constructor
    {
        private sealed class NonInterfaceType { }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ClassTypeSpecifiedInsteadOfInterfaceWillFail()
        {
            var client = TestData.ClientOf<NonInterfaceType>();
        }
    }
}
