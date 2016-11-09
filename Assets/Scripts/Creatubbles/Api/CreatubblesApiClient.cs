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

namespace Creatubbles.Api
{
    public class CreatubblesApiClient
    {
        private IApiConfiguration configuration;
        public ISecureStorage secureStorage; // TODO - eventually make it private

        public CreatubblesApiClient(IApiConfiguration configuration, ISecureStorage secureStorage)
        {
            this.configuration = configuration;
            this.secureStorage = secureStorage;
        }

        public OAuthRequest CreatePostAuthenticationApplicationTokenRequest()
        {
            string url = RequestUrl("/oauth/token");
            WWWForm data = new WWWForm();
            data.AddField("grant_type", "client_credentials");
            data.AddField("client_id", configuration.AppId);
            data.AddField("client_secret", configuration.AppSecret);

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new OAuthRequest(request);
        }

        public UnityWebRequest CreatePostAuthenticationUserTokenRequest(string username, string password)
        {
            string url = RequestUrl("/oauth/token");
            WWWForm data = new WWWForm();
            data.AddField("grant_type", "password");
            data.AddField("client_id", configuration.AppId);
            data.AddField("client_secret", configuration.AppSecret);
            data.AddField("username", username);
            data.AddField("password", password);

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        public ApiRequest<LandingUrlsResponse> CreateGetLandingUrlsRequest(string applicationToken)
        {
            string url = RequestUrl("/landing_urls");
            UnityWebRequest request = UnityWebRequest.Get(url);
            SetAuthorizationHeaderBearerToken(request, applicationToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<LandingUrlsResponse>(request);
        }

        public UnityWebRequest CreateGetLoggedInUserRequest(string userToken)
        {
            string url = RequestUrl("/users/me");
            UnityWebRequest request = UnityWebRequest.Get(url);
            SetAuthorizationHeaderBearerToken(request, userToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        public UnityWebRequest CreateNewCreationRequest(string userToken, NewCreationData creationData)
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
            SetAuthorizationHeaderBearerToken(request, userToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        public UnityWebRequest CreateGetCreationRequest(string userToken, string creationId)
        {
            string url = RequestUrl("/creations/" + creationId);
            UnityWebRequest request = UnityWebRequest.Get(url);
            SetAuthorizationHeaderBearerToken(request, userToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        public UnityWebRequest CreatePostCreationUploadRequest(string userToken, string creationId, UploadExtension extension = UploadExtension.JPG)
        {
            string url = RequestUrl("/creations/" + creationId + "/uploads");
            WWWForm data = new WWWForm();
            data.AddField("extension", extension.StringValue());

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            SetAuthorizationHeaderBearerToken(request, userToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        public UnityWebRequest CreatePutUploadFileRequest(string userToken, string url, string contentType, byte[] data)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, data);
            SetAcceptLanguageHeader(request);
            request.SetRequestHeader("Content-Type", contentType);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        // TODO - abortedWithMessage - argument included when upload fails; include the body returned by the failed upload attempt or ‘user’ in case the user aborted the upload
        // API doc: https://stateoftheart.creatubbles.com/api/#update-creation-upload
        public UnityWebRequest CreatePutUploadFinishedRequest(string userToken, string pingUrl, string abortedWithMessage = null)
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
            SetAuthorizationHeaderBearerToken(request, userToken);
            SetAcceptLanguageHeader(request);
            request.downloadHandler = new DownloadHandlerBuffer();

            return request;
        }

        // helper methods

        private string RequestUrl(string path)
        {
            return configuration.BaseUrl + "/" + configuration.ApiVersion + path;
        }

        private void SetAuthorizationHeaderBearerToken(UnityWebRequest request, string token)
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
        }

        private void SetAcceptLanguageHeader(UnityWebRequest request)
        {
            request.SetRequestHeader("Accept-Language", configuration.Locale);
        }
    }
}
