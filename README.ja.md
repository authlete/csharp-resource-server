リソースサーバー実装 (C#)
=========================

概要
----

これはリソースサーバーの C# 実装です。 [OpenID Connect Core 1.0][2]
で定義されている[ユーザー情報エンドポイント][1]をサポートし、また、[RFC 6750][3]
(The OAuth 2.0 Authorization Framework: Bearer Token Usage)
に定義されている方法でアクセストークンを受け取る保護リソースエンドポイントの例も含んでいます。

この実装は ASP.NET Core API と、NuGet パッケージ [Authlete.Authlete][5]
として提供される [authlete-csharp][4] ライブラリを用いて書かれています。

クライアントアプリケーションが提示したアクセストークンの有効性を調べるため、
このリソースサーバーは [Authlete][6] サーバーに問い合わせをおこないます。
これはつまり、このリソースサーバーは、アクセストークンを発行した認可サーバーが
Authlete をバックエンドサービスとして使用していることを期待していることを意味します。
[csharp-oauth-server][7] はそのような認可サーバーの実装であり、[OAuth 2.0][8] と
[OpenID Connect][9] をサポートしています。


ライセンス
----------

  Apache License, Version 2.0


ソースコード
------------

  <code>https://github.com/authlete/csharp-resource-server</code>


Authlete について
-----------------

[Authlete][6] は [OAuth 2.0][8] と [OpenID Connect][9]
の実装を提供するクラウドサービスです。
Authlete が提供する Web API を使い、DB-less (データベース無し)
の認可サーバーを構築することができます。
「DB-less」とは、認可データ (アクセストークン等)、
認可サーバーの設定、クライアントアプリケーションの設定を保存するデータベースを管理する必要が無い、という意味です。
これらのデータはクラウド上にある Authlete サーバーに保存されます。

Authlete のアーキテクチャーの詳細については、
*[New Architecture of OAuth 2.0 and OpenID Connect Implementation][10]*
をお読みください。真のエンジニアであれば、このアーキテクチャーを気に入ってくれるはずです ;-)
なお、日本語版は「[OAuth 2.0 / OIDC 実装の新アーキテクチャー][19]」です。

> The primary advantage of this architecture is in that the
> backend service can focus on implementing OAuth 2.0 and OpenID
> Connect without caring about other components such as identity
> management, user authentication, login session management, API
> management and fraud detection. And, consequently, it leads to
> another major advantage which enables the backend service
> (implementation of OAuth 2.0 and OpenID Connect) to be combined
> with any solution of other components and thus gives flexibility
> to frontend server implementations.
>
> このアーキテクチャーの一番の利点は、アイデンティティー管理やユーザー認証、
> ログインセッション管理、API 管理、不正検出などの機能について気にすることなく、
> バックエンドサービスが OAuth 2.0 と OpenID Connect の実装に集中できることにあります。
> この帰結として、バックエンドサービス (OAuth 2.0 と OpenID Connect の実装)
> をどのような技術部品とも組み合わせることが可能というもう一つの大きな利点が得られ、
> フロントエンドサーバーの実装に柔軟性がもたらされます。


実行方法
--------

1. このリソースサーバーの実装をダウンロードします。

        $ git clone https://github.com/authlete/csharp-resource-server.git
        $ cd csharp-resource-server/ResourceServer

2. 設定ファイルを編集して API クレデンシャルズをセットします。

        $ vi authlete.properties

3. [http://localhost:5001/][11] でリソースサーバーを起動します。

        $ dotnet run


#### 設定ファイル

`csharp-resource-server` は `authlete.properties` を設定ファイルとして参照します。
他のファイルを使用したい場合は、次のようにそのファイルの名前を環境変数
`AUTHLETE_CONFIGURATION_FILE` で指定してください。

    $ export AUTHLETE_CONFIGURATION_FILE=local.authlete.properties


エンドポイント
--------------

この実装は、下表に示すエンドポイントを公開します。

| エンドポイント             | パス            |
|:---------------------------|:----------------|
| ユーザー情報エンドポイント | `/api/userinfo` |
| 時刻エンドポイント         | `/api/time`     |


#### ユーザー情報エンドポイント

ユーザー情報エンドポイントは、[OpenID Connect Core 1.0][2] の
[5.3. UserInfo Endpoint][1] に記述されている要求事項を実装したものです。

このエンドポイントは、アクセストークンを Bearer Token として受け取ります。
つまり、`Authorization: Bearer {アクセストークン}`
を介して、もしくはリクエストパラメーター `access_token={アクセストークン}`
によりアクセストークンを受け取ります。 詳細は [RFC 6750][3] を参照してください。

このエンドポイントは、クライアントアプリケーションの設定に応じて、ユーザー情報を
JSON 形式もしくは [JWT][12] 形式で返します。 クライアントアプリケーションのメタデータの
`userinfo_signed_response_alg` と `userinfo_encrypted_response_alg`
の両方とも指定されていなければ、ユーザー情報は素の JSON で返されます。
そうでない場合は、シリアライズされた JWT で返されます。 Authlete
はクライアントアプリケーションのメタデータを管理するための Web コンソール
([デベロッパー・コンソール][13]) を提供しています。
クライアントアプリケーションのメタデータについては、
[OpenID Connect Dynamic Client Registration 1.0][15] の
[2. Client Metadata][14] を参照してください。

エンドポイントから返されるユーザー情報には、ユーザーの[クレーム][16]が含まれています。
手短に言うと、_クレーム_とは、名前やメールアドレスなどの、ユーザーに関する情報です。
Authlete は (OpenID Connect をサポートしているにもかかわらず)
ユーザーデータを管理しないので、あなたがクレーム値を提供しなければなりません。
これは、`IUserInfoRequestHandlerSpi` インターフェースを実装することでおこないます。

このリソースサーバーの実装では、`UserInfoRequestHandlerSpiImpl` が `IUserInfoRequestHandlerSpi`
インターフェースの実装で、ダミーデータベースからクレーム値を取り出しています。
実際のユーザーデータベースを参照するよう、この実装を変更する必要があります。


#### 時刻エンドポイント

このリソースサーバーに実装されているカントリーエンドポイントは、
保護リソースエンドポイントの一例に過ぎません。
主な目的は、保護リソースエンドポイントにおけるアクセストークンの有効性の確認方法を示すことです。

時刻エンドポイントのパスは `/api/time` です。
このエンドポイントは [RFC 6750][3] で定義されている
3 つの方法を全てサポートするので、次のいずれの方法でもアクセストークンを渡すことができます。

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

時刻エンドポイントは、現在時刻 (UTC) に関する情報を JSON で返します。
下記はレスポンスの例です。

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

Web API を OAuth 2.0 のアクセストークンで保護する方法に関する一般的な情報および
Authlete 固有の情報については、[Authlete Definitive Guide][18] の
[Protected Resource][17] を参照してください。


その他の情報
------------

- [Authlete][6] - Authlete ホームページ
- [authlete-csharp][4] - C# 用 Authlete ライブラリ
- [csharp-oauth-server][7] - 認可サーバーの実装


コンタクト
----------

| 目的 | メールアドレス       |
|:-----|:---------------------|
| 一般 | info@authlete.com    |
| 営業 | sales@authlete.com   |
| 広報 | pr@authlete.com      |
| 技術 | support@authlete.com |


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
[19]: https://qiita.com/TakahikoKawasaki/items/b2a4fc39e0c1a1949aab
