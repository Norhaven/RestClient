using Moq;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClient.Tests
{
    static class TestingExtensions
    {
        public static HttpResponseMessage WithContentOf(this HttpResponseMessage message, string content)
        {
            message.Content = new StringContent(content);
            return message;
        }

        public static Task<T> AsTask<T>(this T instance) => Task.FromResult(instance);

        public static Mock<IHttpClient> RequireHeaderOf(this Mock<IHttpClient> mock, string headerName, object value)
        {
            return mock.IfRequestIs(message => message.Headers.Contains(headerName) && message.Headers.GetValues(headerName).FirstOrDefault() == value?.ToString(), ReturnsEmptyString);
        }

        public static Mock<IHttpClient> RequireQueryStringOf(this Mock<IHttpClient> mock, string expectedQueryString)
        {
            return mock.IfRequestIs(message => message.RequestUri.Query == $"?{expectedQueryString}", ReturnsEmptyString);
        }

        public static Mock<IHttpClient> RequirePathOf(this Mock<IHttpClient> mock, string expectedPath)
        {
            return mock.IfRequestIs(message => message.RequestUri.AbsolutePath == expectedPath, ReturnsEmptyString);
        }

        public static Mock<IHttpClient> RequireRequestUsesVerb(this Mock<IHttpClient> mock, HttpMethod method)
        {
            return mock.IfRequestIs(message => message.Method == method, ReturnsEmptyString);
        }

        public static Mock<IHttpClient> IfRequestHasStringContentAndContentIs<T>(this Mock<IHttpClient> mock, Predicate<string> isMatch, Func<T> then)
        {
            return mock.IfRequestIs(message => message.Content.IsStringContent(content => isMatch(content)), then);
        }

        public static Mock<IHttpClient> IfRequestIs<T>(this Mock<IHttpClient> mock, Predicate<HttpRequestMessage> isMatch, Func<T> then)
        {
            var setup = mock.Setup(x => x.SendAsync(It.Is<HttpRequestMessage>(p => isMatch(p)), It.IsAny<CancellationToken>()));
            setup.ReturnsAsync(TestData.Responses.SuccessWith(then().Serialized()));
            return mock;
        }

        public static bool IsStringContent(this HttpContent content, Predicate<string> isMatch = null)
        {
            return content.Is<StringContent>(x =>
            {
                var stringContent = x.ReadAsStringAsync().Result;

                if (isMatch == null)
                    return true;
                return isMatch(stringContent);
            });
        }

        public static bool Is<T>(this HttpContent content, Predicate<T> isMatch = null)
        {
            var isOfType = content is T;

            if (!isOfType)
                return false;

            if (isMatch == null)
                return true;

            return isMatch((T)(object)content);
        }

        public static string Serialized<T>(this T instance) => ((IRestSerializer)new RestClient.Internal.DefaultSerializer()).Serialize(instance);

        public static bool IsEqualToSerialized<T>(this string text, T instance) => text == instance.Serialized();

        private static string ReturnsEmptyString() => string.Empty;  
    }
}
