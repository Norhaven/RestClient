using RestClient.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RestClient.Definitions;
using System.Threading;
using RestClient.Internal.Extensions;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using RestClient.Exceptions;
using System.Text.RegularExpressions;

namespace RestClient
{
    /// <summary>
    /// Represents a REST client that may be used to perform REST calls (as described by <typeparamref name="TInterface"/>).
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface that describes the possible REST calls.</typeparam>
    public sealed class Client<TInterface>:IRestClient<TInterface> where TInterface:class
    {
        private const string OpenVariable = "{";
        private const string CloseVariable = "}";
        private const string VariableDereferenceOperator = ".";
        private const string VariableCaptureName = "variable";

        private readonly Regex collectRoutePathVariables = new Regex($@"\{{(?<{VariableCaptureName}>.+?)\}}");
        private readonly Uri baseUri;
        private readonly IHttpClient httpClient;
        private readonly IRestSerializer serializer;

        private readonly IDictionary<Type, HttpMethod> httpMethodsByHttpVerbType = new Dictionary<Type, HttpMethod>
        {
            [typeof(DeleteAttribute)] = HttpMethod.Delete,
            [typeof(GetAttribute)] = HttpMethod.Get,
            [typeof(HeadAttribute)] = HttpMethod.Head,
            [typeof(OptionsAttribute)] = HttpMethod.Options,
            [typeof(PatchAttribute)] = new HttpMethod("PATCH"),
            [typeof(PutAttribute)] = HttpMethod.Put,
            [typeof(PostAttribute)] = HttpMethod.Post
        };
        
        /// <summary>
        /// Initializes an instance of <see cref="Client{TInterface}"/> with the given parameters.
        /// </summary>
        /// <param name="baseUri">The base URI that all routes on <typeparamref name="TInterface"/> will be executed against.</param>
        /// <param name="httpClient">An implementation of an HTTP client (if null, the default implementation will be used).</param>
        /// <param name="serializer">An implementation of a complex object serializer (if null, the default implementation will be used).</param>
        public Client(Uri baseUri, IHttpClient httpClient = null, IRestSerializer serializer = null)
        {
            this.baseUri = baseUri;
            this.httpClient = httpClient ?? new DefaultHttpClient();
            this.serializer = serializer ?? new DefaultSerializer();

            if (!typeof(TInterface).IsInterface)
                throw new InvalidOperationException($"The generic type parameter TInterface must correspond to an interface but the provided type {typeof(TInterface).Name} is not an interface");
        }

        /// <summary>
        /// Executes a call to the REST endpoint invoked on <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result expected from this operation.</typeparam>
        /// <param name="invokeRestMethod">An expression invoking a method on the <typeparamref name="TInterface"/> interface.</param>
        /// <returns>An instance of <typeparamref name="TResult"/> that may be awaited."</returns>
        public Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod) => CallAsync(invokeRestMethod, CancellationToken.None);

