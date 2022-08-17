// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAnchorNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
	using System;
	using System.Runtime.InteropServices;
	using Native;

	/// <summary>
	///	Spatial Anchor management is closely tied to the selected mapping mode on the device. The modes are
	///	mutually exclusive and affect the functionality of these APIs.For example, publishing an anchor is
	/// not available in "per-session" mapping mode.The available mapping modes are:
	/// Per-Session Mode - A non-persistent mode in which anchors are only available for the currently active tracking session.
	/// On-Device Mode - A persistent mode in which anchors are persisted locally and will be available
	/// in multiple sessions when localized to the same space in which they were published.
	/// </summary>
	public partial class MLAnchors
	{
		private MLResult.Code CreateQuery(Request.Params requestParams, out ulong queryHandle, out uint resultsCount) => NativeBindings.MLSpatialAnchorQueryFilter.Create(requestParams, out queryHandle, out resultsCount);

		private MLResult.Code GetQueryResult(Request.Params requestParams, ulong queryHandle, uint firstIndex, uint lastIndex, NativeBindings.MLSpatialAnchor[] anchors)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorQueryGetResult(this.Handle, queryHandle, firstIndex, lastIndex, anchors);
#else
			queryHandle = MagicLeapNativeBindings.InvalidHandle;
			return MLResult.Code.APIDLLNotFound;
#endif
		}

		private MLResult.Code DestroyQuery(ulong queryHandle)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorQueryDestroy(this.Handle, queryHandle);
#else
			return MLResult.Code.APIDLLNotFound;

#endif
		}

		private MLResult.Code GetLocalizationInformation(out MLAnchors.LocalizationInfo info)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			var resultCode = NativeBindings.MLSpatialAnchorGetLocalizationInfo(this.Handle, out NativeBindings.MLSpatialAnchorLocalizationInfo nativeInfo);
			info = new LocalizationInfo(nativeInfo);
			return resultCode;
#else
			info = default;
			return MLResult.Code.APIDLLNotFound;
#endif
		}

		private MLResult.Code CreateAnchor(NativeBindings.MLSpatialAnchorCreateInfo createInfo, out NativeBindings.MLSpatialAnchor anchor)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorCreate(this.Handle, in createInfo, out anchor);
#else
			anchor = default;
			return MLResult.Code.APIDLLNotFound;

#endif
		}

		private MLResult.Code PublishAnchor(Anchor anchor)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorPublish(this.Handle, anchor.id);
#else
			return MLResult.Code.APIDLLNotFound;

#endif
		}

		private MLResult.Code UpdateAnchor(Anchor anchor, long expirationTimeStamp)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			var unixTimestamp = (DateTime.UtcNow.AddSeconds(expirationTimeStamp)).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			var nativeAnchor = new NativeBindings.MLSpatialAnchor(anchor, (ulong)unixTimestamp);
			return NativeBindings.MLSpatialAnchorUpdate(this.Handle, in nativeAnchor);
#else
			return MLResult.Code.APIDLLNotFound;

#endif
		}

		private MLResult.Code DeleteAnchor(Anchor anchor)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorDelete(this.Handle, anchor.id);
#else
			return MLResult.Code.APIDLLNotFound;

#endif
		}
		private MLResult.Code DeleteAnchor(string anchorId)
		{
#if UNITY_MAGICLEAP || UNITY_ANDROID
			return NativeBindings.MLSpatialAnchorDelete(this.Handle, new MagicLeapNativeBindings.MLUUIDBytes(anchorId));
#else
			return MLResult.Code.APIDLLNotFound;

#endif
		}

	}
}
