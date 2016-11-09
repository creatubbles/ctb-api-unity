//
//  CreationUploadSession.cs
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
using System.Collections;
using UnityEngine;

namespace Creatubbles.Api
{
    // TODO - add progress reporting
    // performs series of requests allocating Creation, uploading file, updating Creation and notifying backend when operation finishes
    // see https://stateoftheart.creatubbles.com/api/#creation-upload for details
    public class CreationUploadSession
    {
        private const string InternalErrorMissingOrInvalidResponseData = "Invalid or missing data in reponse body.";
        private const string InternalErrorUnsupportedDataType = "Unsupported data type used.";

        private NewCreationData creationData;

        // is true when all requests completed or error was encountered
        public bool IsDone { get; private set; }

        // true when Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded
        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html
        public bool IsSystemError { get; private set; }

        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html
        public string SystemError { get; private set; }

        // true when request ends with an error like HTTP status 4xx or 5xx
        public bool IsApiError { get; private set; }

        // contains the errors returned by the API
        public ApiError[] ApiErrors { get; private set; }

        // true if some client implementation or internal API error occured like failed data mapping or unsupported upload type selected
        // NOTE! these kinds of errors are not user-recoverable and should be reported to users as "Something went wrong" kind of error
        public bool IsInternalError { get; private set; }

        public string InternalError { get; private set; }

        // true when either system, API or internal errors occured
        public bool IsAnyError { get { return IsSystemError || IsApiError || IsInternalError; } }

        public CreationUploadSession(NewCreationData creationData)
        {
            this.creationData = creationData;
            this.IsSystemError = false;
            this.IsApiError = false;
            this.IsInternalError = false;
            this.IsDone = false;
        }
            
        // triggers series of requests forming new Creation entity, uploading file, updating Creation with uploaded file's URL and notifying server when upload is finished
        public IEnumerator Upload(CreatubblesApiClient creatubbles)
        {
            IsSystemError = false;
            IsApiError = false;
            IsInternalError = false;

            IsDone = false;

            string creationId = creationData.creationId;
            // make new Creation entity
            if (creationId == null)
            {
                ApiRequest<CreationGetResponse> makeCreationRequest = creatubbles.CreateNewCreationRequest(creationData);

                Debug.Log("Sending request: " + makeCreationRequest.Url);

                yield return creatubbles.SendSecureRequest(makeCreationRequest);

                if (makeCreationRequest.IsAnyError)
                {
                    FinishWithErrors(makeCreationRequest);
                    yield break;
                }

                if (makeCreationRequest.data == null || makeCreationRequest.data.data == null)
                {
                    FinishWithInternalError(InternalErrorMissingOrInvalidResponseData);
                    yield break;
                }

                Debug.Log("Success with data: " + makeCreationRequest.data.data.ToString());

                creationId = makeCreationRequest.data.data.id;
            }

            // TODO - support other upload types
            if (creationData.dataType != NewCreationData.Type.Image)
            {
                FinishWithInternalError(InternalErrorMissingOrInvalidResponseData);
                yield break;   
            }

            // prepare upload for Creation
            ApiRequest<CreationsUploadPostResponse> prepareUploadRequest = creatubbles.CreatePostCreationUploadRequest(creationId, creationData.uploadExtension);

            Debug.Log("Sending request: " + prepareUploadRequest.Url);

            yield return creatubbles.SendSecureRequest(prepareUploadRequest);

            if (prepareUploadRequest.IsAnyError)
            {
                FinishWithErrors(prepareUploadRequest);
                yield break;
            }

            if (prepareUploadRequest.data == null || prepareUploadRequest.data.data == null)
            {
                Debug.Log("Error: Invalid or missing data in response");
                yield break;
            }

            Debug.Log("Prepared Creation upload: " + prepareUploadRequest.data.data.ToString());
            CreationsUploadAttributesDto upload = prepareUploadRequest.data.data.attributes;

            // perform upload
            ApiRequestWithEmptyResponseData uploadRequest = creatubbles.CreatePutUploadFileRequest(upload.url, upload.content_type, creationData.image);

            Debug.Log("Sending request: " + uploadRequest.Url);

            yield return creatubbles.SendRequest(uploadRequest);

            if (uploadRequest.IsAborted)
            {
                yield return PutUploadFileFinished(creatubbles, upload.ping_url, "user");
                yield break;
            }

            if (uploadRequest.IsAnyError)
            {
                FinishWithErrors(uploadRequest);
                yield return PutUploadFileFinished(creatubbles, upload.ping_url, uploadRequest.RawResponseBody);
                yield break;
            }

            Debug.Log("Success");
            yield return PutUploadFileFinished(creatubbles, upload.ping_url);

            IsDone = true;
        }

        #region Helper methods

        private IEnumerator PutUploadFileFinished(CreatubblesApiClient creatubbles, string pingUrl, string abortedWithMessage = null)
        {
            ApiRequestWithEmptyResponseData request = creatubbles.CreatePutUploadFinishedRequest(pingUrl);

            Debug.Log("Sending request: " + request.Url);

            yield return creatubbles.SendSecureRequest(request);

            if (request.IsAnyError)
            {
                FinishWithErrors(request);
                yield break;
            }

            Debug.Log("Success");
        }

        private void FinishWithErrors<T>(ApiRequest<T> request)
        {
            if (request.IsSystemError)
            {
                FinishWithSystemError(request);
            }
            else
            {
                FinishWithApiErrors(request);
            }
        }

        private void FinishWithSystemError<T>(ApiRequest<T> request)
        {
            IsSystemError = true;
            SystemError = request.SystemError;
            IsDone = true;
        }

        private void FinishWithApiErrors<T>(ApiRequest<T> request)
        {
            IsApiError = true;
            ApiErrors = request.apiErrors;
            IsDone = true;
        }

        private void FinishWithInternalError(string message)
        {
            IsInternalError = true;
            InternalError = message;
            IsDone = true;
        }

        #endregion
    }
}
