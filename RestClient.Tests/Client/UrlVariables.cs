using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RestClient.Tests.Client
{
    [TestClass]
    public class UrlVariables
    {
        [TestMethod]
        public async Task ComplexObjectMayBeDereferencedInRoute()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IUrlVariables>(x => x.RequirePathOf($"/{request.TextContent}"));
            await client.CallAsync(x => x.DereferencedComplexObject(request));
        }

        [TestMethod]
        public async Task PrimitiveObjectMayBeDirectlyIncludedInRoute()
        {
            var number = TestData.RandomInt();
            var client = TestData.ClientOf<IUrlVariables>(x => x.RequirePathOf($"/{number}"));
            await client.CallAsync(x => x.DirectPrimitiveInclusion(number));
        }

        [TestMethod]
        public async Task MultiplePrimitiveObjectsMayBeDirectlyIncludedInRoute()
        {
            var number = TestData.RandomInt();
            var otherNumber = TestData.RandomInt();
            var client = TestData.ClientOf<IUrlVariables>(x => x.RequirePathOf($"/{number}/{otherNumber}"));
            await client.CallAsync(x => x.MultipleDirectPrimitiveInclusion(number, otherNumber));
        }

        [TestMethod]
        public async Task DirectlyIncludedPrimitivesAndDereferencedComplexObjectsMayBeIncludedInRouteTogether()
        {
            var number = TestData.RandomInt();
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IUrlVariables>(x => x.RequirePathOf($"/{number}/{request.TextContent}"));
            await client.CallAsync(x => x.DirectPrimitiveAndComplexDereference(number, request));
        }
    }
}
