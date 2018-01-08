Resource Server Implementation in C#
====================================

Overview
--------

This is a resource server implementation in C#. It supports a
[userinfo endpoint][1] defined in [OpenID Connect Core 1.0][2]
and includes an example of a protected resource endpoint that
accepts an access token in the ways defined in [RFC 6750][3]
(The OAuth 2.0 Authorization Framework: Bearer Token Usage).

This implementation is written using ASP.NET Core API and
[authlete-csharp][4] library which is provided as a NuGet
package [Authlete.Authlete][5].

To validate an access token presented by a client application,
this resource server makes an inquiry to the [Authlete][6] server.
This means that this resource server expects that the authorization
server which has issued the access token uses Authlete as a backend
service. [csharp-oauth-server][7] is such an authorization server
implementation and it supports [OAuth 2.0][8] and [OpenID Connect][9].


License
-------

  Apache License, Version 2.0


Source Code
-----------

  <code>https://github.com/authlete/csharp-resource-server</code>


About Authlete
--------------

[Authlete][6] is a cloud service that provides an implementation
of [OAuth 2.0][8] & [OpenID Connect][9]. By using the Web APIs
provided by Authlete, you can develop a _DB-less_ authorization
server and/or OpenID provider. "DB-less" here means that you
don't have to manage a database server that stores authorization
data (e.g. access tokens), settings of authorization servers and
settings of client applications. These data are stored in the
Authlete server on cloud.

Please read
*[New Architecture of OAuth 2.0 and OpenID Connect Implementation][10]*
for details about the architecture of Authlete. True engineers
will love the architecture ;-)

> The primary advantage of this architecture is in that the
> backend service can focus on implementing OAuth 2.0 and OpenID
> Connect without caring about other components such as identity
> management, user authentication, login session management, API
> management and fraud detection. And, consequently, it leads to
> another major advantage which enables the backend service
> (implementation of OAuth 2.0 and OpenID Connect) to be combined
> with any solution of other components and thus gives flexibility
> to frontend server implementations.


How To Run
----------

1. Download the source code of this resource server implementation.

        $ git clone https://github.com/authlete/csharp-resource-server.git
        $ cd csharp-resource-server/ResourceServer

2. Edit the configuration file to set the API credentials of yours.

        $ vi authlete.properties

3. Start the resource server on [http://localhost:5001/][11].

        $ dotnet run


#### Configuration File

`csharp-resource-server` refers to `authlete.properties` as a
configuration file. If you want to use another different file,
specify the name of the file by the environment variables
`AUTHLETE_CONFIGURATION_FILE` like the following.

    $ export AUTHLETE_CONFIGURATION_FILE=local.authlete.properties


Endpoints
---------

This implementation exposes endpoints as listed in the table below.

| Endpoint          | Path            |
|:------------------|:----------------|
| UserInfo Endpoint | `/api/userinfo` |
| Time Endpoint     | `/api/time`     |


#### UserInfo Endpoint

The userinfo endpoint is an implementation of the requirements
described in [5.3. UserInfo Endpoint][1] of [OpenID Connect
Core 1.0][2].

The endpoint accepts an access token as a Bearer Token. That is,
it accepts an access token via `Authorization: Bearer {access-token}`
or by a request parameter `access_token={access-token}`. See
[RFC 6750][3] for details.

The endpoint returns user information in JSON or [JWT][12] format,
depending on the configuration of the client application. If both
`userinfo_signed_response_alg` and `userinfo_encrypted_response_alg`
of the metadata of the client application are not specified, user
information is returned as a plain JSON. Otherwise, it is returned
as a serialized JWT. Authlete provides you with a Web console
([Developer Console][13]) to manage metadata of client applications.
As for metadata of client applications, see [2. Client Metadata][14]
in [OpenID Connect Dynamic Client Registration 1.0][15].

User information returned from the endpoint contains [claims][16]
of the user. In short, _claims_ are pieces of information about
the user such as a given name and an email address. Because Authlete
does not manage user data (although it supports OpenID Connect),
you have to provide claim values. It is achieved by implementing
`IUserInfoRequestHandlerSpi` interface.

In this resource server implementation, `UserInfoRequestHandlerSpiImpl`
is an example implementation of `IUserInfoRequestHandlerSpi` interface
and it retrieves claim values from a dummy database. You need to modify
the implementation to make it refer to your actual user database.


#### Time Endpoint

The time endpoint implemented in this resource server is just an
example of a protected resource endpoint. Its main purpose is to
show how to validate an access token at a protected resource
endpoint.

The path of the time endpoint is `/api/time`. Because the endpoint
supports all the three ways defined in [RFC 6750][3], you can pass
an access token to the endpoint by any means of the following.

```
# RFC 6750, 2.1. Authorization Request Header Field
$ curl -v http://localhost:5001/api/time \
       -H 'Authorization: Bearer {access_token}'
```

```
# RFC 6750, 2.2. Form-Encoded Body Parameter
$ curl -v http://localhost:5001/api/time \
       -d access_token={access_token}
```

```
# RFC 6750, 2.3. URI Query Parameter
$ curl -v http://localhost:5001/api/time\?access_token={access_token}
```

The time endpoint returns information about the current time (UTC)
in JSON format. The following is an example response.

```
{
  "year":        2018,
  "month":       1,
  "day":         8,
  "hour":        9,
  "minute":      46,
  "second":      10,
  "millisecond": 15
}
```

As for generic and Authlete-specific information regarding how to
protect Web APIs by OAuth 2.0 access tokens, see [Protected Resource][17]
in [Authlete Definitive Guide][18].


See Also
--------

- [Authlete][6] - Authlete Home Page
- [authlete-csharp][4] - Authlete Library for C#
- [csharp-oauth-server][7] - Authorization Server Implementation


Contact
-------

| Purpose   | Email Address        |
|:----------|:---------------------|
| General   | info@authlete.com    |
| Sales     | sales@authlete.com   |
| PR        | pr@authlete.com      |
| Technical | support@authlete.com |


[1]: https://openid.net/specs/openid-connect-core-1_0.html#UserInfo
[2]: https://openid.net/specs/openid-connect-core-1_0.html
[3]: https://tools.ietf.org/html/rfc6750
[4]: https://github.com/authlete/authlete-csharp
[5]: https://www.nuget.org/packages/Authlete.Authlete
[6]: https://www.authlete.com/
[7]: https://github.com/authlete/csharp-oauth-server
[8]: https://tools.ietf.org/html/rfc6749
[9]: https://openid.net/connect/
[10]: https://medium.com/@darutk/new-architecture-of-oauth-2-0-and-openid-connect-implementation-18f408f9338d
[11]: http://localhost:5001/
[12]: https://tools.ietf.org/html/rfc7519
[13]: https://www.authlete.com/documents/cd_console
[14]: https://openid.net/specs/openid-connect-registration-1_0.html#ClientMetadata
[15]: https://openid.net/specs/openid-connect-registration-1_0.html
[16]: https://openid.net/specs/openid-connect-core-1_0.html#Claims
[17]: https://www.authlete.com/documents/definitive_guide/protected_resource
[18]: https://www.authlete.com/documents/definitive_guide
