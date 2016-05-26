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

        public static void ThrowIfNull(this object instance, string paramName, string message)
        {
            if (instance == null)
                throw new ArgumentNullException(paramName, message);
        }

        public static void ThrowIfNull<TException>(this object instance, string message) where TException:Exception
        {
            instance.ThrowIf<object, TException>(obj => obj == null, message);
        }

        private static void ThrowIf<T, TException>(this T instance, Predicate<T> isMatch, string message) where TException:Exception
        {
            if (isMatch(instance))
                throw (TException)Activator.CreateInstance(typeof(TException), message);
        }
    }
}
