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
    public sealed class InvalidRestCallException : Exception
    {
        public HttpResponseMessage Response { get; private set; }
        public HttpStatusCode Status => this.Response.StatusCode;

        public InvalidRestCallException(HttpResponseMessage response, string message) : base(message)
        {
            response.ThrowIfNull(nameof(response), $"Unable to instantiate an exception of type '{nameof(InvalidRestCallException)}' because the HTTP response provided was null");
            this.Response = response;
        }
    }
}
