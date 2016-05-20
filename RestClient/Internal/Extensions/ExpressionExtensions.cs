using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal.Extensions
{
    static class ExpressionExtensions
    {
        //public static LinkedList<Expression> GetDereferenceChain(this Expression expression)
        //{

        //}

        public static object GetValueFromMember(this object instance, MemberInfo member)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Property: return ((PropertyInfo)member).GetValue(instance);
                case MemberTypes.Field: return ((FieldInfo)member).GetValue(instance);
                default:
                    throw new ArgumentException($"Unable to get value from member '{member.Name}' as it is not a property or a field");
            }
        }

        public static object GetValue(this Expression expression)
        {
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;

            throw new ArgumentException($"Unable to get value from a non-constant expression of type '{expression.NodeType}'");            
        }
    }
}
