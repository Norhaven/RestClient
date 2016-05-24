using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    [TestClass]
    public class UnitTest1
    {   
        [TestMethod]
        public async Task TestMethod1()
        {
            var testHttpClient = new TestHttpClient();
            var client = new Client<IPostBody>(new Uri("http://127.0.0.1"), testHttpClient);
            var request = new Request();
            var response = await client.CallAsync(x => x.Run(request));
        }
    }
}
