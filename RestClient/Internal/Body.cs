using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal sealed class Body
    {
        public object Content { get; set; }
        public Encoding Encoding { get; set; } = Encoding.Unicode;
        public MediaType MediaType { get; set; } = MediaType.PlainText;
    }
}
