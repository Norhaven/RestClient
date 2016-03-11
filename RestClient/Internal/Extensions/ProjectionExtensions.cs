using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal.Extensions
{
    internal static class ProjectionExtensions
    {
        public static string GetParameterName<TAttribute>(this ParameterDefinition<TAttribute> parameter) where TAttribute:Attribute, IIdentityDefinition
        {
            if (parameter.Definition.Name.IsMissing())
                return parameter.FormalParameter.Name;

            return parameter.Definition.Name;
        }

        public static object GetParameterValue<TAttribute>(this ParameterDefinition<TAttribute> parameter) where TAttribute:Attribute
        {
            return parameter.ActualParameter.GetValue();
        }

        public static string GetTypeQualifiedMethodName(this MethodInfo method)
        {
            return $"{method.ReflectedType.Name}.{method.Name}";
        }

        public static IEnumerable<ParameterDefinition> AsParameterDefinition(this IEnumerable<ParameterInfo> formalParameters, IEnumerable<Expression> actualParameters)
        {
            var allFormalParameters = formalParameters.ToArray();
            var allActualParameters = actualParameters.ToArray();

            Debug.Assert(allFormalParameters.Length == allActualParameters.Length, $"Parameter count mismatch, found {allFormalParameters.Length} formal parameters and {allActualParameters.Length} actual parameters");

            for(var i = 0; i < allFormalParameters.Length; i++)
            {
                var formal = allFormalParameters[i];
                var actual = allActualParameters[i];

                yield return new ParameterDefinition { FormalParameter = formal, ActualParameter = actual };
            }
        }
    }
}
