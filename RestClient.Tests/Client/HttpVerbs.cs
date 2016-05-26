using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient.Tests.Client
{
    [TestClass]
    public class HttpVerbs
    {
        [TestMethod]
        public async Task PostVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Post));
            await client.CallAsync(x => x.Post(request));
        }

        [TestMethod]
        public async Task PutVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Put));
            await client.CallAsync(x => x.Put(request));
        }

        [TestMethod]
        public async Task DeleteVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Delete));
            await client.CallAsync(x => x.Delete(request));
        }

        [TestMethod]
        public async Task GetVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Get));
            await client.CallAsync(x => x.Get(request));
        }

        [TestMethod]
        public async Task OptionsVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Options));
            await client.CallAsync(x => x.Options(request));
        }

        [TestMethod]
        public async Task PatchVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(new HttpMethod("PATCH")));
            await client.CallAsync(x => x.Patch(request));
        }

        [TestMethod]
        public async Task HeadVerbIsUsed()
        {
            var request = TestData.OfType<Request>();
            var client = TestData.ClientOf<IHttpVerbs>(x => x.RequireRequestUsesVerb(HttpMethod.Head));
            await client.CallAsync(x => x.Head(request));
        }
    }
}
