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
    // performs series of requests allocating Creation, uploading file, updating Creation and notifying backend when operation finishes
    // see https://stateoftheart.creatubbles.com/api/#creation-upload for details
    public class CreationUploadSession: ICancellable
    {
        private const string InternalErrorMissingOrInvalidResponseData = "Invalid or missing data in reponse body.";
        private const string InternalErrorUnsupportedDataType = "Unsupported data type used.";
        private const string InternalErrorUnknownError = "An unknown error occured.";
        private const string InternalErrorUserCancelled = "Request cancelled by user.";

        private const string NotifyFileUploadFinishedMessageUserCancelled = "user";

        private NewCreationData creationData;
        // upload request is stored in instance variable as data source for UploadProgress
        private HttpRequest uploadRequest;
        // current request is stored to support cancellation
        private ICancellable currentRequest;

        // is true when all requests completed or error was encountered
        public bool IsDone { get; private set; }

        // true if request was cancelled
        public bool IsCancelled { get; private set; }

        // true when Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded
        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html
        public bool IsSystemError { get; private set; }

        // See: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html
        public string SystemError { get; private set; }

        // true when request ends with an error like HTTP status 4xx or 5xx
        public bool IsApiError { get; private set; }

        // contains the errors returned by the API
        public ApiError[] ApiErrors { get; private set; }

        // true if some client implementation or internal API error occured like failed data mapping or malformed response
        // NOTE! internal errors are not user-recoverable as they usually indicate an issue with implementation (client or backend) e.g. "Something went wrong" kind of error
        public bool IsInternalError { get; private set; }

        public string InternalError { get; private set; }

        // true when either system, API or internal errors occured
        public bool IsAnyError { get { return IsSystemError || IsApiError || IsInternalError; } }

        public float UploadProgress { get { return uploadRequest == null ? 0 : uploadRequest.UploadProgress; } }

        public CreationUploadSession(NewCreationData creationData)
        {
            this.creationData = creationData;
            this.ResetFlagsAndFields();
        }
            
        // triggers series of requests forming new Creation entity, uploading file, updating Creation with uploaded file's URL and notifying server when upload is finished
        public IEnumerator Upload(CreatubblesApiClient creatubbles)
        {
            ResetFlagsAndFields();

            string creationId = creationData.creationId;
            // make new Creation entity
            if (creationId == null)
            {
                ApiRequest<CreationGetResponse> makeCreationRequest = creatubbles.CreateNewCreationRequest(creationData);
                currentRequest = makeCreationRequest;

                Debug.Log("Sending request: " + makeCreationRequest.Url);

                yield return creatubbles.SendRequest(makeCreationRequest);

                if (makeCreationRequest.IsAnyError)
                {
                    FinishWithErrors(makeCreationRequest);
                    yield break;
                }

                if (makeCreationRequest.Data == null || makeCreationRequest.Data.data == null)
                {
                    FinishWithInternalError(InternalErrorMissingOrInvalidResponseData);
                    yield break;
                }

                Debug.Log("Success with data: " + makeCreationRequest.Data.data.ToString());

                creationId = makeCreationRequest.Data.data.id;
            }

            if (IsCancelled)
            {
                FinishWithInternalError(InternalErrorUserCancelled);
                yield break;
            }

            // get upload path for Creation
            ApiRequest<CreationsUploadPostResponse> prepareUploadRequest = creatubbles.CreatePostCreationUploadRequest(creationId, creationData.uploadExtension);
            currentRequest = prepareUploadRequest;

            Debug.Log("Sending request: " + prepareUploadRequest.Url);

            yield return creatubbles.SendRequest(prepareUploadRequest);

            if (prepareUploadRequest.IsAnyError)
            {
                FinishWithErrors(prepareUploadRequest);
                yield break;
            }

            if (prepareUploadRequest.Data == null || prepareUploadRequest.Data.data == null)
            {
                FinishWithInternalError(InternalErrorMissingOrInvalidResponseData);
                yield break;
            }

            Debug.Log("Prepared Creation upload: " + prepareUploadRequest.Data.data.ToString());
            CreationsUploadAttributesDto upload = prepareUploadRequest.Data.data.attributes;

            if (IsCancelled)
            {
                yield return NotifyFileUploadFinished(creatubbles, upload.ping_url, NotifyFileUploadFinishedMessageUserCancelled);
                yield break;
            }

            // prepare data for upload
            byte[] uploadData = null;
            // if creationData.url is set, file must be downloaded from the URL, before it can be uploaded
            if (creationData.url != null)
            {
                HttpRequest downloadRequest = creatubbles.CreateDownloadFileRequest(creationData.url);
                currentRequest = downloadRequest;

                yield return creatubbles.SendRequest(downloadRequest);

                if (downloadRequest.IsAnyError)
                {
                    FinishWithSystemOrInternalErrors(downloadRequest);
                    yield break;
                }

                uploadData = downloadRequest.ResponseBodyBytes;
            }

            if (IsCancelled)
            {
                yield return NotifyFileUploadFinished(creatubbles, upload.ping_url, NotifyFileUploadFinishedMessageUserCancelled);
                yield break;
            }

            uploadData = uploadData != null ? uploadData : creationData.data;
            uploadRequest = creatubbles.CreateUploadFileRequest(upload.url, upload.content_type, uploadData);
            currentRequest = uploadRequest;

            Debug.Log("Sending request: " + uploadRequest.Url);

            // perform upload
            yield return creatubbles.SendRequest(uploadRequest);
            // regardless whether upload was success or failure, we must notify backend about it's status - that request should not be cancelled so currentRequest is nullyfied
            currentRequest = null;

            if (uploadRequest.IsCancelled)
            {
                yield return NotifyFileUploadFinished(creatubbles, upload.ping_url, NotifyFileUploadFinishedMessageUserCancelled);
                yield break;
            }

            if (uploadRequest.IsAnyError)
            {
                FinishWithSystemOrInternalErrors(uploadRequest);
                yield return NotifyFileUploadFinished(creatubbles, upload.ping_url, uploadRequest.ResponseBodyText);
                yield break;
            }

            yield return NotifyFileUploadFinished(creatubbles, upload.ping_url);

            IsDone = true;
            Debug.Log("Creation upload successful");
        }

        public void Cancel()
        {
            if (IsDone)
            {
                return;
            }

            if (currentRequest != null)
            {
                currentRequest.Cancel();
            }

            IsCancelled = true;
        }

        #region Helper methods

        // notifies the backend that file upload is finished (regardless whether it completed successfully or with errors)
        private IEnumerator NotifyFileUploadFinished(CreatubblesApiClient creatubbles, string pingUrl, string abortedWithMessage = null)
        {
            HttpRequest request = creatubbles.CreatePutUploadFinishedRequest(pingUrl);

            string aborted = abortedWithMessage == null ? "" : " with aborted message: " + abortedWithMessage;
            Debug.Log("Sending request: " + request.Url + aborted);

            yield return creatubbles.SendRequest(request);

            if (request.IsAnyError)
            {
                FinishWithSystemOrInternalErrors(request);
                yield break;
            }

            Debug.Log("Notify file upload finished request completed");
        }

        private void FinishWithErrors<T>(ApiRequest<T> request)
        {
            if (request.IsSystemError)
            {
                FinishWithSystemError(request);
            }
            else if (request.IsHttpError)
            {
                FinishWithApiErrors(request);
            }
            else
            {
                FinishWithInternalError(InternalErrorUnknownError);
            }
        }

        private void FinishWithSystemOrInternalErrors(HttpRequest request)
        {
            if (request.IsSystemError)
            {
                FinishWithSystemError(request);
            }
            else if (request.IsHttpError)
            {
                FinishWithInternalError("Request '" + request.Url + "' failed with response code '" + request.ResponseCode + "' and body '" + request.RawHttpError + "'");
            }
            else
            {
                FinishWithInternalError(InternalErrorUnknownError);
            }
        }

        private void FinishWithSystemError(HttpRequest request)
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

        private void ResetFlagsAndFields()
        {
            IsSystemError = false;
            IsApiError = false;
            IsInternalError = false;
            IsDone = false;
            IsCancelled = false;
            uploadRequest = null;
        }

        #endregion
    }
}
