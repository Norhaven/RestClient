using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    /// <summary>
    /// Represents a serializer for complex objects used in REST calls.
    /// </summary>
    public interface IRestSerializer
    {
        /// <summary>
        /// Serializes an object instance to a string.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A string containing the serialized data.</returns>
        string Serialize(object instance);

        /// <summary>
        /// Deserializes a string into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized instance.</typeparam>
        /// <param name="text">The serialized text.</param>
        /// <returns>An instance of <typeparamref name="T"/>"</returns>
        T Deserialize<T>(string text);
    }
}
