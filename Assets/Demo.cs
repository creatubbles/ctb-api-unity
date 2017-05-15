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

    private OperationStatus publicAuthenticationStatus = OperationStatus.Idle;
    private OperationStatus privateAuthenticationStatus = OperationStatus.Idle;
    private OperationStatus getLandingUrlsStatus = OperationStatus.Idle;
    private OperationStatus getCreationStatus = OperationStatus.Idle;
    private OperationStatus uploadStatus = OperationStatus.Idle;

    private string privateAuthenticationToken = SecretData.PrivateAuthenticationToken;
    private string getCreationId = "";
    private string uploadGalleryId = "";
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
        var margin = 15;
        var containerWidth = Screen.width - 2 * margin;
        var elementHeight = 22;
        var elementSpacing = 6;
        var positionYDelta = elementHeight + elementSpacing;
        var subElementMargin = 10;
        var subElementWidth = containerWidth - subElementMargin;

        // PUBLIC AUTHENTICATION
        var currentPositionX = margin;
        var currentPositionY = margin;
        var subElementPositionX = currentPositionX + subElementMargin;
        GUI.Label(new Rect(currentPositionX, currentPositionY, containerWidth, elementHeight), "PUBLIC AUTHENTICATION");

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Status: " + publicAuthenticationStatus);

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Authenticate"))
        {
            StartCoroutine(AuthenticatePublicOAuth());
        }

        // PRIVATE AUTHENTICATION
        currentPositionY += positionYDelta;
        GUI.Label(new Rect(currentPositionX, currentPositionY, containerWidth, elementHeight), "PRIVATE AUTHENTICATION");

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Status: " + privateAuthenticationStatus);

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Provide access token: ");

        currentPositionY += positionYDelta;
        privateAuthenticationToken = GUI.TextField(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), privateAuthenticationToken);

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Authenticate"))
        {
            StartCoroutine(AuthenticatePrivateAndGetLoggedInUser());
        }

        // GET LANDING URLs
        currentPositionY += positionYDelta;
        GUI.Label(new Rect(currentPositionX, currentPositionY, containerWidth, elementHeight), "GET LANDING URLS");

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Status: " + getLandingUrlsStatus);

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Get public URLs"))
        {
            StartCoroutine(GetLandingUrls(Request.AuthorizationType.Public));
        }

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Get private URLs"))
        {
            StartCoroutine(GetLandingUrls(Request.AuthorizationType.Private));
        }

        // GET CREATION
        currentPositionY += positionYDelta;
        GUI.Label(new Rect(currentPositionX, currentPositionY, containerWidth, elementHeight), "GET CREATION");

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Status: " + getCreationStatus);

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Provide creation ID: ");

        currentPositionY += positionYDelta;
        getCreationId = GUI.TextField(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), getCreationId);

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Get creation"))
        {
            StartCoroutine(GetCreation(getCreationId));
        }

        // UPLOAD
        currentPositionY += positionYDelta;
        GUI.Label(new Rect(currentPositionX, currentPositionY, containerWidth, elementHeight), "UPLOAD CREATION");

        currentPositionY += positionYDelta;
        GUI.DrawTexture(new Rect(subElementPositionX, currentPositionY, elementHeight, elementHeight), imageTexture);

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Status: " + uploadStatus);

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Creation name: ");

        currentPositionY += positionYDelta;
        uploadCreationName = GUI.TextField(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), uploadCreationName);

        currentPositionY += positionYDelta;
        GUI.Label(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Gallery ID (to submit creation to): ");

        currentPositionY += positionYDelta;
        uploadGalleryId = GUI.TextField(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), uploadGalleryId);

        currentPositionY += positionYDelta;
        if (GUI.Button(new Rect(subElementPositionX, currentPositionY, subElementWidth, elementHeight), "Upload"))
        {
            StartCoroutine(UploadCreation(uploadCreationName, uploadGalleryId));
        }
    }

    #endregion

    #region Creatubbles methods

    IEnumerator AuthenticatePublicOAuth()
    {
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
        Debug.Log("Received public token: " + publicTokenRequest.ParsedResponse.token + ", " + publicTokenRequest.ParsedResponse.tokenType);
    }

    IEnumerator AuthenticatePrivateAndGetLoggedInUser()
    {
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
        Debug.Log("Loaded user: " + userRequest.ParsedResponse.display_name + " from " + userRequest.ParsedResponse.country_name + "\n" + userRequest.ParsedResponse.ToString());
    }

    IEnumerator GetLandingUrls(Request.AuthorizationType authorizationType)
    {
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
        Debug.Log("Fetched URLS: \n" + String.Join("\n", urlsRequest.ParsedResponse.Select(url => url.ToString()).ToArray()));
    }

    IEnumerator GetCreation(string creationId)
    {
        getCreationStatus = OperationStatus.Idle;

        // get creation
        var getCreationRequest = new GetCreationRequest(SecretData.CreationId);

        getCreationStatus = OperationStatus.InProgress;
        yield return StartCoroutine(creatubbles.Send(getCreationRequest));

        if (getCreationRequest.IsError)
        {
            getCreationStatus = OperationStatus.Failure;
            LogErrors("Get creation failed:", getCreationRequest.Errors);
            yield break;
        }

        getCreationStatus = OperationStatus.Success;
        Debug.Log("Fetched creation: " + getCreationRequest.ParsedResponse.ToString());
    }

    IEnumerator UploadCreation(string creationName, string galleryId)
    {
        uploadStatus = OperationStatus.Idle;

        var imageData = imageTexture.EncodeToPNG();
        var imageExtension = UploadExtension.PNG;

        var newCreationData = new NewCreationData(imageData, imageExtension);
        newCreationData.name = creationName;
        newCreationData.creationMonth = 3;
        newCreationData.creationYear = 2017;
        newCreationData.galleryId = galleryId;

        uploadSession = new CreationUploadSession(creatubbles, newCreationData);

        Debug.Log("Upload started");

        uploadStatus = OperationStatus.InProgress;
        yield return StartCoroutine(uploadSession.Upload());

        if (uploadSession.IsCancelled)
        {
            uploadStatus = OperationStatus.Cancelled;
            Debug.LogWarning("Upload cancelled");
            yield break;
        }
        else if (uploadSession.IsError)
        {
            uploadStatus = OperationStatus.Failure;
            LogErrors("Upload failed!", uploadSession.Errors);
            yield break;
        }

        uploadStatus = OperationStatus.Success;
        Debug.Log("Upload completed!");
    }

    private void LogErrors(string title, IList<RequestError> errors)
    {
        Debug.LogError(title + "\n" + String.Join("\n", errors.Select(e => e.ToString()).ToArray()));
    }

    #endregion
}
