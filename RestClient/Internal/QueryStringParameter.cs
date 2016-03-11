using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal sealed class QueryStringParameter
    {
        public string Name { get; set; }
        public object Value { get; internal set; }
    }
}
