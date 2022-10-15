# Magic Leap Unity SDK
This is the official Magic Leap Unity SDK, which exposes the full Magic Leap device capabilities within the Unity engine. 

Scoped Registry URL: `http://registry.npmjs.org`

Scope: com.magicleap

This SDK is provided in the form of an npm package, just like Unity's official add-ons. Therefore, integration into your Unity project is as easy as adding our scoped registry to your project settings and using the Unity Package Manager to install. After integration, upgrades and dependencies can also be managed via the Unity Package Manager. For more detailed instructions, please visit [this page on the Magic Leap Developer Portal](https://developer.magicleap.com/en-us/learn/guides/unity-setup-intro).

For a full list of features and documentation on how to use the Magic Leap Unity SDK, see [this page on the Magic Leap Developer Portal](https://developer.magicleap.com/en-us/learn/guides/unity-overview).

Examples and a project template for quick setup can be found at our [Example Project page](https://github.com/magicleap/MagicLeapUnityExamples).

Note : If this package depends on unreleased versions of the Magic Leap XR Plugin (com.unity.xr.magicleap), that plugin will need to be added to the project manually.

Note : Preview builds will not be uploaded on npm. The package will need to either be embedded in the project, or linked locally using the download path.

## SDK Refactor
The Magic Leap Unity SDK is undergoing drastic changes to better support and align with existing Unity frameworks like AR Foundation, XR Interaction Toolkit and the new Input System. This change also aims to reduce the duplication of APIs in our SDK which are already covered by these cross-platform frameworks.
This means APIs like `MLEyes`, `MLHandTracking`, `MLInput`, `MLImageTracker` etc. will either go away completely or be reduced to a basic "extension" form i.e. only functionality which is specific to Magic Leap and not available in the Unity frameworks will be exposed in our SDK
package. Such extension classes will be provided under the namespace `UnityEngine.XR.MagicLeap.Extensions`. E.g. Magic Leap exposes APIs to configure which hand gestures to enable detection for. This ability does not exist in any of Unity's frameworks. Therefore, we will expose this via
the `UnityEngine.XR.MagicLeap.Extensions.MLHandTracking` class. This major refactor of the SDK is currently in progress and we hope to finish it by the end of
December 2021, complete with migration guides and examples. Please note that initial internal releases of this refactor WILL CONTAIN breaking changes when compared to the 0.26.0 ML1 release. Patching up the breaking changes with proper deprecations of the removed data types and methods to make the code transition easier for developers will be addressed before the SDK becomes production ready.
