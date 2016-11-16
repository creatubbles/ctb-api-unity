//
//  LogInDemo.cs
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

public class LogInDemo : MonoBehaviour 
{
    #region UI elements

    public InputField usernameInput;
    public InputField passwordInput;
    public Button logInButton;
    public Button logOutButton;
    public Text logInStatusText;

    #endregion

    private const string StatusNotLoggedIn = "Not logged in";
    private const string StatusLoggedInAs = "Logged in as: ";
    private const string StatusLoggingIn = "Logging in...";
    private const string StatusLogInCancelled = "Log in cancelled";
    private const string StatusLogInFailed = "Log in failed";
    private const string StatusGettingUserProfile = "Getting user profile...";

    private CreatubblesApiClient creatubbles;

    public string Status { set { logInStatusText.text = value; } }
    public bool LogInInteractable { set { logInButton.interactable = value; } }
    public bool LogOutInteractable { set { logOutButton.interactable = value; } }
    public string Username { get { return usernameInput.text; } }
    public string Password { get { return passwordInput.text; } }

	void Start()
    {
        // configuration - in order for the API client to function properly it requires a simple configuration class containing information such as base URL, app ID and secret
        // secureStorage - currently only InMemoryStorage is available as part of the SDK, however user can provide his / her own implementation
        creatubbles = new CreatubblesApiClient(new CreatubblesConfiguration(), new InMemoryStorage());

        Status = StatusNotLoggedIn;
        LogInInteractable = true;
        LogOutInteractable = false;
	}
	
	void Update()
    {
	}

    public void LogInButtonClicked()
    {
        StartCoroutine(LogIn(Username, Password));
    }

    public void LogOutButtonClicked()
    {
        LogInInteractable = true;
        LogOutInteractable = false;   
        creatubbles.LogOut();
        Status = StatusNotLoggedIn;
    }

    // Creates and sends a log in request, if successful saves the returned token in ISecureStorage instance.
    // After log in request is complete user profile is fetched from the API.
    private IEnumerator LogIn(string username, string password)
    {
        LogInStarted();

        // Getting OAuth user token
        OAuthRequest logInRequest = creatubbles.CreatePostAuthenticationUserTokenRequest(username, password);

        yield return creatubbles.SendLogInRequest(logInRequest);

        // cancelling request will usually cause a System or Internal error to be reported, so we should always check for cancellation before checking for errors
        if (logInRequest.IsCancelled)
        {
            LogInCancelled();
            yield break;
        }

        if (logInRequest.IsAnyError)
        {
            if (logInRequest.IsSystemError)
            {
                Debug.Log("System error: " + logInRequest.SystemError);
            }
            else if (logInRequest.IsHttpError && logInRequest.oAuthError != null)
            {
                Debug.Log("API error: " + logInRequest.oAuthError.error_description);
            }

            LogInFailed();
            yield break;
        }

        // Getting user profile
        ApiRequest<LoggedInUserResponse> userProfileRequest = creatubbles.CreateGetLoggedInUserRequest();

        Status = StatusGettingUserProfile;

        yield return creatubbles.SendRequest(userProfileRequest);

        if (logInRequest.IsCancelled)
        {
            LogInCancelled();
            yield break;
        }

        if (userProfileRequest.IsAnyError)
        {
            if (userProfileRequest.IsSystemError)
            {
                Debug.Log("System error: " + userProfileRequest.SystemError);
            }
            else if (userProfileRequest.IsHttpError && userProfileRequest.apiErrors != null && userProfileRequest.apiErrors.Length > 0)
            {
                // if unauthorized, log user out
                Debug.Log("API error: " + userProfileRequest.apiErrors[0].title);
            }

            LogInFailed();
            yield break;
        }

        if (userProfileRequest.Data == null || userProfileRequest.Data.data == null)
        {
            Debug.Log("Error: Invalid or missing data in response");
            LogInFailed();
            yield break;
        }

        Debug.Log("Success with data: " + userProfileRequest.Data.data.ToString());
        LogInSuccessful(userProfileRequest.Data.data.attributes.name);
    }

    private void LogInStarted()
    {
        Status = StatusLoggingIn;
        LogInInteractable = false;
        LogOutInteractable = false;
    }

    private void LogInSuccessful(string username)
    {
        Status = StatusLoggedInAs + username;
        LogInInteractable = false;
        LogOutInteractable = true;
    }

    private void LogInFailed()
    {
        Status = StatusLogInFailed;
        LogInInteractable = true;
        LogOutInteractable = false;
    }

    private void LogInCancelled()
    {
        Status = StatusLogInCancelled;
        LogInInteractable = true;
        LogOutInteractable = false;
    }
}
