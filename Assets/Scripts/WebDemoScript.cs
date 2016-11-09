//
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
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());
//        StartCoroutine(GetLandingUrls());
        StartCoroutine(LogIn(SecretData.Username, SecretData.Password));
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    IEnumerator GetLandingUrls()
    {
        ApiRequest<LandingUrlsResponse> request = creatubbles.CreateGetLandingUrlsRequest();

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendPublicRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors<LandingUrlsResponse>(request);
            yield break;
        }

        LandingUrlsResponse response = request.data;
        if (response != null && response.data != null && response.data.Length > 0)
        {
            Log("Response data: " + response.data[0].ToString());
        }
    }

    #region Log in and get profile methods

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

        StartCoroutine(GetLoggedInUser());
    }

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

        // TODO - if data.data == null - report error
        Log("Data: " + request.data.data.ToString());

        //            StartCoroutine(NewCreation());
        StartCoroutine(GetCreation("4NKhBZLU"));
    }

    #endregion

    #region Upload Creation methods

    IEnumerator NewCreation()
    {
        NewCreationData creationData = new NewCreationData("http://www.dailybackgrounds.com/wp-content/gallery/sweet-cat/sweet-cat-prayering-image.jpg", UploadExtension.JPG);
        creationData.name = "Awesome Unity Creation";
        creationData.creationMonth = 10;
        creationData.creationYear = 2016;

        ApiRequest<CreationGetResponse> request = creatubbles.CreateNewCreationRequest(creationData);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendSecureRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            yield break;
        }

        // TODO - if data.data == null - report error
        Log("Data: " + request.data.data.ToString());
    }

    IEnumerator GetCreation(string creationId)
    {
        ApiRequest<CreationGetResponse> request = creatubbles.CreateGetCreationRequest(creationId);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendSecureRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            yield break;
        }

        // TODO - if data.data == null - report error
        Log("Creation: " + request.data.data.ToString());
        StartCoroutine(PostCreationsUpload(creationId));
    }

    IEnumerator PostCreationsUpload(string creationId)
    {
        ApiRequest<CreationsUploadPostResponse> request = creatubbles.CreatePostCreationUploadRequest(creationId, UploadExtension.PNG);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendSecureRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            yield break;
        }

        // TODO - if data.data == null - report error
        Log("Creation upload: " + request.data.data.ToString());
        CreationsUploadAttributesDto upload = request.data.data.attributes;
        StartCoroutine(PutUploadFile(upload.url, upload.content_type, upload.ping_url));
    }

    IEnumerator PutUploadFile(string uploadUrl, string contentType, string pingUrl)
    {
        byte[] data = texture.EncodeToPNG();
        ApiRequestWithEmptyResponseData request = creatubbles.CreatePutUploadFileRequest(uploadUrl, contentType, data);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            // TODO - call PutUploadFileUploaded with abortedWithMessage
            yield break;
        }

        Log("Success");
        StartCoroutine(PutUploadFileUploaded(pingUrl));
    }

    IEnumerator PutUploadFileUploaded(string pingUrl)
    {
        ApiRequestWithEmptyResponseData request = creatubbles.CreatePutUploadFinishedRequest(pingUrl);

        Log("Sending request: " + request.Url);

        yield return creatubbles.SendSecureRequest(request);

        if (request.IsAnyError)
        {
            HandleApiErrors(request);
            yield break;
        }

        Log("Success");
    }

    #endregion

    #region Helper methods

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
        else
        {
            Log("Something went wrong");
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
            Log("API error: " + request.apiErrors[0].title);
        }
        else
        {
            Log("Something went wrong");
        }
    }

    private void Log(string text)
    {
        textControl.text += "\n\n" + text;
        Debug.Log(text);
    }

    #endregion
}
