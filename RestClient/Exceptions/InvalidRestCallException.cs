using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Exceptions
{
    public sealed class InvalidRestCallException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public InvalidRestCallException(HttpStatusCode status, string message) : base(message)
        {
            this.Status = status;
        }
    }
}
