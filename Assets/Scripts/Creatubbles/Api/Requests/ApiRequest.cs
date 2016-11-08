//
//  ApiRequest.cs
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
    public class ApiRequest<T>
    {
        private UnityWebRequest webRequest;

        // true for HTTP statuses from 200 to 399
        private bool isNonFailureHttpStatus { get { return 200 <= webRequest.responseCode && webRequest.responseCode <= 399; } }

        // data from response body
        public T data;

        // true when Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded
        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html
        public bool IsSystemError { get { return webRequest.isError; } }

        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html
        public string SystemError { get { return webRequest.error; } }

		// true when request ends with an error like HTTP status 4xx or 5xx
        public bool IsApiError { get { return !isNonFailureHttpStatus; } }

        // contains the errors returned by the API
        public ApiError[] apiErrors;

        // true when either system or API errors occured
        public bool IsAnyError { get { return IsSystemError || IsApiError; } }

        // URL of the request
        public string Url { get { return webRequest.url; } }

        public ApiRequest(UnityWebRequest webRequest)
        {
            this.webRequest = webRequest;
        }

        public IEnumerator Send()
        {
            yield return webRequest.Send();

            // can't process response without download handler
            if (webRequest.downloadHandler == null)
            {
                yield break;
            }

            string json = webRequest.downloadHandler.text;

            // deserialize any API errors
            if (!IsSystemError && IsApiError)
            {
                apiErrors = DeserializeJson<ApiErrorResponse>(json).errors;
                yield break;
            }

            // deserialize actual response body
            data = DeserializeJson<T>(json);
        }

        public void Abort()
        {
            webRequest.Abort();
        }

        private static DeserializedType DeserializeJson<DeserializedType>(string json)
        {
            return JsonUtility.FromJson<DeserializedType>(json);
        }
    }
}
