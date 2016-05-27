# RestClient
An easy to use, declarative way of performing REST calls.

## How do I use this?

After adding a reference to the RestClient assembly, create a new interface. For this example, we'll assume that it's a service for things relating to accounts.

```C#
public interface IAccountService
{
}
```

In that file, add the following to your 'using' directives.

```C#
using RestClient.Definitions;
```

The interface you define will represent a particular set of REST endpoints available to be called. Each method on the interface will represent a single REST endpoint, with all of its associated data.

Let's say it's a method to get an account, because that's a thing people do.

```C#
Account GetAccount();
```

You'll want to add an attribute to the method that indicates the HTTP verb to use when calling it.

```C#
[Get]
Account GetAccount();
```

You'll also want to add a route so that the client knows where this endpoint is (relative to whatever base URI it's been given).

```C#
[Get]
[Route("api/accounts/single/{id}")]
Account GetAccount(int id);
```

We've included one of the method parameters in the route by enclosing its name in curly braces within the route. Whatever value is actually passed when the method is invoked will be added into the route. You may also access members on your parameters by using the dot notation in your route. For example, "{account.Id.Value}".

You may want to include additional information in the query string, the headers, the body, all of which can be performed by using attributes on the parameters in your method signature. Note that the InHeader and InQueryString attributes can optionally specify the name to use as the key, but if no name is supplied then the name of the method parameter will be used instead.

```C#
[Get]
[Route("api/accounts/single/{id}")]
Account GetAccount(int id,
                   [InHeader(Name="version")] string apiVersion,
                   [InQueryString(Name="active")] bool isActive,
                   [InBody] SpecialContents contents);
```

## Okay, so I did that. How do I make a REST call?

Instantiate an instance of `Client<TInterface>` and give it the base URI of the REST service you'll invoke. You can optionally provide your own implementation for handling the actual HTTP connection and the serialization, but if you don't then it will just use the defaults (currently HttpClient and JSON.Net).

```C#
var client = new Client<IAccountService>("http://localhost");
```

Now, use the client to perform the call with values like any other method.

```C#
var account = await client.CallAsync(x => x.GetAccount(5, 1, true, null));
```

This will package up the provided parameters according to your method signature, make the REST call, and give you back the deserialized Account instance they return to you.

Currently, values provided as parameters may be literals, variables, fields, or properties (with any additional dereferencing necessary). Some things, such as using the return value of a method or instantiating an object with the `new` keyword, are not supported at this time.

## I found a bug / I have a suggestion

You're more than welcome to head on over to the [Issues](https://github.com/Norhaven/RestClient/issues) page and submit an issue!
