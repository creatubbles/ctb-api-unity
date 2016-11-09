﻿//
//  WebDemoScript.cs
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

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections.Generic;
using Creatubbles.Api;

public class WebDemoScript: MonoBehaviour
{
    public Text textControl;
    public Texture2D texture;

    private CreatubblesApiClient creatubbles;

    // Use this for initialization
    void Start()
    {
        // configuration - in order for the API client to function properly it requires a simple configuration class containing information such as base URL, app ID and secret
        //                 such class must be implementing IApiConfiguration inteface (see CreatubblesConfiguration for example and details)
        // secureStorage - currently only InMemoryStorage is available but SDK user can provide his / her own implementation until a more secure option is available as part of the SDK
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());

        StartCoroutine(SendRequests());
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    // example of getting landing URLs, logging in and uploading a creation
    IEnumerator SendRequests()
    {
        // get landing URLs (public request)
        yield return GetLandingUrls();

        Log("-------");
        // login and get user profile (OAuth and private request)
        yield return LogIn(SecretData.Username, SecretData.Password);

        Log("-------");
        // upload new creation
        yield return UploadCreation("Unity Creation #2");
    }

    #region API related methods

    IEnumerator GetLandingUrls()
    {
        ApiRequest<LandingUrlsResponse> request = creatubbles.CreateGetLandingUrlsRequest();

        Log("Sending request: " + request.Url);

        // getting landing URLs is a public request
        yield return creatubbles.SendPublicRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors<LandingUrlsResponse>(request);
            yield break;
        }

        // do something useful with the result
        LandingUrlsResponse response = request.data;
        if (response != null && response.data != null && response.data.Length > 0)
        {
            Log("Response data: " + response.data[0].ToString());
        }
    }

    // creates and sends a log in request, if successful saves the returned token in ISecureStorage instance
    // after log in request is complete user profile is fetched from the API
    IEnumerator LogIn(string username, string password)
    {
        OAuthRequest request = creatubbles.CreatePostAuthenticationUserTokenRequest(username, password);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendLogInRequest(request);

        if (request.IsAnyError)
        {
            HandleOAuthError(request);
            yield break;
        }

        yield return GetLoggedInUser();
    }

    // creates a new Creation entity and uploads an image with it
    // TODO - upload progress reporting and abort features are work in progress
    IEnumerator UploadCreation(string name)
    {
        byte[] imageData = texture.EncodeToPNG();

        NewCreationData creationData = new NewCreationData(imageData, UploadExtension.PNG);
        creationData.name = name;
        creationData.creationMonth = DateTime.Now.Month;
        creationData.creationYear = DateTime.Now.Year;

        CreationUploadSession creationUploadSession = new CreationUploadSession(creationData);

        yield return creationUploadSession.Upload(creatubbles);

        if (creationUploadSession.IsAnyError)
        {
            if (creationUploadSession.IsSystemError)
            {
                Log("System error: " + creationUploadSession.SystemError);
            }
            else if (creationUploadSession.IsApiError && creationUploadSession.ApiErrors != null && creationUploadSession.ApiErrors.Length > 0)
            {
                // if unauthorized, log user out
                Log("API error: " + creationUploadSession.ApiErrors[0].title);
            }
            else if (creationUploadSession.IsInternalError)
            {
                Log("Internal erro: " + creationUploadSession.InternalError);
            }
        }
    }

    #endregion

    #region Helper methods

    // creates and sends request for fetching current user data (user must be already logged in, otherwise request will fail with 'unauthorized' error)
    IEnumerator GetLoggedInUser()
    {
        ApiRequest<LoggedInUserResponse> request = creatubbles.CreateGetLoggedInUserRequest();

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendSecureRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            yield break;
        }

        if (request.data == null || request.data.data == null)
        {
            Debug.Log("Error: Invalid or missing data in response");
            yield break;
        }

        Log("Success with data: " + request.data.data.ToString());
    }

    private void HandleOAuthError(OAuthRequest request)
    {
        if (request.IsSystemError)
        {
            Log("System error: " + request.SystemError);
        }
        else if (request.IsOAuthError && request.oAuthError != null)
        {
            Log("API error: " + request.oAuthError.error_description);
        }
    }

    private void HandleApiErrors<T>(ApiRequest<T> request)
    {
        if (request.IsSystemError)
        {
            Log("System error: " + request.SystemError);
        }
        else if (request.IsApiError && request.apiErrors != null && request.apiErrors.Length > 0)
        {
            // if unauthorized, log user out
            Log("API error: " + request.apiErrors[0].title);
        }
    }

    private void Log(string text)
    {
        textControl.text += "\n\n" + text;
        Debug.Log(text);
    }

    #endregion
}
