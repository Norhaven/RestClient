using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Net.Http;
using FluentAssertions;

namespace RestClient.Tests
{
    [TestClass]
    public class ComplexObjectInBody
    {   
        [TestMethod]
        public async Task ComplexRequestAndResponseCanBeSentRoundTrip()
        {
            var actualRequest = TestData.OfType<Request>();
            var expectedResponse = TestData.OfType<Response>();

            var client = TestData.ClientOf<IPostBody>(x =>
                x.IfRequestHasStringContentAndContentIs(content => content.IsEqualToSerialized(actualRequest),
                then: () => expectedResponse)
            );
            
            var response = await client.CallAsync(x => x.Run(actualRequest));

            response.Should().NotBeNull("because a non-null response was sent back");
            response.TextContent.Should().Be(expectedResponse.TextContent, "because that's the exact string that was included in the response");
        }
    }
}
