using RestClient.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal sealed class DefaultHttpClient : IHttpClient
    {
        async Task<HttpResponseMessage> IHttpClient.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.AssertNonNull("Expected the HttpRequestMessage instance to be non-null but found null");

            using (var http = new HttpClient())
            {
                return await http.SendAsync(request, cancellationToken);
            }
        }
    }
}
