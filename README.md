[![](https://stateoftheart.creatubbles.com/wp-content/uploads/2015/01/ctb_home_logo.png)](https://www.creatubbles.com/)

## Creatubbles API Client Unity SDK
Creatubbles API Client Unity SDK is a simple library built to help you communicate with the latest [Creatubbles API](https://stateoftheart.creatubbles.com/api/).

Current SDK version: `0.1.4` (see [changelog](CHANGELOG.md))
Supported Unity versions: `5.4.x` and newer  
Supported output platforms: `iOS`, `macOS` (and possibly others, not yet confirmed)

**NOTE!** Library is still under development, and the SDK API might change in the future versions.

## Documentation
SDK API reference (Doxygen generated) is available under `Docs/reference/html/` as `index.html`.

## Installation
Copy  `Assets/Scripts/Creatubbles` to you project's `Assets/Scripts` folder.

## QuickStart
1. Contact us at <support@creatubbles.com> to obtain application ID and secret.

2. Implement [IApiConfiguration](Assets/Scripts/Creatubbles/Api/IApiConfiguration.cs) including your application ID and secret.

    See [CreatubblesConfiguration](Assets/Scripts/Demo/CreatubblesConfiguration.cs) for example implementation.

3. SDK project contains a demo scene `CreatubblesDemo`, which can be used to check how the SDK is working.

4. Implement [ISecureStorage](Assets/Scripts/Creatubbles/Api/Storage/ISecureStorage.cs).

    See [InMemoryStorage](Assets/Scripts/Creatubbles/Api/Storage/InMemoryStorage.cs) for example implementation.

    [InMemoryStorage](Assets/Scripts/Creatubbles/Api/Storage/InMemoryStorage.cs) can be used for **development and testing**, however for **production** a persistent and more secure implementation is required.

    There are solutions available on the Unity Asset Store like [SPrefs](https://www.assetstore.unity3d.com/en/#!/content/56051) or [Secured PlayerPrefs](https://www.assetstore.unity3d.com/en/#!/content/32357), that could be considered for use as underlying implementation.

5. Configure [CreatubblesApiClient](Assets/Scripts/Creatubbles/Api/CreatubblesApiClient.cs) with instances of [IApiConfiguration](Assets/Scripts/Creatubbles/Api/IApiConfiguration.cs) and [ISecureStorage](Assets/Scripts/Creatubbles/Api/Storage/ISecureStorage.cs) from previous steps.

6. Use [CreatubblesApiClient](Assets/Scripts/Creatubbles/Api/CreatubblesApiClient.cs) to create and send requests.

    Examples of using the API client can be found under `Assets/Scripts/Demo`:
    * [Logging in](Assets/Scripts/Demo/LogInDemo.cs)
    * [Getting landing URLs](Assets/Scripts/Demo/LandingUrlsDemo.cs)
    * [Creation upload](Assets/Scripts/Demo/UploadDemo.cs)

## Author
[Creatubbles](https://www.creatubbles.com/)

## Contact
In order to receive your AppId and AppSecret please contact us at <support@creatubbles.com>.

## License
CreatubblesAPIClient is available under the [MIT license](LICENSE.md).
