using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal.Extensions
{
    internal static class ValidationExtensions
    {
        public static bool IsDefaultValue<T>(this T instance) => EqualityComparer<T>.Default.Equals(instance, default(T));

        public static bool IsMissing(this string text) => string.IsNullOrWhiteSpace(text);
    }
}
