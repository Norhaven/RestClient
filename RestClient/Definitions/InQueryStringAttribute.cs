using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Definitions
{
    [AttributeUsage(AttributeTargets.Parameter | 
                    AttributeTargets.Property | 
                    AttributeTargets.Field, 
                    AllowMultiple = false, 
                    Inherited = true)]
    public sealed class InQueryStringAttribute:Attribute, IIdentityDefinition
    {
        public string Name { get; set; }
    }
}
