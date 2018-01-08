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


using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Authlete.Api;
using Authlete.Handler;
using ResourceServer.Spi;


namespace ResourceServer.Controllers
{
    /// <summary>
    /// An implementation of userinfo endpoint. See
    /// <a href="http://openid.net/specs/openid-connect-core-1_0.html#UserInfo">5.3.
    /// UserInfo Endpoint</a> of
    /// <a href="http://openid.net/specs/openid-connect-core-1_0.html">OpenID
    /// Connect Core 1.0</a>.
    /// </summary>
    [Route("api/[controller]")]
    public class UserinfoController : BaseController
    {
        public UserinfoController(IAuthleteApi api) : base(api)
        {
        }


        /// <summary>
        /// UserInfo endpoint for GET method.
        /// </summary>
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            // Handle the userinfo request.
            return await Handle();
        }


        /// <summary>
        /// UserInfo endpoint for POST method.
        /// </summary>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<HttpResponseMessage> Post()
        {
            // Handle the userinfo request.
            return await Handle();
        }


        /// <summary>
        /// Handle a userinfo request.
        /// </summary>
        ///
        /// <returns>
        /// An HTTP response that should be returned to the client
        /// application.
        /// </returns>
        async Task<HttpResponseMessage> Handle()
        {
            // Extract an access token from the request.
            string accessToken = ExtractAccessToken();

            // Handle the userinfo request.
            return await new UserInfoRequestHandler(
                API, new UserInfoRequestHandlerSpiImpl())
                    .Handle(accessToken);
        }
    }
}
