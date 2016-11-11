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
    public class ApiRequest<T>: HttpRequest
    {
        // data from response body
        public T Data { get; private set; }

        // contains the errors returned by the API
        public ApiError[] apiErrors;

        public ApiRequest(UnityWebRequest webRequest, Type requestType): base(webRequest, requestType)
        {
        }

        override internal IEnumerator Send()
        {
            yield return base.Send();

            if (IsCancelled)
            {
                yield break;
            }

            // deserialize any API errors
            if (IsHttpError)
            {
                Debug.Log("Raw error body: " + ResponseBodyText);
                apiErrors = DeserializeJson<ApiErrorResponse>(ResponseBodyText).errors;
                yield break;
            }

            // deserialize response body
            Data = DeserializeJson<T>(ResponseBodyText);
        }

        private static DeserializedType DeserializeJson<DeserializedType>(string json)
        {
            return JsonUtility.FromJson<DeserializedType>(json);
        }
    }
}
