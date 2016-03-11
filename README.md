# RestClient
An easy to use, declarative way of performing REST calls.

## How do I use this?

After adding a reference to the RestClient assembly, create a new interface.

```
public interface IAccountService
{
}
```

In that file, add the following to your 'using' directives.

```
using RestClient.Definitions;
```

The interface you define will represent a particular set of REST endpoints available to be called. Each method on the interface will represent a single REST endpoint, with all of its associated data.

Let's say it's a method to get an account, because that's a thing people do.

```
Account GetAccount();
```

You'll want to add an attribute to the method that indicates the HTTP verb to use when calling it.

```
[HttpGet]
Account GetAccount();
```

You'll also want to add a route so that the client knows where this endpoint is (relative to whatever base URI it's been given).

```
[HttpGet]
[Route("api/accounts/single/{id}")]
Account GetAccount(int id);
```

We've included one of the parameters in the route by enclosing its name in curly braces within the route. Whatever value is actually passed when the method is invoked will be added into the route. You may also access members on your parameters by using the dot notation in your route. For example, "{account.Id.Value}".

You may want to include additional information in the query string, the headers, the body, all of which can be performed by using attributes on the parameters in your method signature.

```
[HttpGet]
[Route("api/accounts/single/{id}")]
Account GetAccount(int id,
                   [InHeader(Name="version")] string apiVersion,
                   [InQueryString(Name="active")] bool isActive,
                   [InBody] SpecialContents contents);
```

## Okay, so I did that. How do I make a REST call?

Instantiate an instance of Client<TInterface> and give it the base URI of the REST service you'll invoke. You can optionally provide your own implementation for handling the actual HTTP connection and the serialization, but if you don't then it will just use the defaults (HttpClient and JSON.Net).

```
var client = new Client<IAccountService>("http://localhost");
```

Now, just use the client.

```
var account = await client.CallAsync(x => x.GetAccount(5, 1, true, null));
```

This will package up the provided parameters according to your method signature, make the REST call, and give you back the deserialized Account instance they return to you.
