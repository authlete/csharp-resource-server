//
// Copyright (C) 2018 Authlete, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific
// language governing permissions and limitations under the
// License.
//


using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Authlete.Api;
using Authlete.Util;
using Authlete.Web;


namespace ResourceServer.Controllers
{
    /// <summary>
    /// Base controller.
    /// </summary>
    public class BaseController : Controller
    {
        // "Bearer {access_token}" (RFC 6750, 2.1).
        static readonly Regex BEARER_PATTERN;


        static BaseController()
        {
            // Create a regular expression for "Bearer {access_token}".
            BEARER_PATTERN = new Regex(
                "^Bearer *(?<parameter>[^ ]+) *$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );
        }


        /// <summary>
        /// Constructor with an implementation of the
        /// <c>IAuthleteApi</c> interface. The given instance can
        /// be referred to as the value of the <c>API</c> property.
        /// </summary>
        public BaseController(IAuthleteApi api)
        {
            API = api;
        }


        /// <summary>
        /// The implementation of the <c>IAuthleteApi</c> interface
        /// that was given to the constructor.
        /// </summary>
        public IAuthleteApi API { get; }


        /// <summary>
        /// Get the value of a request HTTP header.
        /// </summary>
        ///
        /// <returns>
        /// The value of the specified HTTP header in the request.
        /// </returns>
        ///
        /// <param name="headerName">
        /// The name of an HTTP header.
        /// </param>
        public string GetRequestHeaderValue(string headerName)
        {
            StringValues values = Request.Headers[headerName];

            // Return the value of the first entry.
            return values.GetEnumerator().Current;
        }


        /// <summary>
        /// Get the value of a query parameter.
        /// </summary>
        public string GetRequestQueryValue(string parameterName)
        {
            if (Request.Query[parameterName].Count == 0)
            {
                return null;
            }

            return Request.Query[parameterName][0];
        }


        /// <summary>
        /// Get the value of a form parameter.
        /// </summary>
        public string GetRequestFormValue(string parameterName)
        {
            if (Request.Form[parameterName].Count == 0)
            {
                return null;
            }

            return Request.Form[parameterName][0];
        }


        /// <summary>
        /// Extract an access token from the request by the way
        /// defined in RFC 6750.
        /// </summary>
        public string ExtractAccessToken()
        {
            // The value of the 'Authorization' header.
            string authorization =
                GetRequestHeaderValue("Authorization");

            if (authorization != null)
            {
                // Check if the value of the 'Authorization' header
                // matches the pattern, "Bearer {access_token}".
                var match = BEARER_PATTERN.Match(authorization);

                // If it matched the pattern.
                if (match.Success)
                {
                    // RFC 6750, 2.1. Authorization Request Header Field
                    return match.Groups["parameter"].Value;
                }
            }

            // If the HTTP method is "POST".
            if (TextUtility.EqualsIgnoreCase(
                "POST", HttpContext.Request.Method))
            {
                // RFC 6750, 2.2. Form-Encoded Body Parameter
                return GetRequestFormValue("access_token");
            }
            else
            {
                // RFC 6750, 2.3. URI Query Parameter
                return GetRequestQueryValue("access_token");
            }
        }


        /// <summary>
        /// Validate the access token. This method extracts an
        /// access token from the request and then validates the
        /// access token by calling <c>Validate()</c> method of
        /// <c>Authlete.Web.AccessTokenValidator</c>.
        /// </summary>
        ///
        /// <returns>
        /// An instance of <c>AccessTokenValidator</c> that holds
        /// the result of the access token validation. See the
        /// API document of
        /// <c><a href="https://authlete.github.io/authlete-csharp/class_authlete_1_1_web_1_1_access_token_validator.html">AccessTokenValidator</a></c>
        /// for details as to how to use <c>AccessTokenValidator</c>.
        /// </returns>
        ///
        /// <param name="requiredScopes">
        /// Scopes that the access token should cover. If a
        /// non-null value is given to this parameter, Authlete's
        /// <c>/api/auth/introspection</c> API checks whether the
        /// access token covers all the required scopes.
        /// </param>
        ///
        /// <param name="requiredSubject">
        /// Subject (= unique identifier of an end-user) that the
        /// access token should be associated with. If a non-null
        /// value is given to this parameter, Authlete's
        /// <c>/api/auth/introspection</c> API checks whether the
        /// access token is associated with the required subject.
        /// </param>
        public async Task<AccessTokenValidator> ValidateAccessToken(
            string[] requiredScopes = null,
            string requiredSubject = null)
        {
            // Extract an access token from the request.
            string accessToken = ExtractAccessToken();

            // Create a validator to validate the access token.
            var validator = new AccessTokenValidator(API);

            // Validate the access token. As a result of this call,
            // some properties of 'validator' are set. For example,
            // the 'IsValid' property holds the validation result.
            await validator.Validate(
                accessToken, requiredScopes, requiredSubject);

            // Return the validator that holds the result of the
            // access token validation.
            return validator;
        }
    }
}
