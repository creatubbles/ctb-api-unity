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
    public class HttpRequest
    {
        public enum Type
        {
            // Public requests must be authorized with application OAuth token
            Public,
            // Private requests must be authorized with user OAuth token
            Private,
            // Regular requests do not require OAuth authorization
            Regular
        }

        #region Properties and fields

        private UnityWebRequest webRequest;

        // determines if request requires OAuth authorization headers
        public readonly Type requestType;

        // true when Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded
        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html
        public bool IsSystemError { get { return webRequest.isError; } }

        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html
        public string SystemError { get { return webRequest.error; } }

        // true if request is done but response code represents failure
        public bool IsHttpError { get { return IsDone && !IsNonFailureHttpStatus; } }

        // returns RawResponseBody
        public string RawHttpError { get { return RawResponseBody; } }

        // true if System or HTTP error occured
        public bool IsAnyError { get { return IsSystemError || IsHttpError; } }

        // URL of the request
        public string Url { get { return webRequest.url; } }

        public bool IsDone { get { return webRequest.isDone; } }

        public bool IsAborted { get; internal set; }

        public long ResponseCode { get { return webRequest.responseCode; } }

        // true for HTTP statuses from 200 to 399
        public bool IsNonFailureHttpStatus { get { return 200 <= ResponseCode && ResponseCode <= 399; } }

        // returns response body as string or empty string if response had no body or DownloadHandler was not provided
        public string RawResponseBody
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

        #endregion

        #region Methods

        public HttpRequest(UnityWebRequest webRequest, Type requestType)
        {
            this.webRequest = webRequest;
            this.requestType = requestType;
            this.IsAborted = false;
        }

        virtual internal IEnumerator Send()
        {
            IsAborted = false;

            yield return webRequest.Send();
        }

        // aborts request if not IsDone
        public void Abort()
        {
            if (IsDone)
            {
                return;
            }

            webRequest.Abort();
            IsAborted = true;
        }

        public void SetRequestHeader(string name, string value)
        {
            webRequest.SetRequestHeader(name, value);
        }

        #endregion
    }
}
