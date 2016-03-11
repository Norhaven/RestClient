using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal sealed class CallState
    {
        public HttpMethod HttpVerb { get; set; }
        public string Route { get; set; }
        public IList<Header> Headers { get; set; }
        public IList<QueryStringParameter> QueryString { get; set; }
    }
}
