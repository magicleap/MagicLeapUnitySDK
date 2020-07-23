# Release Info

## Magic Leap Unity SDK Release 0.24.2

## Supported Unity Editor Release: 2020.1 (Tested against Magic Leap XR Plugin version 5.1.2 verified)

The files in this folder are meant to provide examples, best practices and wrappers to
help the user work and understand the MagicLeap APIs.

These files can change or even be removed from one release to another. If you're planning
on depending or modifying these assets for your own project, we recommend that you duplicate
the files, change the names and move them out of the Assets/MagicLeap folder. This will avoid
issues like your changes being deleted when you upgrade to a new unitypackage.

# Copyright

Copyright (c) 2020-present Magic Leap, Inc. All Rights Reserved.
Use of this file is governed by the Developer Agreement, located
here: https://id.magicleap.com/terms/developer

# Release Notes

## Bug Fixes
* Fixed issue where hand tracking was broken on second scene load.
* Fixed issue where touchpad processor not cleared out on scene unload.

## Known Issues
* CEA608 Subtitles are not working