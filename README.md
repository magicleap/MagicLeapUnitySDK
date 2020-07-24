# Release Info

## Magic Leap Unity SDK Release 0.24.2 - Experimental

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

The following is an unofficial release of the Magic Leap Unity SDK. It contains experimental functionalty
intended to provide new performance fixes and potential features to be included in the SDK at a future date.

Please do provide all feedback through the standard Developer portals.

If you would like the officially supported 0.24.2, please grab the appropriate release tagged github bits, or
download from the Magic Leap Package Manager.

## What you can find here

# New API

PCFs have brought in the new perception root API. This has been available in the C API and is now exposed in
the Unity SDK. Please see the C API documentation for intended usage.

# Performance Enhancements

MLPlanes, MLRaycast and PCFs are now run multi-threaded rather than on the main thread's update callback. This
change has required some changes to made in how they report back results. The thread scheduler should make
all callbacks to the user on the main thread, while work is scheduled to be performed on the worker thread.

There is only 1 Magic Leap Worker Thread.

# Potential Future Updates

The Spatial Mapper in the Unity XR Package for Magic Leap has a number of issues that have been reported since
it was first introduced. However, the spatial mapper straddles the boundary of ownership between Magic Leap
and Unity. As such, it hasn't received as much attention as it has deserved in terms of bug fixing or
performance enhancements.

Magic Leap is providing MLMeshing as an alternative and potential replacement to the MLSpatialMapper. Based on
user feedback concerning both usability and performance, Unity and Magic Leap will determine the best direction
for users and a proper course for code updates.
