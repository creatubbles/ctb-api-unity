[![](https://stateoftheart.creatubbles.com/wp-content/uploads/2015/01/ctb_home_logo.png)](https://www.creatubbles.com/)

## Creatubbles API Client Unity SDK
Creatubbles API Client Unity SDK is a simple library built to help you communicate with the latest [Creatubbles API](https://stateoftheart.creatubbles.com/api/).

Current SDK version: `0.1.3` (see [changelog](https://github.com/creatubbles/ctb-api-unity/blob/master/CHANGELOG.md))
Supported Unity versions: `5.4.x` and newer  
Supported output platforms: `iOS`, `macOS` (and possibly others, not yet confirmed)

Please note that library is still under development, and the SDK API might change in next versions.

## Documentation
SDK API reference (Doxygen generated) is available under `Docs/reference/html/` as `index.html`.

## Installation
Copy  `Assets/Scripts/Creatubbles` to you project's `Assets/Scripts` folder.

## QuickStart
1. Contact us at <support@creatubbles.com> to obtain application ID and secret.

2. Implement [IApiConfiguration](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/IApiConfiguration.cs) including your application ID and secret.

    See [CreatubblesConfiguration](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Demo/CreatubblesConfiguration.cs) for example implementation.

3. SDK project contains a demo scene `CreatubblesDemo`, which can be used to check how the SDK is working.

4. Implement [ISecureStorage](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/Storage/ISecureStorage.cs).

    See [InMemoryStorage](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/Storage/InMemoryStorage.cs) for example implementation.

    [InMemoryStorage](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/Storage/InMemoryStorage.cs) can be used for **development and testing**, however for **production** a persistent and more secure implementation is required.

    There are solutions available on the Unity Asset Store like [SPrefs](https://www.assetstore.unity3d.com/en/#!/content/56051) or [Secured PlayerPrefs](https://www.assetstore.unity3d.com/en/#!/content/32357), that could be considered for use as underlying implementation.

5. Configure [CreatubblesApiClient](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/CreatubblesApiClient) with instances of [IApiConfiguration](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/IApiConfiguration.cs) and [ISecureStorage](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/Storage/ISecureStorage.cs) from previous steps.

6. Use [CreatubblesApiClient](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Creatubbles/Api/CreatubblesApiClient) to create and send requests.

    Examples of using the API client can be found under `Assets/Scripts/Demo`:
    * [Logging in](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Demo/LogInDemo.cs)
    * [Getting landing URLs](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Demo/LandingUrlsDemo.cs)
    * [Creation upload](https://github.com/creatubbles/ctb-api-unity/blob/master/Assets/Scripts/Demo/UploadDemo.cs)

## Author
[Creatubbles](https://www.creatubbles.com/)

## Contact
In order to receive your AppId and AppSecret please contact us at <support@creatubbles.com>.

## License
CreatubblesAPIClient is available under the [MIT license](https://github.com/creatubbles/ctb-api-unity/blob/master/LICENSE.md).
