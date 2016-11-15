//
//  HttpRequest.cs
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
using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

namespace Creatubbles.Api
{
    /// <summary>
    /// Class representing an HTTP request, which serves as wrapper around <c>UnityWebRequest</c>.
    /// </summary>
    public class HttpRequest: ICancellable
    {
        /// <summary>
        /// Type of the request based on which <c>CreatubblesApiClient</c> adds authorization headers if required.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Public requests must be authorized with application OAuth token.
            /// </summary>
            Public,
            /// <summary>
            /// Private requests must be authorized with user OAuth token.
            /// </summary>
            Private,
            /// <summary>
            /// Regular requests do not require OAuth authorization.
            /// </summary>
            Regular
        }

        #region Properties and fields

        private UnityWebRequest webRequest;

        /// <summary>
        /// The type of the request determines if request requires OAuth authorization headers
        /// </summary>
        public readonly Type requestType;

        /// <summary>
        /// Indicates whether a system error occured.
        /// More info at <see href="https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html">https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html</see>.
        /// </summary>
        /// <value><c>true</c> if Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded, otherwise <c>false</c>.</value>
        public bool IsSystemError { get { return webRequest.isError; } }

        /// <summary>
        /// Gets the system error.
        /// More info at <see href="https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html">https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html</see>
        /// </summary>
        /// <value>The system error.</value>
        public string SystemError { get { return webRequest.error; } }

        /// <summary>
        /// Indicating whether an Http error occured.
        /// </summary>
        /// <value><c>true</c> if request is done, not cancelled, but response code represents failure, otherwise <c>false</c>.</value>
        public bool IsHttpError { get { return IsDone && !IsCancelled && !IsNonFailureHttpStatus; } }

        /// <summary>
        /// Gets the raw HTTP error.
        /// </summary>
        /// <value>The raw HTTP error.</value>
        public string RawHttpError { get { return ResponseBodyText; } }

        /// <summary>
        /// Gets a value indicating whether this instance is any error.
        /// </summary>
        /// <value><c>true</c> if System or HTTP error occured, otherwise <c>false</c>.</value>
        public bool IsAnyError { get { return IsSystemError || IsHttpError; } }

        /// <summary>
        /// Gets the URL of the request.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get { return webRequest.url; } }

        /// <summary>
        /// Indicates whether request is finished.
        /// </summary>
        /// <value><c>true</c> if request is finished, otherwise <c>false</c>.</value>
        public bool IsDone { get { return webRequest.isDone; } }

        /// <summary>
        /// Determines if request was cancelled.
        /// </summary>
        /// <value>true</value>
        /// <see cref="ICancellable.Cancel()"></see>
        /// <c>false</c>
        public bool IsCancelled { get; internal set; }

        /// <summary>
        /// Gets the HTTP response code.
        /// </summary>
        /// <value>The HTTP response code.</value>
        public long ResponseCode { get { return webRequest.responseCode; } }

        /// <summary>
        /// Gets the upload progress.
        /// </summary>
        /// <value>The upload progress in rage <0;1>.</value>
        public float UploadProgress { get { return webRequest.uploadProgress; } }

        /// <summary>
        /// Gets the download progress.
        /// </summary>
        /// <value>The download progress in range <0;1>.</value>
        public float DownloadProgress { get { return webRequest.downloadProgress; } }

        // true for HTTP statuses from 200 to 399
        /// <summary>
        /// Determines if HTTP status is a non-failure one (in range of <200;399>).
        /// </summary>
        /// <value><c>true</c> in range of <200;399>, otherwise <c>false</c>.</value>
        public bool IsNonFailureHttpStatus { get { return 200 <= ResponseCode && ResponseCode <= 399; } }

        /// <summary>
        /// Gets the response body as string.
        /// </summary>
        /// <value>The response body string or empty string, if response had no body or DownloadHandler was not provided.</value>
        public string ResponseBodyText
        {
            get
            {
                if (webRequest.downloadHandler == null || webRequest.downloadHandler.text == null)
                {
                    return "";
                }

                return webRequest.downloadHandler.text;
            }
        }

        /// <summary>
        /// Gets the response body as bytes.
        /// </summary>
        /// <value>The response body as byte array or empty byte array, if response had no body or DownloadHandler was not provided.</value>
        public byte[] ResponseBodyBytes
        {
            get
            {
                if (webRequest.downloadHandler == null || webRequest.downloadHandler.data == null)
                {
                    return new byte[0];
                }

                return webRequest.downloadHandler.data;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.HttpRequest"/> class.
        /// </summary>
        /// <param name="webRequest">Web request.</param>
        /// <param name="requestType">Request type.</param>
        public HttpRequest(UnityWebRequest webRequest, Type requestType)
        {
            this.webRequest = webRequest;
            this.requestType = requestType;
            this.IsCancelled = false;
        }

        /// <summary>
        /// Send the request.
        /// </summary>
        virtual internal IEnumerator Send()
        {
            IsCancelled = false;

            yield return webRequest.Send();
        }

        /// <summary>
        /// Cancels the request if not <see cref="HttpRequest.IsDone"/>.
        /// </summary>
        public void Cancel()
        {
            if (IsDone)
            {
                return;
            }

            webRequest.Abort();
            IsCancelled = true;
        }

        /// <summary>
        /// Sets the request header.
        /// </summary>
        /// <param name="name">Header parameter name.</param>
        /// <param name="value">Header parameter value.</param>
        public void SetRequestHeader(string name, string value)
        {
            webRequest.SetRequestHeader(name, value);
        }

        #endregion
    }
}
