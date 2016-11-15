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
    /// <summary>
    /// Creatubbles API request providing parsed response as <c cref="ApiRequest.Data">Data</c> and API specific errors as <c cref="ApiRequest.apiErrors">apiErrors</c>.
    /// </summary>
    public class ApiRequest<T>: HttpRequest
    {
        /// <summary>
        /// Parsed data from response body.
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; private set; }

        /// <summary>
        /// Errors returned by the API.
        /// </summary>
        public ApiError[] apiErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.ApiRequest`1"/> class.
        /// </summary>
        /// <param name="webRequest">Web request.</param>
        /// <param name="requestType">Request type.</param>
        public ApiRequest(UnityWebRequest webRequest, Type requestType): base(webRequest, requestType)
        {
        }

        /// <summary>
        /// Send the request.
        /// </summary>
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

        /// <summary>
        /// Deserializes the JSON body.
        /// </summary>
        /// <returns>The deserialized response body.</returns>
        /// <param name="json">JSON.</param>
        /// <typeparam name="DeserializedType">Type of the parsed response.</typeparam>
        private static DeserializedType DeserializeJson<DeserializedType>(string json)
        {
            return JsonUtility.FromJson<DeserializedType>(json);
        }
    }
}
