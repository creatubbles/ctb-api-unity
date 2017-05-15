//
//  CreatubblesApiClient.cs
//  Creatubbles API Client Unity SDK
//
//  Copyright (c) 2017 Creatubbles Pte. Ltd.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
using System;
using System.Collections;
using UnityEngine;
using Creatubbles.Api.Data;
using Creatubbles.Api.Storage;
using Creatubbles.Api.Requests;
using Creatubbles.Api.Helpers;

namespace Creatubbles.Api
{
    /// <summary>
    /// This is the main class of Creatubbles API Client Unity SDK used for creating and sending Creatubbles API requests.
    /// </summary>
    /// <remarks>
    /// Official documentation at https://stateoftheart.creatubbles.com/api/.
    /// </remarks>
    public class CreatubblesApiClient
    {
        private const string AppAccessTokenKey = "ctb_app_access_token";
        private const string UserAccessTokenKey = "ctb_user_access_token";

        public ICoroutineStarter coroutineStarter { get; private set; }

        private IApiConfiguration configuration;
        private ISecureStorage secureStorage;

        private string BaseUrl { get { return configuration.BaseUrl + "/" + configuration.ApiVersion; } }

        public CreatubblesApiClient(IApiConfiguration configuration, ISecureStorage secureStorage, ICoroutineStarter coroutineStarter)
        {
            this.configuration = configuration;
            this.secureStorage = secureStorage;
            this.coroutineStarter = coroutineStarter;
        }

        /// <summary>
        /// Sets the user (private) authentication token to be used for authenticating requests.
        /// </summary>
        /// <param name="token">User (private) access token.</param>
        public void SetAuthenticationToken(string accessToken)
        {
            secureStorage.SaveValue(UserAccessTokenKey, accessToken);
        }

        /// <summary>
        /// Authenticates application (public authentication) with Creatubbles.
        /// </summary>
        /// <param name="request">Request for authenticating application with Creatubbles (public authentication).</param>
        public IEnumerator AuthenticatePublicOAuth(GetPublicOAuthTokenRequest request)
        {
            request.AddField("client_id", configuration.AppId);
            request.AddField("client_secret", configuration.AppSecret);

            yield return coroutineStarter.StartCoroutine(Send(request));

            if (request.IsError)
            {
                yield break;
            }

            secureStorage.SaveValue(AppAccessTokenKey, request.ParsedResponse.token);
        }

        /// <summary>
        /// Sends specified request to the API.
        /// </summary>
        /// <param name="request">Request to be sent. Can be inspected after being sent for result.</param>
        public IEnumerator Send(Request request)
        {
            var url = request.AbsoluteUrl != null ? request.AbsoluteUrl : BaseUrl + request.Path;

            AuthorizeRequest(request);
            SetAcceptLanguageHeader(request);
            SetAcceptJsonHeader(request);
            if (request.Method != HttpMethod.POST)
            {
                SetMethodOverrideHeader(request);
                SetFormDummyData(request);
            }

            var shouldSendHeaders = request.HasAnyHeaders() && !request.IgnoreHeaders;
            WWW www =  shouldSendHeaders ? new WWW(url, request.Form.data, new Hashtable(request.Headers)) : new WWW(url, request.Form);
            request.SetBaseRequest(www);

            yield return www;

            request.HandleResponse();
        }

        private void AuthorizeRequest(Request request)
        {
            switch (request.Authorization)
            {
                case Request.AuthorizationType.Private:
                    if (secureStorage.HasValue(UserAccessTokenKey))
                    {
                        var accessToken = secureStorage.LoadValue(UserAccessTokenKey);
                        SetAuthorizationBearerTokenHeader(request, accessToken);
                    }
                    break;
                case Request.AuthorizationType.Public:
                    if (secureStorage.HasValue(AppAccessTokenKey))
                    {
                        var accessToken = secureStorage.LoadValue(AppAccessTokenKey);
                        SetAuthorizationBearerTokenHeader(request, accessToken);
                    }
                    break;
                case Request.AuthorizationType.None:
                    break;
            }
        }

        private void SetAuthorizationBearerTokenHeader(Request request, string accessToken)
        {
            request.SetHeader("Authorization", "Bearer " + accessToken);
        }

        private void SetAcceptLanguageHeader(Request request)
        {
            request.SetHeader("Accept-Language", configuration.Locale);
        }

        private void SetMethodOverrideHeader(Request request)
        {
            request.SetHeader("X-HTTP-Method-Override", request.Method.ToString());
        }

        private void SetAcceptJsonHeader(Request request)
        {
            request.SetHeader("Accept", "application/vnd.api+json");
        }

        private void SetFormDummyData(Request request)
        {
            request.AddField("dummy", "dummy_data_for_method_override");
        }
    }
}

