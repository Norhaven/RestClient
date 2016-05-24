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
    public sealed class Client<TInterface>:IRestClient<TInterface> where TInterface:class
    {
        private const string OpenVariable = "{";
        private const string CloseVariable = "}";
        private const string VariableDereferenceOperator = ".";
        private const string VariableCaptureName = "variable";

        private readonly Regex collectRoutePathVariables = new Regex($@"\{{(<{VariableCaptureName}>?.+)\}}");
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
        
        public Client(Uri baseUri, IHttpClient httpClient = null, IRestSerializer serializer = null)
        {
            this.baseUri = baseUri;
            this.httpClient = httpClient ?? new DefaultHttpClient();
            this.serializer = serializer ?? new DefaultSerializer();
        }

        public Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod) => CallAsync<TResult>(invokeRestMethod, CancellationToken.None);

        public Task<TResult> CallAsync<TResult>(Expression<Func<TInterface, TResult>> invokeRestMethod, CancellationToken cancellationToken)
        {
            var lambdaBody = invokeRestMethod.Body;

            if (lambdaBody.NodeType != ExpressionType.Call)
                throw new ArgumentException($"The lambda provided to {nameof(IRestClient<TInterface>.CallAsync)} must contain a single method call using the specified {typeof(TInterface).Name} type.", nameof(invokeRestMethod));

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

            var mediaType = body.MediaType.ToMediaTypeString();
            var content = this.serializer.Serialize(body.Content);

            request.Content = new StringContent(content, body.Encoding, mediaType);

            var response = await this.httpClient.SendAsync(request, cancellationToken);
            var status = response.StatusCode;

            if (status != HttpStatusCode.OK)
                throw new InvalidRestCallException(status, $"Received status '{status}' for REST call to {fullUri.AbsolutePath}");

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
