using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Definitions
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InBodyAttribute:Attribute
    {
        public Encoding Encoding { get; set; } = Encoding.Unicode;
        public MediaType MediaType { get; set; } = MediaType.ApplicationJson;
    }
}
