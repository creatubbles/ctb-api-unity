//
//  CreatubblesApiClient.cs
//  CreatubblesApiClient
//
//  Copyright (c) 2016 Creatubbles Pte. Ltd.
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

using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Creatubbles.Api
{
    public class CreatubblesApiClient
    {
        private const string AppAccessTokenKey = "ctb_app_access_token";
        private const string UserAccessTokenKey = "ctb_user_access_token";

        private IApiConfiguration configuration;
        private ISecureStorage secureStorage;

        public CreatubblesApiClient(IApiConfiguration configuration, ISecureStorage secureStorage)
        {
            this.configuration = configuration;
            this.secureStorage = secureStorage;
        }

        #region Methods for sending requests and managing user session

        // sends log in request and saves the token to ISecureStorage instance if successful
        public IEnumerator SendLogInRequest(OAuthRequest request)
        {
            SetAcceptLanguageHeader(request);

            yield return request.Send();

            if (request.IsAnyError || request.Data == null)
            {
                yield break;
            }

            secureStorage.SaveValue(UserAccessTokenKey, request.Data.access_token);
            Debug.Log("logged in and saved token: " + request.Data.access_token);
        }

        // sends the request with authorization header set based on requestType
        public IEnumerator SendRequest(HttpRequest request)
        {
            SetAcceptLanguageHeader(request);

            switch (request.requestType)
            {
                case HttpRequest.Type.Private:
                    yield return SendPrivateRequest(request);
                    break;
                case HttpRequest.Type.Public:
                    yield return SendPublicRequest(request);
                    break;
                case HttpRequest.Type.Regular:
                    yield return SendRegularRequest(request);
                    break;
                default:
                    break;
            }
        }

        // attempts to send request with user OAuth token if it's present in ISecureStorage instance
        private IEnumerator SendPrivateRequest(HttpRequest request)
        {
            string accessToken = null;
            if (secureStorage.HasValue(UserAccessTokenKey))
            {
                accessToken = secureStorage.LoadValue(UserAccessTokenKey);
                SetAuthorizationHeaderBearerToken(request, accessToken);
            }

            yield return request.Send();
        }

        // attempts to send request with application OAuth token if it's present in ISecureStorage instance
        private IEnumerator SendPublicRequest(HttpRequest request)
        {
            string accessToken = null;
            if (secureStorage.HasValue(AppAccessTokenKey))
            {
                accessToken = secureStorage.LoadValue(AppAccessTokenKey);
            }
            else
            {
                OAuthRequest tokenRequest = CreatePostAuthenticationApplicationTokenRequest();

                yield return tokenRequest.Send();

                if (!tokenRequest.IsAnyError)
                {
                    accessToken = tokenRequest.Data.access_token;
                    secureStorage.SaveValue(AppAccessTokenKey, accessToken);
                }
            }

            SetAuthorizationHeaderBearerToken(request, accessToken);

            yield return request.Send();
        }

        // sends unmodified request
        private IEnumerator SendRegularRequest(HttpRequest request)
        {
            yield return request.Send();
        }

        // removes user token from secure storage
        public void LogOut()
        {
            if (secureStorage.HasValue(UserAccessTokenKey))
            {
                secureStorage.DeleteValue(UserAccessTokenKey);
            }
        }

        // returns true if user token is present in secure storage
        public bool IsUserLoggedIn()
        {
            return secureStorage.HasValue(UserAccessTokenKey);
        }

        #endregion

        #region Requests factory methods

        private OAuthRequest CreatePostAuthenticationApplicationTokenRequest()
        {
            string url = RequestUrl("/oauth/token");
            WWWForm data = new WWWForm();
            data.AddField("grant_type", "client_credentials");
            data.AddField("client_id", configuration.AppId);
            data.AddField("client_secret", configuration.AppSecret);

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new OAuthRequest(request);
        }

        public OAuthRequest CreatePostAuthenticationUserTokenRequest(string username, string password)
        {
            string url = RequestUrl("/oauth/token");
            WWWForm data = new WWWForm();
            data.AddField("grant_type", "password");
            data.AddField("client_id", configuration.AppId);
            data.AddField("client_secret", configuration.AppSecret);
            data.AddField("username", username);
            data.AddField("password", password);

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new OAuthRequest(request);
        }

        public ApiRequest<LandingUrlsResponse> CreateGetLandingUrlsRequest()
        {
            string url = RequestUrl("/landing_urls");
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<LandingUrlsResponse>(request, HttpRequest.Type.Public);
        }

        public ApiRequest<LoggedInUserResponse> CreateGetLoggedInUserRequest()
        {
            string url = RequestUrl("/users/me");
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<LoggedInUserResponse>(request, HttpRequest.Type.Private);
        }

        public ApiRequest<CreationGetResponse> CreateNewCreationRequest(NewCreationData creationData)
        {
            string url = RequestUrl("/creations");
            WWWForm data = new WWWForm();
            if (creationData.name != null) 
            { 
                data.AddField("name", creationData.name); 
            }
            if (creationData.creatorIds != null) 
            { 
                string creatorIds = String.Join(",", creationData.creatorIds);
                data.AddField("creator_ids", creatorIds); 
            }
            if (creationData.HasCreationMonth) 
            {
                data.AddField("created_at_month", creationData.creationMonth);
            }
            if (creationData.HasCreationYear)
            {
                data.AddField("created_at_year", creationData.creationYear);
            }
            if (creationData.reflectionText != null)
            {
                data.AddField("reflection_text", creationData.reflectionText);
            }
            if (creationData.reflectionVideoUrl != null)
            {
                data.AddField("reflection_video_url", creationData.reflectionVideoUrl);
            }

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<CreationGetResponse>(request, HttpRequest.Type.Private);
        }

        public ApiRequest<CreationGetResponse> CreateGetCreationRequest(string creationId)
        {
            string url = RequestUrl("/creations/" + creationId);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<CreationGetResponse>(request, HttpRequest.Type.Private);
        }

        public ApiRequest<CreationsUploadPostResponse> CreatePostCreationUploadRequest(string creationId, UploadExtension extension)
        {
            string url = RequestUrl("/creations/" + creationId + "/uploads");
            WWWForm data = new WWWForm();
            data.AddField("extension", extension.StringValue());

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<CreationsUploadPostResponse>(request, HttpRequest.Type.Private);
        }

        public HttpRequest CreatePutUploadFileRequest(string url, string contentType, byte[] data)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, data);
            request.SetRequestHeader("Content-Type", contentType);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new HttpRequest(request, HttpRequest.Type.Regular);
        }

        // API doc: https://stateoftheart.creatubbles.com/api/#update-creation-upload
        public HttpRequest CreatePutUploadFinishedRequest(string pingUrl, string abortedWithMessage = null)
        {
            UnityWebRequest request;
            if (abortedWithMessage != null)
            {
                WWWForm parameters = new WWWForm();
                parameters.AddField("aborted_with", abortedWithMessage);
                request = UnityWebRequest.Put(pingUrl, parameters.data);
            }
            else
            {
                request = new UnityWebRequest(pingUrl);
            }
            request.method = UnityWebRequest.kHttpVerbPUT;
            request.downloadHandler = new DownloadHandlerBuffer();

            return new HttpRequest(request, HttpRequest.Type.Private);
        }

        #endregion

        #region Helper methods

        private string RequestUrl(string path)
        {
            return configuration.BaseUrl + "/" + configuration.ApiVersion + path;
        }

        private void SetAuthorizationHeaderBearerToken(HttpRequest request, string accessToken)
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        private void SetAcceptLanguageHeader(HttpRequest request)
        {
            request.SetRequestHeader("Accept-Language", configuration.Locale);
        }

        #endregion
    }
}
