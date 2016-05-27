using RestClient.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Exceptions
{
    /// <summary>
    /// Represents a call to a REST endpoint that received an error response.
    /// </summary>
    public sealed class InvalidRestCallException : Exception
    {
        /// <summary>
        /// Gets the response message returned from the REST call.
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

        /// <summary>
        /// Gets the HTTP status returned from the REST call.
        /// </summary>
        public HttpStatusCode Status => this.Response.StatusCode;

        /// <summary>
        /// Gets the content of the HTTP response returned from the REST call.
        /// </summary>
        public string Content => this.Response.Content?.ReadAsStringAsync().Result;

        /// <summary>
        /// Initializes an instance of <see cref="InvalidRestCallException"/> with the given parameters.
        /// </summary>
        /// <param name="response">The HTTP response message returned from the REST call.</param>
        /// <param name="message">The exception message.</param>
        public InvalidRestCallException(HttpResponseMessage response, string message) : base(message)
        {
            response.ThrowIfNull(nameof(response), $"Unable to instantiate an exception of type '{nameof(InvalidRestCallException)}' because the HTTP response provided was null");
            this.Response = response;
        }
    }
}
