using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    static class TestData
    {
        private const int testRandomSeed = 1;
        private const string alphanumericSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random(testRandomSeed);

        public static readonly Uri BaseUri = new Uri("http://localhost");

        public static T OfType<T>() where T :new()
        {
            var type = typeof(T);
            var instance = new T();

            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType == typeof(string))
                    property.SetValue(instance, RandomString());
            }

            return instance;
        }

        public static int RandomInt(int min = 0, int max = 10)
        {
            return random.Next(min, max);
        }

        public static string RandomString(int length = 10)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var randomIndex = random.Next(0, alphanumericSet.Length);
                var randomCharacter = alphanumericSet[randomIndex];
                builder.Append(randomCharacter);
            }

            return builder.ToString();
        }

        public static Mock<T> StrictMockOf<T>(params object[] parameters) where T :class => new Mock<T>(MockBehavior.Strict, parameters);

        public static Client<T> ClientOf<T>(Action<Mock<IHttpClient>> setup = null) where T : class
        {
            var httpClient = StrictMockOf<IHttpClient>();

            if (setup != null)
                setup(httpClient);

            return new Client<T>(BaseUri, httpClient.Object);
        }
        
        public static class Routes
        {
            public const string Empty = "/empty";
        }

        public static class Responses
        {
            public static HttpResponseMessage Success => new HttpResponseMessage(HttpStatusCode.OK).WithContentOf(string.Empty);
            public static HttpResponseMessage SuccessWith(string content) => Success.WithContentOf(content);
        }
    }
}