        /// <summary>
        /// Executes a call to the REST endpoint invoked on <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result expected from this operation.</typeparam>
        /// <param name="invokeRestMethod">An expression invoking a method on the <typeparamref name="TInterface"/> interface.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>An instance of <typeparamref name="TResult"/> that may be awaited."</returns>
        public Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod, CancellationToken cancellationToken)
        {
            var lambdaBody = invokeRestMethod.Body;

            if (lambdaBody.NodeType != ExpressionType.Call)
                throw new ArgumentException($"The expression provided to {nameof(IRestClient<TInterface>.CallAsync)} must contain a single method call using the specified {typeof(TInterface).Name} type.", nameof(invokeRestMethod));

            var methodCall = (MethodCallExpression)lambdaBody;
            var method = methodCall.Method;
            var formalParameters = methodCall.Method.GetParameters();
            var actualParameters = methodCall.Arguments.ToArray();

            var parameters = formalParameters.AsParameterDefinition(actualParameters);

            var httpVerb = GetHttpVerbFrom(method);
            var route = GetRouteFrom(method, parameters);
            var headers = GetHeadersFrom(parameters);
            var queryString = GetQueryStringFrom(parameters);
            var body = GetBodyFrom(method, parameters);

            var fullUrl = CreateFullUri(this.baseUri, route, queryString);

            return CallAsync<TResult>(httpVerb, fullUrl, headers, body, cancellationToken);
        }

        private Uri CreateFullUri(Uri baseUri, string route, IEnumerable<QueryStringParameter> queryString)
        {
            var formattedParameters = from parameter in queryString select $"{parameter.Name}={parameter.Value}";
            var formattedQueryString = string.Join("&", formattedParameters);

            if (formattedQueryString.IsMissing())
                return new Uri(baseUri, route);

            var pathAndQuery = $"{route}?{formattedQueryString}";
            return new Uri(baseUri, pathAndQuery);
        }

        private async Task<TResult> CallAsync<TResult>(HttpMethod httpVerb, Uri fullUri, IEnumerable<Header> headers, Body body, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(httpVerb, fullUri);

            foreach (var header in headers)
            {
                request.Headers.Add(header.Name, header.Value.ToString());
            }

            if (body != null)
            {
                var mediaType = body.MediaType.ToMediaTypeString();
                var content = this.serializer.Serialize(body.Content);

                request.Content = new StringContent(content, body.Encoding, mediaType);
            }

            var response = await this.httpClient.SendAsync(request, cancellationToken);
            var status = response.StatusCode;

            if (status != HttpStatusCode.OK)
                throw new InvalidRestCallException(response, $"Received status '{status}' for REST call to {fullUri.AbsoluteUri}");

            var responseContent = await response.Content.ReadAsStringAsync();
                        
            return this.serializer.Deserialize<TResult>(responseContent);
        }

        private Body GetBodyFrom(MethodInfo method, IEnumerable<ParameterDefinition> parameters)
        {
            var bodyParameters = EvaluateParametersFor<InBodyAttribute>(parameters);
            var allBodyParameters = bodyParameters.ToArray();

            if (allBodyParameters.Length == 0)
                return null;
            
            if (allBodyParameters.Length > 1)
                throw new ArgumentException($"Multiple parameters in REST interface method {method.GetTypeQualifiedMethodName()} contain an {nameof(InBodyAttribute)}, only one object may be included in the request body at a time", nameof(method));
            
            var body = allBodyParameters[0];
           
            return new Body
            {
                Content = body.GetParameterValue(),
                Encoding = body.Definition.Encoding,
                MediaType = body.Definition.MediaType
            };
        }

        private IEnumerable<Header> GetHeadersFrom(IEnumerable<ParameterDefinition> parameters)
        {
            var headerParameters = EvaluateParametersFor<InHeaderAttribute>(parameters);

            return from parameter in headerParameters
                   let name = parameter.GetParameterName()
                   let value = parameter.GetParameterValue()
                   select new Header
                   {
                       Name = name,
                       Value = value
                   };    
        }

        private IEnumerable<QueryStringParameter> GetQueryStringFrom(IEnumerable<ParameterDefinition> parameters)
        {
            var queryStringParameters = EvaluateParametersFor<InQueryStringAttribute>(parameters);

            return from parameter in queryStringParameters
                   let name = parameter.GetParameterName()
                   let value = parameter.GetParameterValue()
                   where value != null
                   select new QueryStringParameter
                   {
                       Name = name,
                       Value = value
                   };
        }

        private string GetRouteFrom(MethodInfo method, IEnumerable<ParameterDefinition> parameters)
        {
            var routeTemplate = method.GetCustomAttribute<RouteAttribute>();

            if (routeTemplate == null)
                throw new ArgumentException($"Unable to create an HTTP route for REST interface method {method.GetTypeQualifiedMethodName()}, did you forget to add a {nameof(RouteAttribute)}?", nameof(method));

            var path = routeTemplate.Path;

            if (path.IsMissing())
                throw new ArgumentException($"Unable to create an HTTP route for REST interface method {method.GetTypeQualifiedMethodName()} with a null, empty, or whitespace path");

            var pathVariables = ReadPathVariables(path);
            var parametersByName = parameters.ToDictionary(x => x.FormalParameter.Name);

            foreach (var variable in pathVariables)
            {
                var references = variable.Split(new[] { VariableDereferenceOperator }, StringSplitOptions.RemoveEmptyEntries);
                var parameterReference = references[0];

                if (!parametersByName.ContainsKey(parameterReference))
                    throw new ArgumentException($"Unknown variable '{parameterReference}' found in HTTP route path for REST interface method {method.GetTypeQualifiedMethodName()}");

                var parameter = parametersByName[parameterReference];
                var currentReference = parameter.ActualParameter.GetValue();

                for(var i = 1; i < references.Length; i++)
                {
                    var memberName = references[i];
                    var member = currentReference.GetType().GetMember(memberName).FirstOrDefault();

                    if (member == null)
                        throw new ArgumentException($"Could not dereference unknown member '{memberName}' in HTTP route path variable {variable} for REST interface method {method.GetTypeQualifiedMethodName()}");

                    currentReference = currentReference.GetValueFromMember(member);
                }

                path = path.Replace($"{OpenVariable}{variable}{CloseVariable}", currentReference.ToString());
            }

            return path;
        }

        private HttpMethod GetHttpVerbFrom(MethodInfo method)
        {
            var verb = method.GetCustomAttribute<HttpVerbAttribute>(inherit: true);

            if (verb == null)
                throw new ArgumentException($"Unable to determine HTTP verb for REST interface method {method.GetTypeQualifiedMethodName()}, did you forget to add an {nameof(HttpVerbAttribute)}?", nameof(method));

            var verbType = verb.GetType();

            if (!this.httpMethodsByHttpVerbType.ContainsKey(verbType))
                throw new ArgumentOutOfRangeException(nameof(method), $"Unable to convert attribute of type {verbType.Name} into a recognized HTTP verb, did you forget to use the attributes in RestClient.Attributes?");

            return this.httpMethodsByHttpVerbType[verbType];
        }
                
        private IEnumerable<ParameterDefinition<TAttribute>> EvaluateParametersFor<TAttribute>(IEnumerable<ParameterDefinition> parameters) where TAttribute : Attribute
        {
            foreach (var parameter in parameters)
            { 
                var attribute = parameter.FormalParameter.GetCustomAttribute<TAttribute>(inherit: true);

                if (attribute == null)
                    continue;

                yield return new ParameterDefinition<TAttribute>
                {
                    Definition = attribute,
                    ActualParameter = parameter.ActualParameter,
                    FormalParameter = parameter.FormalParameter
                };
            }
        }

        private IEnumerable<string> ReadPathVariables(string path)
        {
            var pathVariables = this.collectRoutePathVariables.Matches(path);

            foreach (Match variable in pathVariables)
            {
                var capture = variable.Groups[VariableCaptureName];

                if (capture == null)
                    continue;

                yield return capture.Value;
            }
        }
    }
}
