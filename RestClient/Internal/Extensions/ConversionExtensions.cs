using RestClient.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Internal.Extensions
{
    internal static class ConversionExtensions
    {
        public static string ToMediaTypeString(this MediaType mediaType)
        {
            switch(mediaType)
            {
                case MediaType.PlainText: return "text/plain";
                case MediaType.ApplicationJson: return "application/json";
                default:
                    throw new ArgumentOutOfRangeException($"Unable to determine media type for unknown value '{mediaType}'", nameof(mediaType));
            }
        }
    }
}
