using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal
{
    internal sealed class DefaultSerializer : IRestSerializer
    {
        T IRestSerializer.Deserialize<T>(string text)
        {
            using (var reader = new StringReader(text))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        string IRestSerializer.Serialize(object instance)
        {
            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, instance);

                return writer.ToString();
            }
        }
    }
}
