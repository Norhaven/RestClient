using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RestClient.Tests.Client
{
    [TestClass]
    public class QueryStrings
    {
        [TestMethod]
        public async Task PrimitiveMayBeIncludedInQueryStringWithDefaultNameOfParameterName()
        {
            var value = TestData.RandomInt();
            var client = TestData.ClientOf<IQueryStrings>(x => x.RequireQueryStringOf($"number={value}"));
            await client.CallAsync(x => x.PrimitiveWithDefaultName(value));
        }

        [TestMethod]
        public async Task PrimitiveMayBeIncludedInQueryStringWithSpecifiedName()
        {
            var value = TestData.RandomInt();
            var client = TestData.ClientOf<IQueryStrings>(x => x.RequireQueryStringOf($"SpecifiedName={value}"));
            await client.CallAsync(x => x.PrimitiveWithSpecifiedName(value));
        }
    }
}
