using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RestClient.Tests.Client
{
    [TestClass]
    public class Headers
    {
        [TestMethod]
        public async Task PrimitiveMayBeIncludedInHeaderWithDefaultNameOfParameterName()
        {
            var value = TestData.RandomInt();
            var client = TestData.ClientOf<IHeaders>(x => x.RequireHeaderOf("number", value));
            await client.CallAsync(x => x.PrimitiveWithDefaultName(value));
        }

        [TestMethod]
        public async Task PrimitiveMayBeIncludedInHeaderWithSpecifiedName()
        {
            var value = TestData.RandomInt();
            var client = TestData.ClientOf<IHeaders>(x => x.RequireHeaderOf("SpecifiedName", value));
            await client.CallAsync(x => x.PrimitiveWithSpecifiedName(value));
        }
    }
}
