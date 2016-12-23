//
//  UploadDemo.cs
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

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections.Generic;
using Creatubbles.Api;
using Creatubbles.Api.Storage;
using Creatubbles.Api.Requests;
using Creatubbles.Api.Data;

public class UploadDemo: MonoBehaviour
{
    private const string StatusUploading = "Uploading...";
    private const string StatusSuccess = "Success";
    private const string StatusFailure = "Failed";
    private const string StatusCancelled = "Cancelled";

    public Image progressImage;
    public InputField creationNameInput;
    public InputField galleryIdInput;
    public Button startButton;
    public Button cancelButton;
    public Text statusText;
    public Image textureImage;
    public InputField imageUrlInput;

    private CreatubblesApiClient creatubbles;
    private CreationUploadSession creationUploadSession;

    private string Status { set { statusText.text = value; } }
    private Texture2D imageTexture { get { return textureImage.mainTexture as Texture2D; } }
    private string ImageUrl { get { return imageUrlInput.text; } }

    // Use this for initialization.
    void Start()
    {
        // configuration - in order for the API client to function properly it requires a simple configuration class containing information such as base URL, app ID and secret
        // secureStorage - currently only InMemoryStorage is available as part of the SDK, however user can provide his / her own implementation
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());
    }
	
    // Update is called once per frame.
    void Update()
    {
        if (creationUploadSession != null && !creationUploadSession.IsCancelled)
        {
            progressImage.fillAmount = creationUploadSession.UploadProgress;
        }
    }

    public void StartUploadButtonClicked()
    {
        StartCoroutine(UploadCreation());
    }

    public void CancelUploadButtonClicked()
    {
        if (creationUploadSession != null)
        {
            creationUploadSession.Cancel();
        }
    }
        
    // Creates a new Creation entity and uploads an image with it.
    IEnumerator UploadCreation()
    {
        Started();

        NewCreationData creationData;
        // if user provided URL, prepare upload with URL
        if (ImageUrl != null && ImageUrl.Length > 0)
        {
            creationData = new NewCreationData(ImageUrl, UploadExtension.PNG);
        }
        // otherwise prepare upload with local texture
        else
        {
            byte[] imageData = imageTexture.EncodeToPNG();
            creationData = new NewCreationData(imageData, UploadExtension.PNG);
        }
        creationData.name = creationNameInput.text;
        creationData.galleryId = galleryIdInput.text;
        creationData.creationMonth = DateTime.Now.Month;
        creationData.creationYear = DateTime.Now.Year;

        creationUploadSession = new CreationUploadSession(creationData);

        yield return creationUploadSession.Upload(creatubbles);

        // cancelling upload session will usually cause a System or Internal error to be reported, so we should always check for cancellation before checking for errors
        if (creationUploadSession.IsCancelled)
        {
            Cancelled();
            yield break;
        }

        if (creationUploadSession.IsAnyError)
        {
            if (creationUploadSession.IsSystemError)
            {
                Debug.Log("System error: " + creationUploadSession.SystemError);
            }
            else if (creationUploadSession.IsApiError && creationUploadSession.ApiErrors != null && creationUploadSession.ApiErrors.Length > 0)
            {
                // if unauthorized, log user out
                Debug.Log("API error: " + creationUploadSession.ApiErrors[0].title);
            }
            else if (creationUploadSession.IsInternalError)
            {
                Debug.Log("Internal error: " + creationUploadSession.InternalError);
            }

            creationUploadSession = null;
            Failed();
            yield break;
        }

        creationUploadSession = null;
        Successful();
    }

    private void Started()
    {
        Status = StatusUploading;
        progressImage.fillAmount = 0;
        startButton.interactable = false;
        cancelButton.interactable = true;
    }

    private void Successful()
    {
        Status = StatusSuccess;
        startButton.interactable = true;
        cancelButton.interactable = false;
    }

    private void Failed()
    {
        Status = StatusFailure;
        startButton.interactable = true;
        cancelButton.interactable = false;
    }

    private void Cancelled()
    {
        Status = StatusCancelled;
        startButton.interactable = true;
        cancelButton.interactable = false;
    }
}
