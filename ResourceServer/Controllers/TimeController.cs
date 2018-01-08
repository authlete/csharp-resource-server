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


using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Authlete.Api;
using Authlete.Web;


namespace ResourceServer.Controllers
{
    /// <summary>
    /// An example of a protected resource endpoint. This
    /// implementation returns JSON that contains information about
    /// the current time.
    /// </summary>
    [Route("api/[controller]")]
    public class TimeController : BaseController
    {
        public TimeController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// API entry point for HTTP GET method. The request must
        /// contain an access token.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// # Passing an access token in the way defined in
        /// # RFC 6750, 2.3. URI Query Parameter.
        ///
        /// $ curl -v http://localhost:5001/api/time\?access_token={access_token}
        /// </code>
        ///
        /// <code>
        /// # Passing an access token in the way defined in
        /// # RFC 6750, 2.1. Authorization Request Header Field.
        ///
        /// $ curl -v http://localhost:5001/api/time \
        ///        -H 'Authorization: Bearer {access_token}'
        /// </code>
        /// </example>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            return await Process();
        }


        /// <summary>
        /// API entry point for HTTP POST method. The request must
        /// contain an access token.
        /// </summary>
        ///
        /// <example>
        /// <code>
        /// # Passing an access token in the way defined in
        /// # RFC 6750, 2.2. Form-Encoded Body Parameter.
        ///
        /// $ curl -v http://localhost:5001/api/time \
        ///        -d access_token={access_token}
        /// </code>
        ///
        /// <code>
        /// # Passing an access token in the way defined in
        /// # RFC 6750, 2.1. Authorization Request Header Field.
        /// # '-X POST -d ""' is used to force curl to use POST.
        /// # Without '-d ""', '411 Length Required' is returned.
        ///
        /// $ curl -v http://localhost:5001/api/time \
        ///        -H 'Authorization: Bearer {access_token}' \
        ///        -X POST -d ""
        /// </code>
        /// </example>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            return await Process();
        }


        async Task<HttpResponseMessage> Process()
        {
            // Extract an access token from the request and
            // validate it. The instance of 'AccessTokenValidator'
            // returned from ValidateAccessToken() method holds the
            // result of the access token validation.
            //
            // Note that ValidateAccessToken() can optionally take
            // 'requiredScopes' and 'requiredSubject' parameters
            // although they are not used in this example.
            AccessTokenValidator validator = await ValidateAccessToken();

            // If the access token is not valid.
            if (validator.IsValid == false)
            {
                // When 'IsValid' returns false, 'ErrorResponse'
                // holds an error response that should be returned
                // to the client application. The response complies
                // with RFC 6750 (The OAuth 2.0 Authorization
                // Framework: Bearer Token Usage).
                //
                // You can refer to 'IntrospectionResult' or
                // 'IntrospectionError' for more information.
                return validator.ErrorResponse;
            }

            // The access token is valid, so it's okay for this
            // protected resource endpoint to return the requested
            // protected resource.

            // Generate a response specific to this protected
            // resource endpoint and return it to the client.
            return BuildResponse();
        }


        HttpResponseMessage BuildResponse()
        {
            // This simple example generates JSON that holds
            // information about the current time.

            // The current time in UTC.
            DateTime current = System.DateTime.UtcNow;

            // Build JSON manually.
            string json =
                "{\n" +
                $"  \"year\":        {current.Year},\n" +
                $"  \"month\":       {current.Month},\n" +
                $"  \"day\":         {current.Day},\n" +
                $"  \"hour\":        {current.Hour},\n" +
                $"  \"minute\":      {current.Minute},\n" +
                $"  \"second\":      {current.Second},\n" +
                $"  \"millisecond\": {current.Millisecond}\n" +
                "}\n";

            // "200 OK", "application/json;charset=UTF-8"
            return ResponseUtility.OkJson(json);
        }
    }
}
