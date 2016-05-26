using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal.Extensions
{
    internal static class AssertionExtensions
    {
        private const string IsDebug = "DEBUG";

        [Conditional(IsDebug)]
        public static void AssertNonNull<T>(this T instance, string message)
        {
            Debug.Assert(!instance.IsDefaultValue(), message);
        }

        [Conditional(IsDebug)]
        public static void AssertEqualTo<T>(this T instance, T otherInstance)
        {
            var leftIsDefault = instance.IsDefaultValue();
            var rightIsDefault = otherInstance.IsDefaultValue();

            if (leftIsDefault && rightIsDefault)
                return;

            if (leftIsDefault || rightIsDefault)
                Debug.Fail("Expected instances to be equal but found a default value and an instance");
            else
                Debug.Assert(instance.Equals(otherInstance), "Expected instances to be equal but found an inequality");               
        }
    }
}
