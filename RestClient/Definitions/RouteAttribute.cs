using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Definitions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RouteAttribute:Attribute
    {
        public string Path { get; private set; }

        public RouteAttribute(string path)
        {
            this.Path = path;
        }
    }
}
