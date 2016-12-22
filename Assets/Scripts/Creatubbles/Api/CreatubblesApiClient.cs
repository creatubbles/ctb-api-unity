//
//  CreatubblesApiClient.cs
//  Creatubbles API Client Unity SDK
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
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Creatubbles.Api.Data;
using Creatubbles.Api.Storage;
using Creatubbles.Api.Requests;

/// <summary>
/// Contains classes and interfaces used in communicating with Creatubbles API.
/// </summary>
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

        private IApiConfiguration configuration;
        private ISecureStorage secureStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.CreatubblesApiClient"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="secureStorage">Secure storage.</param>
        public CreatubblesApiClient(IApiConfiguration configuration, ISecureStorage secureStorage)
        {
            this.configuration = configuration;
            this.secureStorage = secureStorage;
        }

        #region Methods for managing authentication tokens and sending requests

        /// <summary>
        /// Sets the authentication token to be used for authenticating requests.
        /// </summary>
        /// <param name="token">Token.</param>
        public void SetAuthenticationToken(string accessToken)
        {
            secureStorage.SaveValue(UserAccessTokenKey, accessToken);
        }

        /// <summary>
        /// Sends log in request and saves the token to secure storage if successful.
        /// </summary>
        /// <returns>Enumerator.</returns>
        /// <param name="request">Log in request.</param>
        public IEnumerator SendLogInRequest(OAuthRequest request)
        {
            SetAcceptLanguageHeader(request);
            SetAcceptHeaderJson(request);

            yield return request.Send();

            if (request.IsAnyError || request.Data == null)
            {
                yield break;
            }

            secureStorage.SaveValue(UserAccessTokenKey, request.Data.access_token);
        }

        /// <summary>
        /// Attempts to send the request.
        /// </summary>
        /// <remarks>
        /// Will fail with unathorized error, if request requires application or user authorization token and one isn't found in secure storage.
        /// </remarks>
        /// <returns>Enumerator.</returns>
        /// <param name="request">Request.</param>
        public IEnumerator SendRequest(HttpRequest request)
        {
            SetAcceptLanguageHeader(request);
            SetAcceptHeaderJson(request);

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

        /// <summary>
        /// Attempts to send request with user OAuth token.
        /// </summary>
        /// <remarks>
        /// Will fail with unathorized error, if token was not found.
        /// </remarks>
        /// <returns>Enumerator.</returns>
        /// <param name="request">Request.</param>
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

        /// <summary>
        /// Attempts to send request with application OAuth token.
        /// </summary>
        /// <remarks>
        /// Will fail with unathorized error, if token was not found.
        /// </remarks>
        /// <returns>Enumerator.</returns>
        /// <param name="request">Request.</param>
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

        /// <summary>
        /// Sends unmodified request.
        /// </summary>
        /// <returns>Enumerator.</returns>
        /// <param name="request">Request.</param>
        private IEnumerator SendRegularRequest(HttpRequest request)
        {
            yield return request.Send();
        }

        /// <summary>
        /// Logs user out if currently logged in.
        /// </summary>
        public void LogOut()
        {
            if (secureStorage.HasValue(UserAccessTokenKey))
            {
                secureStorage.DeleteValue(UserAccessTokenKey);
            }
        }

        /// <summary>
        /// Determines whether user is currently logged in.
        /// </summary>
        /// <returns><c>true</c> if user is currently logged in, otherwise <c>false</c>.</returns>
        public bool IsUserLoggedIn()
        {
            return secureStorage.HasValue(UserAccessTokenKey);
        }

        #endregion

        #region Requests factory methods

        /// <summary>
        /// Creates Creatubbles OAuth application token request.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#oauth-token-client-credentials-flow.
        /// </remarks>
        /// <returns>OAuth application token request.</returns>
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

        /// <summary>
        /// Creates Creatubbles OAuth log in request.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#oauth-token-client-credentials-flow.
        /// </remarks>
        /// <returns>OAuth log in request.</returns>
        /// <param name="username">User's Creatubbles account username.</param>
        /// <param name="password">User's Creatubbles account password.</param>
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

        /// <summary>
        /// Retrieve all landing URLs (besides creation landing URLs) for a specific application or user.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#list-landing-urls.
        /// </remarks>
        /// <returns>The get landing URLs request.</returns>
        /// <param name="requestType">
        ///     <list type="bullet">
        ///         <item><c>Public</c> request will return application specific URLs.</item>
        ///         <item><c>Private</c> request will return user specific URLs (user must be logged in first).</item>
        ///         <item><c>Regular</c> request will cause request to fail.</item>
        ///     </list>
        /// </param>
        public ApiRequest<LandingUrlsResponse> CreateGetLandingUrlsRequest(HttpRequest.Type requestType = HttpRequest.Type.Public)
        {
            string url = RequestUrl("/landing_urls");
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<LandingUrlsResponse>(request, requestType);
        }

        /// <summary>
        /// Creates request to get logged in user’s profile.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#get-user-39-s-profile.
        /// </remarks>
        /// <returns>The get logged in user request.</returns>
        public ApiRequest<LoggedInUserResponse> CreateGetLoggedInUserRequest()
        {
            string url = RequestUrl("/users/me");
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<LoggedInUserResponse>(request, HttpRequest.Type.Private);
        }

        /// <summary>
        /// Creates the create creation request.
        /// </summary>
        /// <remarks>
        /// Response will contain newly created creation object with ID.
        /// More info at https://stateoftheart.creatubbles.com/api/#create-creation.
        /// </remarks>
        /// <returns>The request to create creation.</returns>
        /// <param name="creationData">Creation data.</param>
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
            if (creationData.creationMonth != null)
            {
                data.AddField("created_at_month", creationData.creationMonth.ToString());
            }
            if (creationData.creationYear != null)
            {
                data.AddField("created_at_year", creationData.creationYear.ToString());
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

        /// <summary>
        /// Creates request to get specific creation.
        /// </summary>
        /// <remarks>
        /// More info at https://stateoftheart.creatubbles.com/api/#get-specific-creation.
        /// </remarks>
        /// <returns>The request to get specific creation.</returns>
        /// <param name="creationId">Creation ID.</param>
        public ApiRequest<CreationGetResponse> CreateGetCreationRequest(string creationId)
        {
            string url = RequestUrl("/creations/" + creationId);
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<CreationGetResponse>(request, HttpRequest.Type.Private);
        }

        /// <summary>
        /// Creates request that retrieves all information required to upload a new image.
        /// </summary>
        /// <remarks>
        /// You can use this also to upload updated versions of a creation.
        /// More info at https://stateoftheart.creatubbles.com/api/#create-creation-upload.
        /// </remarks>
        /// <returns>The request for retrieving upload information.</returns>
        /// <param name="creationId">ID of creation for which to retrieve upload information.</param>
        /// <param name="extension">Extension of file to be uploaded.</param>
        public ApiRequest<CreationsUploadPostResponse> CreatePostCreationUploadRequest(string creationId, UploadExtension extension)
        {
            string url = RequestUrl("/creations/" + creationId + "/uploads");
            WWWForm data = new WWWForm();
            data.AddField("extension", extension.StringValue());

            UnityWebRequest request = UnityWebRequest.Post(url, data);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<CreationsUploadPostResponse>(request, HttpRequest.Type.Private);
        }

        /// <summary>
        /// Creates request to upload a file to an absolute URL.
        /// </summary>
        /// <returns>The upload file request.</returns>
        /// <param name="url">Absolute URL to which data should be uploaded.</param>
        /// <param name="contentType">Value that will be set in <c>Content-Type</c> header.</param>
        /// <param name="data">Data to be uploaded.</param>
        public HttpRequest CreateUploadFileRequest(string url, string contentType, byte[] data)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, data);
            request.SetRequestHeader("Content-Type", contentType);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new HttpRequest(request, HttpRequest.Type.Regular);
        }

        /// <summary>
        /// Creates the request to download a file from an absolute URL.
        /// </summary>
        /// <returns>The download file request.</returns>
        /// <param name="url">Absolute URL of the file to be downloaded.</param>
        public HttpRequest CreateDownloadFileRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new HttpRequest(request, HttpRequest.Type.Regular);
        }

        /// <summary>
        /// Creates update creation upload request.
        /// </summary>
        /// <remarks>
        /// This request notifies server that upload has finished (successfully or not).
        /// More info at https://stateoftheart.creatubbles.com/api/#update-creation-upload.
        /// </remarks>
        /// <returns>The update creation upload request.</returns>
        /// <param name="pingUrl">Absolute URL containing upload ID.</param>
        /// <param name="abortedWithMessage">Argument included when upload fails. Include the body returned by the failed upload attempt or ‘user’ in case the user aborted the upload.</param>
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

        /// <summary>
        /// Creates creation gallery submission request.
        /// </summary>
        /// <remarks>
        /// This request submits a creation to a gallery.
        /// More info at https://stateoftheart.creatubbles.com/api/#submit-creation-to-the-gallery.
        /// </remarks>
        /// <returns>The update creation upload request.</returns>
        /// <param name="galleryId">Id of the gallery to upload to.</param>
        /// <param name="creationId">Id of the creation to upload.</param>
        public ApiRequest<GallerySubmissionResponse> CreateGallerySubmissionRequest(string galleryId, string creationId)
        {
            string url = RequestUrl("/gallery_submissions");
            WWWForm parameters = new WWWForm();
            if (galleryId != null)
            {
                parameters.AddField("gallery_id", galleryId);
            }
            if (creationId != null)
            {
                parameters.AddField("creation_id", creationId);
            }
            UnityWebRequest request = UnityWebRequest.Post(url, parameters);
            request.downloadHandler = new DownloadHandlerBuffer();

            return new ApiRequest<GallerySubmissionResponse>(request, HttpRequest.Type.Private);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Creates an absolute request URL based on configuration and path.
        /// </summary>
        /// <returns>The absolute URL.</returns>
        /// <param name="path">Path to the API resource.</param>
        private string RequestUrl(string path)
        {
            return configuration.BaseUrl + "/" + configuration.ApiVersion + path;
        }

        /// <summary>
        /// Sets the Authorization header as Bearer token.
        /// </summary>
        /// <param name="request">Request to set the header for.</param>
        /// <param name="accessToken">Access token to be set in the header.</param>
        private void SetAuthorizationHeaderBearerToken(HttpRequest request, string accessToken)
        {
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        }

        /// <summary>
        /// Sets the <c>Accept-Language</c> header to locale supplied in configuration.
        /// </summary>
        /// <param name="request">Request to set the header for.</param>
        private void SetAcceptLanguageHeader(HttpRequest request)
        {
            request.SetRequestHeader("Accept-Language", configuration.Locale);
        }

        private void SetAcceptHeaderJson(HttpRequest request)
        {
            request.SetRequestHeader("Accept", "application/vnd.api+json");
        }

        #endregion
    }
}
