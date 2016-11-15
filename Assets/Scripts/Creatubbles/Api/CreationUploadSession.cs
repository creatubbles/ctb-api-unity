//
//  CreationUploadSession.cs
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
using Creatubbles.Api.Data;
using Creatubbles.Api.Requests;

namespace Creatubbles.Api
{
    /// <summary>
    /// Performs series of requests allocating creation, uploading file, updating creation and notifying backend when operation finishes.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#creation-upload.
    /// </remarks>
    public class CreationUploadSession: ICancellable
    {
        private const string InternalErrorMissingOrInvalidResponseData = "Invalid or missing data in reponse body.";
        private const string InternalErrorUnknownError = "An unknown error occured.";
        private const string InternalErrorUserCancelled = "Request cancelled by user.";

        private const string NotifyFileUploadFinishedMessageUserCancelled = "user";

        private NewCreationData creationData;

        /// <summary>
        /// Upload request is stored in instance variable as data source for <see cref="CreationUploadSession.UploadProgress"/>.
        /// </summary>
        private HttpRequest uploadRequest;

        /// <summary>
        /// Current request is stored to support cancellation.
        /// </summary>
        private ICancellable currentRequest;

        /// <summary>
        /// Indicates whether upload session is finished.
        /// </summary>
        /// <value><c>true</c> if all requests completed, upload was cancelled or an error occured, otherwise <c>false</c>.</value>
        public bool IsDone { get; private set; }

        /// <summary>
        /// Determines if session was cancelled.
        /// </summary>
        /// <value><c>true</c> if session was cancelled, otherwise <c>false</c>.</value>
        /// <see cref="ICancellable.Cancel()"/>
        /// <c>false</c>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Indicates whether a system error occured.
        /// </summary>
        /// <remarks>
        /// More info at https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-isError.html.
        /// </remarks>
        /// <value><c>true</c> if Unity encountered a system error like no internet connection, socket errors, errors resolving DNS entries, or the redirect limit being exceeded, otherwise <c>false</c>.</value>
        public bool IsSystemError { get; private set; }

        /// <summary>
        /// Gets the system error.
        /// </summary>
        /// <value>System error reported by Unity.</value>
        public string SystemError { get; private set; }

        /// <summary>
        /// Indicates whether an API error occured.
        /// </summary>
        /// <value><c>true</c> if request ended with an error like HTTP status 4xx or 5xx, otherwise <c>false</c>.</value>
        public bool IsApiError { get; private set; }

        /// <summary>
        /// Gets the API errors.
        /// </summary>
        /// <value>Errors returned by the API.</value>
        public ApiError[] ApiErrors { get; private set; }

        /// <summary>
        /// Indicates whether an internal error has occured.
        /// </summary>
        /// <remarks>
        /// Internal errors are not user-recoverable as they usually indicate an issue with implementation (client or backend) e.g. "Something went wrong" kind of error.
        /// </remarks>
        /// <value><c>true</c> if some client implementation or internal API issue occured like a failed data mapping or malformed response, otherwise <c>false</c>.</value>
        public bool IsInternalError { get; private set; }

        /// <summary>
        /// Gets the internal error message, which describes malformed response, parsing error or other unexpected failure.
        /// </summary>
        /// <value>The internal error.</value>
        public string InternalError { get; private set; }

        /// <summary>
        /// Indicates whether any error occured during upload session.
        /// </summary>
        /// <value><c>true</c> if system, API or internal errors occured, otherwise <c>false</c>.</value>
        public bool IsAnyError { get { return IsSystemError || IsApiError || IsInternalError; } }

        /// <summary>
        /// Gets the upload progress.
        /// </summary>
        /// <value>The upload progress in range <c><0;1></c>.</value>
        public float UploadProgress { get { return uploadRequest == null ? 0 : uploadRequest.UploadProgress; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Creatubbles.Api.CreationUploadSession"/> class.
        /// </summary>
        /// <param name="creationData">New creation data.</param>
        public CreationUploadSession(NewCreationData creationData)
        {
            this.creationData = creationData;
            this.ResetFlagsAndFields();
        }

        /// <summary>
        /// Triggers series of requests forming new creation entity, uploading file, updating creation with uploaded file's URL and notifying server when upload is finished.
        /// </summary>
        /// <param name="creatubbles">Creatubbles API Client instance.</param>
        public IEnumerator Upload(CreatubblesApiClient creatubbles)
        {
            ResetFlagsAndFields();

            string creationId = creationData.creationId;
            // make new Creation entity
            if (creationId == null)
            {
                ApiRequest<CreationGetResponse> makeCreationRequest = creatubbles.CreateNewCreationRequest(creationData);
                currentRequest = makeCreationRequest;

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
        }

        /// <summary>
        /// Cancels the upload session, if <c>CreationUploadSession.IsDone</c> is <c>false</c>.
        /// </summary>
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

        /// <summary>
        /// Notifies the backend that file upload is finished, regardless whether it completed successfully or with errors.
        /// </summary>
        /// <returns>Enumerator.</returns>
        /// <param name="creatubbles">Creatubbles.</param>
        /// <param name="pingUrl">The URL to which the notification request should be sent.</param>
        /// <param name="abortedWithMessage">Should be set to <c>null</c> if request was successful, to body of the response in case of errors and to '<c>user</c>' in case of upload being cancelled by user.</param>
        private IEnumerator NotifyFileUploadFinished(CreatubblesApiClient creatubbles, string pingUrl, string abortedWithMessage = null)
        {
            HttpRequest request = creatubbles.CreatePutUploadFinishedRequest(pingUrl);

            yield return creatubbles.SendRequest(request);

            if (request.IsAnyError)
            {
                FinishWithSystemOrInternalErrors(request);
                yield break;
            }
        }

        /// <summary>
        /// Finishes the upload session and reports encountered errors.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <typeparam name="T">The response type of the request.</typeparam>
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

        /// <summary>
        /// Finishes the upload session and reports encountered errors.
        /// </summary>
        /// <param name="request">Request.</param>
        private void FinishWithSystemOrInternalErrors(HttpRequest request)
        {
            if (request.IsSystemError)
            {
                FinishWithSystemError(request);
            }
            else if (request.IsHttpError)
            {
                FinishWithInternalError("Response code '" + request.ResponseCode + "' and body '" + request.RawHttpError + "'");
            }
            else
            {
                FinishWithInternalError(InternalErrorUnknownError);
            }
        }

        /// <summary>
        /// Finishes upload session with system error.
        /// </summary>
        /// <param name="request">Request.</param>
        private void FinishWithSystemError(HttpRequest request)
        {
            IsSystemError = true;
            SystemError = request.SystemError;
            IsDone = true;
        }

        /// <summary>
        /// Finishes upload session with API errors.
        /// </summary>
        /// <param name="request">Request.</param>
        private void FinishWithApiErrors<T>(ApiRequest<T> request)
        {
            IsApiError = true;
            ApiErrors = request.apiErrors;
            IsDone = true;
        }

        /// <summary>
        /// Finishes upload session with internal error.
        /// </summary>
        /// <param name="request">Request.</param>
        private void FinishWithInternalError(string message)
        {
            IsInternalError = true;
            InternalError = message;
            IsDone = true;
        }

        /// <summary>
        /// Resets error and status flags.
        /// </summary>
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
