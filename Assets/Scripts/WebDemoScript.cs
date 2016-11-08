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

    private const string AppAccessTokenKey = "ctb_app_access_token";
    private const string UserAccessTokenKey = "ctb_user_access_token";

    // Use this for initialization
    void Start()
    {
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());
        StartCoroutine(GetAppToken());
//        StartCoroutine(LogIn(SecretData.Username, SecretData.Password));
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    IEnumerator GetAppToken()
    {
        UnityWebRequest www = creatubbles.CreatePostAuthenticationApplicationTokenRequest();
        ApiRequest<string> request = new ApiRequest<string>(www);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (request.IsAnyError)
        {
            Log("Request error: " + www.error);
			Log("Error body: " + www.downloadHandler.text);

            yield break;
        }

        Log("[Response] code: " + www.responseCode + "\n body: " + www.downloadHandler.text);

        OAuthTokenReponse data = JsonUtility.FromJson<OAuthTokenReponse>(www.downloadHandler.text);

        StartCoroutine(GetLandingUrls(data.access_token));
    }

    IEnumerator GetLandingUrls(string accessToken)
    {
        ApiRequest<LandingUrlsResponse> request = creatubbles.CreateGetLandingUrlsRequest(accessToken);

        Log("Sending request: " + request.Url);

        yield return request.Send();

        // handle errors
        if (request.IsAnyError)
        {
            if (request.IsSystemError)
            {
                Log("System error: " + request.SystemError);
            }
            else if (request.IsApiError)
            {
                if (request.apiErrors != null && request.apiErrors.Length > 0)
                {
                    Log("API error: " + request.apiErrors[0].title);
                }
            }

            yield break;
        }

        // handle response
        LandingUrlsResponse response = request.data;
        if (response != null && response.data != null && response.data.Length > 0)
        {
            Log("Response data: " + response.data[0].ToString());
        }
    }

    IEnumerator LogIn(string username, string password)
    {
        UnityWebRequest www = creatubbles.CreatePostAuthenticationUserTokenRequest(username, password);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (www.isError)
        {
            Log("Request error: " + www.error);
        }
        else
        {
            Log("[Response] code: " + www.responseCode + ", body: " + www.downloadHandler.text);

            OAuthTokenReponse data = JsonUtility.FromJson<OAuthTokenReponse>(www.downloadHandler.text);

            // TODO - move to client
            creatubbles.secureStorage.SaveValue(UserAccessTokenKey, data.access_token);

            StartCoroutine(GetLoggedInUser());
        }
    }

    IEnumerator GetLoggedInUser()
    {
        string token = creatubbles.secureStorage.LoadValue(UserAccessTokenKey);
        UnityWebRequest www = creatubbles.CreateGetLoggedInUserRequest(token);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (www.isError)
        {
            Log("Request error: " + www.error);
        }
        else
        {
            Log("[Response] code: " + www.responseCode + ", body: " + www.downloadHandler.text);

            LoggedInUserResponse data = JsonUtility.FromJson<LoggedInUserResponse>(www.downloadHandler.text);

            Log("Data: " + data.data.ToString());

//            StartCoroutine(NewCreation(userToken));
            StartCoroutine(GetCreation("4NKhBZLU"));
        }
    }

    IEnumerator NewCreation(string userToken)
    {
        NewCreationData creationData = new NewCreationData("http://www.dailybackgrounds.com/wp-content/gallery/sweet-cat/sweet-cat-prayering-image.jpg", UploadExtension.JPG);
        creationData.name = "Awesome Unity Creation";
        creationData.creationMonth = 10;
        creationData.creationYear = 2016;

        UnityWebRequest www = creatubbles.CreateNewCreationRequest(userToken, creationData);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (www.isError)
        {
            Log("Request error: " + www.error);
        }
        else
        {
            Log("[Response] code: " + www.responseCode + ", body: " + www.downloadHandler.text);

            CreationGetResponse data = JsonUtility.FromJson<CreationGetResponse>(www.downloadHandler.text);
            Log("Data: " + data.data.ToString());
        }
    }

    IEnumerator GetCreation(string creationId)
    {
        string token = creatubbles.secureStorage.LoadValue(UserAccessTokenKey);
        UnityWebRequest www = creatubbles.CreateGetCreationRequest(token, creationId);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (www.isError)
        {
            Log("Request error: " + www.error);
        }
        else
        {
            Log("[Response] code: " + www.responseCode + ", body: " + www.downloadHandler.text);

            CreationGetResponse data = JsonUtility.FromJson<CreationGetResponse>(www.downloadHandler.text);
            Log("Creation: " + data.data.ToString());

            StartCoroutine(PostCreationsUpload(creationId));
        }
    }

    // TODO - add extension?
    IEnumerator PostCreationsUpload(string creationId)
    {
        string token = creatubbles.secureStorage.LoadValue(UserAccessTokenKey);
        UnityWebRequest www = creatubbles.CreatePostCreationUploadRequest(token, creationId, UploadExtension.PNG);

        Log("Sending request: " + www.url);

        yield return www.Send();

        if (www.isError)
        {
            Log("Request error: " + www.error);
        }
        else
        {
            Log("[Response] code: " + www.responseCode + ", body: " + www.downloadHandler.text);

            CreationsUploadPostResponse data = JsonUtility.FromJson<CreationsUploadPostResponse>(www.downloadHandler.text);
            Log("Creation upload: " + data.data.ToString());

            CreationsUploadAttributesDto upload = data.data.attributes;
            StartCoroutine(PutUploadFile(upload.url, upload.content_type, upload.ping_url));
        }
    }

    IEnumerator PutUploadFile(string uploadUrl, string contentType, string pingUrl)
    {
        string token = creatubbles.secureStorage.LoadValue(UserAccessTokenKey);
        byte[] data = texture.EncodeToPNG();
        UnityWebRequest request = creatubbles.CreatePutUploadFileRequest(token, uploadUrl, contentType, data);

        Log("Sending request: " + request.url);

        yield return request.Send();

        // TODO - how to check progress? upload handler needs to be available on the outside to be checked in update?

        if (request.isError)
        {
            Log("Request error: " + request.error);
        }
        else
        {
            Log("[Response] code: " + request.responseCode + ", body: " + request.downloadHandler.text);

            StartCoroutine(PutUploadFileUploaded(pingUrl));
        }
    }

    IEnumerator PutUploadFileUploaded(string pingUrl)
    {
        string token = creatubbles.secureStorage.LoadValue(UserAccessTokenKey);
        UnityWebRequest request = creatubbles.CreatePutUploadFinishedRequest(token, pingUrl);

        Log("Sending request: " + request.url);

        yield return request.Send();

        if (request.isError)
        {
            Log("Request error: " + request.error);
        }
        else
        {
            Log("[Response] code: " + request.responseCode + ", body: " + request.downloadHandler.text);
        }
    }

    private void Log(string text)
    {
        textControl.text += "\n\n" + text;
        Debug.Log(text);
    }
}
