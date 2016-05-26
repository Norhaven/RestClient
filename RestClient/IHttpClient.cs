using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient
{
    /// <summary>
    /// Represents a client that can execute HTTP requests.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Sends an HTTP request to its destination and returns the response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An HTTP response that may be awaited.</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}