//
// Demo.cs
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
using System.Collections;
using UnityEngine;
using Creatubbles.Api;
using Creatubbles.Api.Data;
using Creatubbles.Api.Parsers;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Creatubbles.Api.Storage;
using Creatubbles.Api.Helpers;
using Creatubbles.Api.Requests;

public class Demo: MonoBehaviour, ICoroutineStarter
{
    public Texture2D imageTexture;
    private CreatubblesApiClient creatubbles;
    private CreationUploadSession uploadSession;
    private float lastProgress = 0;

    void Awake()
    {
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage(), this);
    }

    // Use this for initialization
    IEnumerator Start()
    {
        yield return StartCoroutine(UploadCreation());
    }

    // Update is called once per frame
    void Update()
    {
        float currentProgress = uploadSession == null ? 0 : uploadSession.FileUploadProgress;
        if (Math.Abs(currentProgress - lastProgress) > 0.1)
        {
            Debug.Log("Upload progress: " + currentProgress);
            lastProgress = currentProgress;
        }
    }

    IEnumerator UploadCreation()
    {
        // set authentication token for upload requests to pass authorization
        creatubbles.SetAuthenticationToken(SecretData.PrivateAuthenticationToken);

        var imageData = imageTexture.EncodeToPNG();
        var imageExtension = UploadExtension.PNG;

        var newCreationData = new NewCreationData(imageData, imageExtension);
        newCreationData.name = "[NEW] Unity4 test!!!";
        newCreationData.creationMonth = 3;
        newCreationData.creationYear = 2017;
        newCreationData.galleryId = SecretData.GalleryId;

        uploadSession = new CreationUploadSession(creatubbles, newCreationData);

        Debug.Log("Beginning upload...");

        yield return StartCoroutine(uploadSession.Upload());

        if (uploadSession.IsCancelled)
        {
            yield break;
        }
        else if (uploadSession.IsError)
        {
            LogErrors("Upload failed!", uploadSession.Errors);
            yield break;
        }

        Debug.Log("Upload completed!");
    }

    private void LogErrors(string title, IList<RequestError> errors)
    {
        Debug.LogWarning(title);
        foreach (var error in errors)
        {
            Debug.LogError(error);
        }
    }
}
