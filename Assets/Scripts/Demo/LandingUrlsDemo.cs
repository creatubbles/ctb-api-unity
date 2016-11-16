//
//  LandingUrlsDemo.cs
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
using UnityEngine.UI;
using Creatubbles.Api;
using Creatubbles.Api.Requests;
using Creatubbles.Api.Storage;
using System.Collections.Generic;
using UnityEngine.Events;

public class LandingUrlsDemo: MonoBehaviour 
{
    private const string StatusGettingUrls = "Getting URLs";
    private const string StatusSuccess = "Success";
    private const string StatusFailure = "Failed";
    private const string StatusCancelled = "Cancelled";

    public Dropdown typeDropdown;
    public Text statusText;
    public Button getUrlsButton;
    public InputField landingUrlsResultInput;

    private CreatubblesApiClient creatubbles;

    private HttpRequest.Type Type { get { return typeDropdown.value == 0 ? HttpRequest.Type.Public : HttpRequest.Type.Private; } }
    private string Status { set { statusText.text = value; } }

    void Start()
    {
        // configuration - in order for the API client to function properly it requires a simple configuration class containing information such as base URL, app ID and secret
        // secureStorage - currently only InMemoryStorage is available as part of the SDK, however user can provide his / her own implementation
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());
    }

    void Update()
    {
    }

    public void GetUrlsButtonClicked()
    {
        StartCoroutine(GetLandingUrls());
    }

    private IEnumerator GetLandingUrls()
    {
        Started();

        ApiRequest<LandingUrlsResponse> request = creatubbles.CreateGetLandingUrlsRequest(Type);

        // getting landing URLs is a public request
        yield return creatubbles.SendRequest(request);

        // cancelling request will usually cause a System or Internal error to be reported, so we should always check for cancellation before checking for errors
        if (request.IsCancelled)
        {
            Cancelled();
            yield break;
        }

        if (request.IsAnyError)
        {
            if (request.IsSystemError)
            {
                Debug.Log("System error: " + request.SystemError);
            }
            else if (request.IsHttpError && request.apiErrors != null && request.apiErrors.Length > 0)
            {
                // if unauthorized, log user out
                Debug.Log("API error: " + request.apiErrors[0].title);
            }

            Failed();
            yield break;
        }

        // do something useful with the result
        LandingUrlsResponse response = request.Data;
        if (response == null || response.data == null || response.data.Length <= 0)
        {
            Failed();
            yield break;
        }

        foreach (var urlContainer in response.data)
        {
            landingUrlsResultInput.text += urlContainer.Url + "\n";
        }
        Successful();
    }

    private void Started()
    {
        landingUrlsResultInput.text = "";
        Status = StatusGettingUrls;
        getUrlsButton.interactable = false;
    }

    private void Successful()
    {
        Status = StatusSuccess;
        getUrlsButton.interactable = true;
    }

    private void Failed()
    {
        Status = StatusFailure;
        getUrlsButton.interactable = true;
    }

    private void Cancelled()
    {
        Status = StatusCancelled;
        getUrlsButton.interactable = true;
    }
}
    