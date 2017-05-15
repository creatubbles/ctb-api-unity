[![](https://stateoftheart.creatubbles.com/wp-content/uploads/2015/01/ctb_home_logo.png)](https://www.creatubbles.com/)

## Creatubbles API Client Unity 4 SDK
Creatubbles API Client Unity 4 SDK is a simple library built to help you communicate with the latest [Creatubbles API](https://stateoftheart.creatubbles.com/api/).

Current SDK version: `0.1.3` (see [changelog](https://github.com/creatubbles/ctb-api-unity/blob/master/CHANGELOG.md))
Supported Unity versions: `4.2.0` and newer
Supported output platforms: `Android`, `iOS`, `macOS`, `Windows desktop` (and possibly others, not yet confirmed)

*NOTE* For Unity 5 projects, please consider using https://github.com/creatubbles/ctb-api-unity/ instead for following benefits:
- cancellable requests
- detailed error reporting
- improved performance

*NOTE2* This library is still under development, and the SDK API might change in the future versions.

## Installation
Copy  `Assets/Scripts/Creatubbles` to you project's `Assets/Scripts` folder.

*IMPORTANT!* This SDK uses `SimpleJSON` library available at http://wiki.unity3d.com/index.php/SimpleJSON under MIT license. Please drop the `SimpleJSON.cs` file under your `Assets/Plugins` directory.

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

## Author
[Creatubbles](https://www.creatubbles.com/)

## Contact
In order to receive your AppId and AppSecret please contact us at <support@creatubbles.com>.

## License
CreatubblesAPIClient is available under the [MIT license](LICENSE.md).
