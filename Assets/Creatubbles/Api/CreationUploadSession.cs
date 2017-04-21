//
// CreationUploadSession.cs
// Creatubbles API Client Unity SDK
//
// Copyright (c) 2017 Creatubbles Pte. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Linq;
using System.Collections;
using Creatubbles.Api.Data;
using UnityEngine;
using System.Collections.Generic;
using Creatubbles.Api.Parsers;
using Creatubbles.Api.Requests;
using Creatubbles.Api.Helpers;

namespace Creatubbles.Api
{
    /// <summary>
    /// Performs series of requests allocating creation, uploading file, updating creation and notifying backend when operation finishes.
    /// </summary>
    /// <remarks>
    /// More info at https://stateoftheart.creatubbles.com/api/#creation-upload.
    /// </remarks>
    public class CreationUploadSession
    {
        private const string NotifyFileUploadFinishedMessageUserCancelled = "user";

        private CreatubblesApiClient creatubbles;
        private NewCreationData creationData;

        private ICoroutineStarter CoroutineStarter { get { return creatubbles.coroutineStarter; } }

        private Request currentRequest;
        private Request uploadRequest;

        /// <summary>
        /// Determines if session was cancelled.
        /// </summary>
        /// <value><c>true</c> if session was cancelled, otherwise <c>false</c>.</value>
        /// <see cref="ICancellable.Cancel()"/>
        /// <c>false</c>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Indicates whether upload session is finished.
        /// </summary>
        /// <value><c>true</c> if all requests completed, upload was cancelled or an error occured, otherwise <c>false</c>.</value>
        public bool IsDone { get; private set; }

        public bool IsError { get { return Errors.Any(); } }
        public readonly List<RequestError> Errors = new List<RequestError>();

        public float FileUploadProgress { get { return uploadRequest == null ? 0 : uploadRequest.UploadProgress; } }

        public CreationUploadSession(CreatubblesApiClient creatubbles, NewCreationData creationData)
        {
            this.creatubbles = creatubbles;
            this.creationData = creationData;
        }

        public IEnumerator Upload()
        {
            ResetFlagsAndFields();

            var creationId = creationData.creationId;

            var creationNotExistingOnBackend = creationId == null;
            if (creationNotExistingOnBackend)
            {
                // setup new creation on backend
                var newCreationRequest = AssignedAsCurrentRequest(new NewCreationRequest(creationData));

                yield return CoroutineStarter.StartCoroutine(creatubbles.Send(newCreationRequest));

                if (newCreationRequest.IsError)
                {
                    FinishWithErrors();
                    yield break;
                }

                creationId = newCreationRequest.ParsedResponse.id;
            }

            if (IsCancelled) { yield break; }

            // setup upload
            var creationUploadSetupRequest = AssignedAsCurrentRequest(new CreationUploadSetupRequest(creationId, creationData.uploadExtension));

            yield return CoroutineStarter.StartCoroutine(creatubbles.Send(creationUploadSetupRequest));

            if (creationUploadSetupRequest.IsError)
            {
                FinishWithErrors();
                yield break;   
            }

            var uploadConfiguration = creationUploadSetupRequest.ParsedResponse;

            if (IsCancelled)
            {
                // notify upload cancelled
                var notifyUploadCancelledRequest = AssignedAsCurrentRequest(new NotifyUploadFinishedRequest(uploadConfiguration.ping_url, NotifyFileUploadFinishedMessageUserCancelled));
                yield return CoroutineStarter.StartCoroutine(creatubbles.Send(notifyUploadCancelledRequest));
                yield break; 
            }

            // upload file to Amazon
            var amazonUploadRequest = AssignedAsCurrentRequest(new AmazonUploadRequest(creationData.data, uploadConfiguration));
            uploadRequest = amazonUploadRequest;

            yield return CoroutineStarter.StartCoroutine(creatubbles.Send(amazonUploadRequest));

            if (amazonUploadRequest.IsError)
            {
                var notifyUploadCancelledRequest = AssignedAsCurrentRequest(new NotifyUploadFinishedRequest(uploadConfiguration.ping_url, amazonUploadRequest.TextResponse));
                yield return CoroutineStarter.StartCoroutine(creatubbles.Send(notifyUploadCancelledRequest));

                FinishWithErrors(amazonUploadRequest);
                yield break;
            }

            // notify backend that file upload is complete
            var notifyUploadCompleteRequest = AssignedAsCurrentRequest(new NotifyUploadFinishedRequest(uploadConfiguration.ping_url));

            yield return CoroutineStarter.StartCoroutine(creatubbles.Send(notifyUploadCompleteRequest));

            if (notifyUploadCompleteRequest.IsError)
            {
                FinishWithErrors();
                yield break;
            }

            if (IsCancelled) { yield break; }

            // if galleryId is specified, submit the creation to the gallery
            if (!String.IsNullOrEmpty(creationData.galleryId) && !String.IsNullOrEmpty(creationId))
            {
                var gallerySubmissionRequest = AssignedAsCurrentRequest(new SubmitCreationToGalleryRequest(creationData.galleryId, creationId));

                yield return CoroutineStarter.StartCoroutine(creatubbles.Send(gallerySubmissionRequest));

                if (gallerySubmissionRequest.IsError)
                {
                    FinishWithErrors();
                    yield break;
                }
            }

            IsDone = true;
        }

        /// <summary>
        /// Cancels the upload session, if <c>CreationUploadSession.IsDone</c> is <c>false</c>.
        /// </summary>
        public void Cancel()
        {
            if (IsDone || IsError)
            {
                return;
            }

            if (currentRequest != null)
            {
                currentRequest.Cancel();
            }
            IsCancelled = true;
        }

        private T AssignedAsCurrentRequest<T>(T request) where T: Request
        {
            currentRequest = request;
            return request;
        }

        private void FinishWithErrors(Request request = null)
        {
            var failedRequest = request != null ? request : currentRequest;
            Errors.AddRange(failedRequest.Errors);

            IsDone = true;
        }

        /// <summary>
        /// Resets error and status flags.
        /// </summary>
        private void ResetFlagsAndFields()
        {
            IsDone = false;
            IsCancelled = false;
            Errors.Clear();
            currentRequest = null;
            uploadRequest = null;
        }
    }
}

