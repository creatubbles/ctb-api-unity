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
    enum OperationStatus
    {
        Idle,
        InProgress,
        Success,
        Failure,
        Cancelled
    }

    #region Required dependencies fields

    public Texture2D imageTexture;
    private CreatubblesApiClient creatubbles;
    private CreationUploadSession uploadSession;

    #endregion

    #region Progress tracking fields

    private Vector2 scrollPosition = Vector2.zero;
    private string logText = string.Empty;

    private OperationStatus publicAuthenticationStatus = OperationStatus.Idle;
    private OperationStatus privateAuthenticationStatus = OperationStatus.Idle;
    private OperationStatus getLandingUrlsStatus = OperationStatus.Idle;
    private OperationStatus getCreationStatus = OperationStatus.Idle;
    private OperationStatus uploadStatus = OperationStatus.Idle;

    private string privateAuthenticationToken = SecretData.PrivateAuthenticationToken;
    private string getCreationId = string.Empty;
    private string uploadGalleryId = string.Empty;
    private string uploadCreationName = "Unity 4 creation";

    #endregion

    #region Unity initialization and update

    void Awake()
    {
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage(), this);
    }

    // due to Creatubbles SDK depending on WWW in Unity 4, actual progress reported will always be either 0 or 1, so there's no point in tracking it in Update()
    void Update()
    {
    }

    #endregion

    #region OnGUI

    void OnGUI()
    {
        var margin = 20;
        var contentHeight = Screen.height - 2 * margin;
        var contentWidth = Screen.width - 2 * margin;
        var containerWidth = (float)(0.5 * contentWidth) + 100;

        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(containerWidth), GUILayout.Height(contentHeight));

        DrawPublicAuthentication();

        DrawPrivateAuthentication();

        DrawGetLandingUrls();

        DrawGetCreation();

        DrawUpload();

        GUILayout.EndScrollView();

        logText = GUILayout.TextArea(logText, GUILayout.Width(contentWidth - containerWidth), GUILayout.Height(contentHeight));

        GUILayout.EndHorizontal();

    }

    private void DrawPublicAuthentication()
    {
        GUILayout.Label("PUBLIC AUTHENTICATION");
        DrawStatus(publicAuthenticationStatus);
        if (GUILayout.Button("Authenticate"))
        {
            StartCoroutine(AuthenticatePublicOAuth());
        }
    }

    private void DrawPrivateAuthentication()
    {
        DrawSeparator();
        GUILayout.Label("PRIVATE AUTHENTICATION");
        DrawStatus(privateAuthenticationStatus);
        GUILayout.Label("Provide access token:");
        privateAuthenticationToken = GUILayout.TextField(privateAuthenticationToken);
        if (GUILayout.Button("Authenticate"))
        {
            StartCoroutine(AuthenticatePrivateAndGetLoggedInUser());
        }
    }

    private void DrawGetLandingUrls()
    {
        DrawSeparator();
        GUILayout.Label("GET LANDING URLS");
        DrawStatus(getLandingUrlsStatus);
        if (GUILayout.Button("Get public URLs"))
        {
            StartCoroutine(GetLandingUrls(Request.AuthorizationType.Public));
        }
        if (GUILayout.Button("Get private URLs"))
        {
            StartCoroutine(GetLandingUrls(Request.AuthorizationType.Private));
        }
    }

    private void DrawGetCreation()
    {
        DrawSeparator();
        GUILayout.Label("GET CREATION");
        DrawStatus(getCreationStatus);
        GUILayout.Label("Provide creation ID:");
        getCreationId = GUILayout.TextField(getCreationId);
        if (GUILayout.Button("Get creation"))
        {
            StartCoroutine(GetCreation(getCreationId));
        }
    }

    private void DrawUpload()
    {
        DrawSeparator();
        GUILayout.Label("UPLOAD CREATION");
        GUILayout.Box(imageTexture);
        DrawStatus(uploadStatus);
        GUILayout.Label("Provide creation name:");
        uploadCreationName = GUILayout.TextField(uploadCreationName);
        GUILayout.Label("Gallery ID (to submit creation to): ");
        uploadGalleryId = GUILayout.TextField(uploadGalleryId);
        if (GUILayout.Button("Upload"))
        {
            StartCoroutine(UploadCreation(uploadCreationName, uploadGalleryId));
        }
    }

    private void DrawSeparator()
    {
        GUILayout.Label("-------------------");
    }

    private void DrawStatus(OperationStatus status)
    {
        GUILayout.Label("Status: " + status);
    }

    #endregion

    #region Creatubbles methods

    IEnumerator AuthenticatePublicOAuth()
    {
        ClearLog();

        privateAuthenticationStatus = OperationStatus.Idle;
        var publicTokenRequest = new GetPublicOAuthTokenRequest();

        publicAuthenticationStatus = OperationStatus.InProgress;
        yield return StartCoroutine(creatubbles.AuthenticatePublicOAuth(publicTokenRequest));

        if (publicTokenRequest.IsError)
        {
            publicAuthenticationStatus = OperationStatus.Failure;
            LogErrors("Failed to retrieve public token:", publicTokenRequest.Errors);
            yield break;
        }

        publicAuthenticationStatus = OperationStatus.Success;
        LogInfo("Fetched public token: " + publicTokenRequest.ParsedResponse.token + ", " + publicTokenRequest.ParsedResponse.tokenType);
    }

    IEnumerator AuthenticatePrivateAndGetLoggedInUser()
    {
        ClearLog();

        privateAuthenticationStatus = OperationStatus.Idle;

        // since we're taking the authentication token from the outside source (e.g. code flow), we only need to pass it to the API client to store locally and use it for authenticating requests
        creatubbles.SetAuthenticationToken(privateAuthenticationToken);

        // we perform fetch user request to verify, that the provided token is valid
        var userRequest = new GetLoggedInUserRequest();

        privateAuthenticationStatus = OperationStatus.InProgress;
        yield return StartCoroutine(creatubbles.Send(userRequest));

        if (userRequest.IsError)
        {
            privateAuthenticationStatus = OperationStatus.Failure;
            LogErrors("Get logged in user failed:", userRequest.Errors);
            yield break;
        }

        privateAuthenticationStatus = OperationStatus.Success;
        LogInfo("Fetched user: " + userRequest.ParsedResponse.display_name + " from " + userRequest.ParsedResponse.country_name + "\n" + userRequest.ParsedResponse.ToString());
    }

    IEnumerator GetLandingUrls(Request.AuthorizationType authorizationType)
    {
        ClearLog();

        getLandingUrlsStatus = OperationStatus.Idle;

        var urlsRequest = new GetLandingUrlsRequest(authorizationType);

        getLandingUrlsStatus = OperationStatus.InProgress;
        yield return StartCoroutine(creatubbles.Send(urlsRequest));

        if (urlsRequest.IsError)
        {
            getLandingUrlsStatus = OperationStatus.Failure;
            LogErrors("Get landing URLs failed:", urlsRequest.Errors);
            yield break;
        }

        getLandingUrlsStatus = OperationStatus.Success;
        LogInfo("Fetched URLS: \n" + String.Join("\n", urlsRequest.ParsedResponse.Select(url => url.ToString()).ToArray()));
    }

    IEnumerator GetCreation(string creationId)
    {
        ClearLog();

        getCreationStatus = OperationStatus.Idle;

        var getCreationRequest = new GetCreationRequest(creationId);

        getCreationStatus = OperationStatus.InProgress;
        yield return StartCoroutine(creatubbles.Send(getCreationRequest));

        if (getCreationRequest.IsError)
        {
            getCreationStatus = OperationStatus.Failure;
            LogErrors("Get creation failed:", getCreationRequest.Errors);
            yield break;
        }

        getCreationStatus = OperationStatus.Success;
        LogInfo("Fetched creation: " + getCreationRequest.ParsedResponse.ToString());
    }

    IEnumerator UploadCreation(string creationName, string galleryId)
    {
        ClearLog();

        uploadStatus = OperationStatus.Idle;

        var imageData = imageTexture.EncodeToPNG();
        var imageExtension = UploadExtension.PNG;

        var newCreationData = new NewCreationData(imageData, imageExtension);
        newCreationData.name = creationName;
        newCreationData.creationMonth = 3;
        newCreationData.creationYear = 2017;
        newCreationData.galleryId = galleryId;

        uploadSession = new CreationUploadSession(creatubbles, newCreationData);

        LogInfo("Upload started");

        uploadStatus = OperationStatus.InProgress;
        yield return StartCoroutine(uploadSession.Upload());

        if (uploadSession.IsCancelled)
        {
            uploadStatus = OperationStatus.Cancelled;
            LogInfo("Upload cancelled");
            yield break;
        }
        else if (uploadSession.IsError)
        {
            uploadStatus = OperationStatus.Failure;
            LogErrors("Upload failed!", uploadSession.Errors);
            yield break;
        }

        uploadStatus = OperationStatus.Success;
        LogInfo("Upload completed!");
    }

    private void ClearLog()
    {
        logText = string.Empty;
    }

    private void LogInfo(string text)
    {
        logText += "\n" + text;
        Debug.Log(text);
    }

    private void LogErrors(string title, IList<RequestError> errors)
    {
        var text = title + "\n" + String.Join("\n", errors.Select(e => e.ToString()).ToArray());
        logText += "\n" + text;
        Debug.LogError(text);
    }

    #endregion
}
