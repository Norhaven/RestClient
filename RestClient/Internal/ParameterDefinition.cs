using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal class ParameterDefinition
    {
        public ParameterInfo FormalParameter { get; set; }
        public Expression ActualParameter { get; set; }
    }

    internal sealed class ParameterDefinition<TAttribute> : ParameterDefinition
        where TAttribute:Attribute
    {
        public TAttribute Definition { get; set; }
    }
}
