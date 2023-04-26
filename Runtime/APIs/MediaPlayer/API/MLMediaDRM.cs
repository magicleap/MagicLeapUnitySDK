// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using MagicLeap.Native;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// Media player script that allows playback of a streaming video (either from file or web URL)
        /// This script will update the main texture parameter of the Renderer attached as a sibling
        /// with the video frame from playback. Audio is also handled through this class and will
        /// playback audio from the file.
        /// </summary>
        public partial class Player
        {
            /// <summary>
            /// Track from the prepared source that can be selected by the media player.
            /// </summary>
            public partial class Track
            {
                /// <summary>
                /// MediaDRM to obtain the content keys for decrypting protected media streams.
                /// </summary>                
                public partial class DRM
                {
                    /// <summary>
                    /// Delegate for the any OnEvent event.
                    /// </summary>
                    [Obsolete("OnEventDelegate has been deprecated and will be removed in a future release.")]
                    public delegate void OnEventDelegate(DRM drm, Event eventInfo);

                    /// <summary>
                    /// Delegate for the any OnExpirationUpdate event.
                    /// </summary>
                    [Obsolete("OnExpirationUpdateDelegate has been deprecated and will be removed in a future release.")]
                    public delegate void OnExpirationUpdateDelegate(DRM drm, Expiration expirationInfo);

                    /// <summary>
                    /// Delegate for the any OnKeyStatusChange event.
                    /// </summary>
                    [Obsolete("OnKeyStatusChangeDelegate has been deprecated and will be removed in a future release.")]
                    public delegate void OnKeyStatusChangeDelegate(DRM drm, KeyStatuses KeyStatuses);

                    /// <summary>
                    /// Event used to listen for different drm events.
                    /// </summary>
                    [Obsolete("OnEvent has been deprecated and will be removed in a future release.")]
                    public event OnEventDelegate OnEvent = delegate { };

                    /// <summary>
                    /// Event used to listen for when there is an update in expiration.
                    /// </summary>
                    [Obsolete("OnExpirationUpdateDelegate has been deprecated and will be removed in a future release.")]
                    public event OnExpirationUpdateDelegate OnExpirationUpdate = delegate { };

                    /// <summary>
                    /// Event used to listen for when a key's status has changed.
                    /// </summary>
                    [Obsolete("OnKeyStatusChangeDelegate has been deprecated and will be removed in a future release.")]
                    public event OnKeyStatusChangeDelegate OnKeyStatusChange = delegate { };

                    public enum Property
                    {
                        Vendor,
                        Version,
                        Description,
                        Algorithms,
                        DeviceUniqueId
                    }

                    /// <summary>
                    /// Type of DRM key.
                    /// </summary>
                    public enum KeyType : int
                    {
                        /// <summary>
                        /// This key request type specifies that the keys will be for online use, they will.
                        /// not be saved to the device for subsequent use when the device is not connected to a network.
                        /// </summary>
                        Streaming = 1,

                        /// <summary>
                        /// This key request type specifies that the keys will be for offline use, they
                        /// will be saved to the device for use when the device is not connected to a network.
                        /// </summary>
                        Offline,

                        /// <summary>
                        /// This key request type specifies that previously saved offline keys should be released.
                        /// </summary>
                        Release,
                    }

                    public enum KeyStatus
                    {
                        Usable = 0,
                        Expired,
                        OutputNotAllowed,
                        Pending,
                        InternalError
                    }

                    /// <summary>
                    /// The type of DRM event.
                    /// </summary>
                    public enum EventType
                    {
                        /// <summary>
                        /// This event type indicates that the app needs to request
                        /// a certificate from the provisioning server.
                        /// The request message data is obtained using MLMediaDRMGetProvisionRequest().
                        /// </summary>
                        ProvisionRequired = 1,

                        /// <summary>
                        /// This event type indicates that the app needs to request keys from a license server.
                        /// The request message data is obtained using MLMediaDRMGetKeyRequest().
                        /// </summary>
                        KeyRequired,

                        /// <summary>
                        /// This event type indicates that the licensed usage duration for keys in a session has expired.
                        /// The keys are no longer valid.
                        /// </summary>
                        KeyExpired,

                        /// <summary>
                        /// This event may indicate some specific vendor-defined condition,
                        /// see your DRM provider documentation for details.
                        /// </summary>
                        VendorDefined,

                        /// <summary>
                        /// This event indicates that a session opened by the app has been
                        /// reclaimed by the resource manager.
                        /// </summary>
                        SessionReclaimed,

                        /// <summary>
                        /// This event is issued when a session expiration update occurs,
                        /// to inform the app about the change in expiration time.
                        /// </summary>
                        ExpirationUpdate,

                        /// <summary>
                        /// This event is issued when the keys in a session change status,
                        /// such as when the license is renewed or expires.
                        /// </summary>
                        KeysChange,
                    };

                    /// <summary>
                    /// Type of track this DRM is used for.
                    /// </summary>
                    [Obsolete("TrackType has been deprecated and will be removed in a future release.")]
                    public Track.Type TrackType { get; private set; }

                    /// <summary>
                    /// Handle for the DRM in native code.
                    /// </summary>
                    private ulong handle = MagicLeapNativeBindings.InvalidHandle;

                    /// <summary>
                    /// Handle for the managed media player object to pass to and from unmanaged code.
                    /// </summary>
                    private GCHandle gcHandle;

                    /// <summary>
                    /// The media player this drm is associated with.
                    /// </summary>
                    private MLMedia.Player mediaPlayer;

                    /// <summary>
                    /// The selected UUID for the drm.
                    /// </summary>
                    private MagicLeapNativeBindings.MLUUID scheme;

                    /// <summary>
                    /// Hides the default initilization of the <see cref="DRM" /> class.
                    /// </summary>
                    private DRM() { }

                    /// <summary>
                    /// Provides initilization of the <see cref="DRM" /> class with the two given parameters.
                    /// </summary>
                    /// <param name="mlPlayer">The media player this DRM is associated with.</param>
                    /// <param name="trackInfo">The info from the media player track that this DRM is associated with.</param>
                    [Obsolete("DRM has been deprecated and will be removed in a future release.")]
                    public DRM(MLMedia.Player mlPlayer, Info trackDRMInfo)
                    {
                        this.mediaPlayer = mlPlayer;
                        this.TrackType = trackDRMInfo.TrackType;
                        this.scheme = trackDRMInfo.UUIDS[0];
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMCreate(this.scheme, out this.handle);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMCreate));
                        NativeBindings.MLMediaDRMEventCallbacks callbacks = NativeBindings.MLMediaDRMEventCallbacks.Create();
                        this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                        System.IntPtr ptr = GCHandle.ToIntPtr(this.gcHandle);
                        resultCode = NativeBindings.MLMediaDRMSetOnEventListenerEx(this.handle, ref callbacks, ptr);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMSetOnEventListenerEx));
                    }

                    /// <summary>
                    /// Creates a new session for the DRM with the provided track information.
                    /// </summary>
                    /// <param name="trackInfo">The provided track information to create the new session with.</param>
                    [Obsolete("CreateSession has been deprecated and will be removed in a future release.")]
                    public Session CreateSession(Info trackInfo) => new Session(this, trackInfo.PSSHEntries[0]);

                    /// <summary>
                    /// Finalizer for releasing native resources.
                    /// </summary>
                    ~DRM()
                    {
                        if (MagicLeapNativeBindings.MLHandleIsValid(this.handle))
                        {
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMRelease(this.handle);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMRelease));
                        }
                    }

                    /// <summary>
                    /// A provision request/response exchange occurs between the app and a provisioning server to retrieve a device certificate.
                    /// If provisioning is required, the #EVENT_PROVISION_REQUIRED event will be sent to the event handler.
                    /// MLMediaDRMGetProvisionRequest() is used to obtain the opaque provision request byte array that should be delivered to the provisioning
                    /// server.
                    /// </summary>
                    [Obsolete("ProvisionRequest has been deprecated and will be removed in a future release.")]
                    public MLResult ProvisionRequest(out byte[] requestData, out string defaultURL)
                    {
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMGetProvisionRequest(this.handle, "none", out NativeBindings.MLMediaDRMRequestMessage provisionRequest);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetProvisionRequest));

                        // Send the provision request message to the provision server and get the response
                        requestData = new byte[provisionRequest.Request.Length];
                        Marshal.Copy(provisionRequest.Request.Data, requestData, 0, requestData.Length);
                        defaultURL = provisionRequest.DefaultURL;

                        NativeBindings.MLMediaDRMRequestMessageRelease(ref provisionRequest);
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// After a provision response is received by the app, it is provided to the DRM engine plugin using this method.
                    /// </summary>
                    [Obsolete("ProvisionResponse has been deprecated and will be removed in a future release.")]
                    public MLResult ProvisionResponse(byte[] responseData)
                    {
                        NativeBindings.MLMediaDRMByteArray responseDRMByteArray = new NativeBindings.MLMediaDRMByteArray();
                        responseDRMByteArray.AllocResources(responseData);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMProvideProvisionResponse(this.handle, ref responseDRMByteArray, out NativeBindings.MLMediaDRMByteArray certificate, out NativeBindings.MLMediaDRMByteArray wrappedKey);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMProvideProvisionResponse));
                        responseDRMByteArray.FreeResources();

                        NativeBindings.MLMediaDRMByteArrayRelease(ref wrappedKey);
                        NativeBindings.MLMediaDRMByteArrayRelease(ref certificate);

                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Access all secure stops.  Secure Stop: A means of enforcing limits on the number of concurrent streams per subscriber
                    /// across devices is provided via #SecureStop.  This is achieved by securely monitoring the lifetime of sessions.
                    /// Information from the server related to the current playback session is written to persistent storage on the device when each
                    /// #MediaCrypto object is created.  In the normal case, playback will be completed, the session destroyed and the Secure Stops
                    /// will be queried.  The app queries secure stops and forwards the secure stop message to the server which verifies the
                    /// signature and notifies the server side database that the session destruction has been confirmed.  The persisted record on
                    /// the client is only removed after positive confirmation that the server received the message using releaseSecureStops().
                    /// </summary>
                    [Obsolete("GetSecureStops has been deprecated and will be removed in a future release.")]
                    public MLResult GetSecureStops(out byte[][] secureStops)
                    {
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMGetSecureStops(this.handle, out NativeBindings.MLMediaDRMByteArrayList secureStopsNative);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetSecureStops));
                        secureStops = secureStopsNative.ByteArrays;
                        NativeBindings.MLMediaDRMByteArrayListRelease(ref secureStopsNative);
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    ///  Access secure stop by secure stop ID. \see MLMediaDRMGetSecureStops()
                    /// </summary>
                    [Obsolete("GetSecureStop has been deprecated and will be removed in a future release.")]
                    public MLResult GetSecureStop(byte[] secureStopId, out byte[] secureStop)
                    {
                        NativeBindings.MLMediaDRMByteArray secureStopIdNative = new NativeBindings.MLMediaDRMByteArray();
                        secureStopIdNative.AllocResources(secureStopId);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMGetSecureStop(this.handle, ref secureStopIdNative, out NativeBindings.MLMediaDRMByteArray secureStopNative);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetSecureStop));
                        secureStop = secureStopNative.Bytes;
                        secureStopIdNative.FreeResources();
                        NativeBindings.MLMediaDRMByteArrayRelease(ref secureStopNative);
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Remove all the SecureStops.
                    /// </summary>
                    [Obsolete("ReleaseAllSecureStops has been deprecated and will be removed in a future release.")]
                    public MLResult ReleaseAllSecureStops()
                    {
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMReleaseAllSecureStops(this.handle);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMReleaseAllSecureStops));
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Process the SecureStop server response message. After authenticating the message, remove the SecureStops identified in the response.
                    /// </summary>
                    [Obsolete("ReleaseSecureStops has been deprecated and will be removed in a future release.")]
                    public MLResult ReleaseSecureStops(byte[] secureStops)
                    {
                        NativeBindings.MLMediaDRMByteArray secureStopNative = new NativeBindings.MLMediaDRMByteArray();
                        secureStopNative.AllocResources(secureStops);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMReleaseSecureStops(this.handle, ref secureStopNative);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMReleaseSecureStops));
                        secureStopNative.FreeResources();
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Query if the given scheme identified by its UUID is supported on this device. And whether the drm plugin is able to
                    /// handle the media container format.
                    /// </summary>
                    [Obsolete("IsCryptoSchemeSupported has been deprecated and will be removed in a future release.")]
                    public MLResult IsCryptoSchemeSupported(MagicLeapNativeBindings.MLUUID uuid, string mime, out bool isSupported)
                    {
                        byte[] mimeBytes = System.Text.Encoding.ASCII.GetBytes(mime);
                        System.IntPtr mimePtr = Marshal.AllocHGlobal(mimeBytes.Length);
                        Marshal.Copy(mimeBytes, 0, mimePtr, mimeBytes.Length);

                        MLResult.Code resultCode = NativeBindings.MLMediaDRMIsCryptoSchemeSupported(uuid, mimePtr, out isSupported);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMIsCryptoSchemeSupported));
                        Marshal.FreeHGlobal(mimePtr);

                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Read a DRM engine plugin String property value, given the property name.
                    /// </summary>
                    [Obsolete("GetPropertyString has been deprecated and will be removed in a future release.")]
                    public MLResult GetPropertyString(Property property, out string propertyValue)
                    {
                        NativeBindings.Properties.TryGetValue(property, out string propertyName);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMGetPropertyString(this.handle, propertyName, out propertyValue);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetPropertyString));
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Set a DRM engine plugin String property value.
                    /// </summary>
                    [Obsolete("SetPropertyString has been deprecated and will be removed in a future release.")]
                    public MLResult SetPropertyString(Property property, string propertyValue)
                    {
                        NativeBindings.Properties.TryGetValue(property, out string propertyName);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMSetPropertyString(this.handle, propertyName, propertyValue);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMSetPropertyString));
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Read a DRM engine plugin byte array property value, given the property name.
                    /// </summary>
                    [Obsolete("GetPropertyByteArray has been deprecated and will be removed in a future release.")]
                    public MLResult GetPropertyByteArray(Property property, out byte[] propertyValue)
                    {
                        NativeBindings.Properties.TryGetValue(property, out string propertyName);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMGetPropertyByteArray(this.handle, propertyName, out NativeBindings.MLMediaDRMByteArray propertyValueByteArray);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetPropertyByteArray));
                        propertyValue = propertyValueByteArray.Bytes;
                        NativeBindings.MLMediaDRMByteArrayRelease(ref propertyValueByteArray);
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// ***CURRENTLY UNTESTED ON CAPI***
                    /// Set a DRM engine plugin byte array property value.
                    /// </summary>
                    [Obsolete("SetPropertyByteArray has been deprecated and will be removed in a future release.")]
                    public MLResult SetPropertyByteArray(Property property, byte[] propertyValue)
                    {
                        NativeBindings.Properties.TryGetValue(property, out string propertyName);
                        NativeBindings.MLMediaDRMByteArray propertyValuByteArray = new NativeBindings.MLMediaDRMByteArray();
                        propertyValuByteArray.AllocResources(propertyValue);
                        MLResult.Code resultCode = NativeBindings.MLMediaDRMSetPropertyByteArray(this.handle, propertyName, ref propertyValuByteArray);
                        MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMSetPropertyByteArray));
                        propertyValuByteArray.FreeResources();
                        return MLResult.Create(resultCode);
                    }

                    /// <summary>
                    /// A DRM session that's required for a media player track to be prepared.
                    /// </summary>
                    [Obsolete("Session has been deprecated and will be removed in a future release.")]
                    public class Session
                    {
                        [Obsolete("CryptoKeyParam has been deprecated and will be removed in a future release.")]
                        public readonly struct CryptoKeyParam
                        {
                            [Obsolete("CryptoKeyParam has been deprecated and will be removed in a future release.")]
                            public CryptoKeyParam(string algorithm, byte[] keySetId, byte[] initialVector)
                            {
                                this.Algorithm = algorithm;
                                this.KeySetId = keySetId;
                                this.InitialVector = initialVector;
                            }

                            [Obsolete("Algorithm has been deprecated and will be removed in a future release.")]
                            public readonly string Algorithm;

                            [Obsolete("KeySetId has been deprecated and will be removed in a future release.")]
                            public readonly byte[] KeySetId;

                            [Obsolete("InitialVector has been deprecated and will be removed in a future release.")]
                            public readonly byte[] InitialVector;
                        }

                        [Obsolete("HMACParam has been deprecated and will be removed in a future release.")]
                        public readonly struct HMACParam
                        {
                            public HMACParam(string algorithm, byte[] keySetId)
                            {
                                this.Algorithm = algorithm;
                                this.KeySetId = keySetId;
                            }

                            public readonly string Algorithm;
                            public readonly byte[] KeySetId;
                        }

                        [Obsolete("RSAParam has been deprecated and will be removed in a future release.")]
                        public readonly struct RSAParam
                        {
                            [Obsolete("RSAParam has been deprecated and will be removed in a future release.")]
                            public RSAParam(string algorithm, byte[] wrappedRsaKey)
                            {
                                this.Algorithm = algorithm;
                                this.WrappedRsaKey = wrappedRsaKey;
                            }

                            [Obsolete("Algorithm has been deprecated and will be removed in a future release.")]
                            public readonly string Algorithm;

                            [Obsolete("WrappedRsaKey has been deprecated and will be removed in a future release.")]
                            public readonly byte[] WrappedRsaKey;
                        }

                        /// <summary>
                        /// Provides initilization of the <see cref="Session" /> class with the two given parameters.
                        /// </summary>
                        /// <param name="drmHandle">The handle of the drm this session is associated with.</param>
                        /// <param name="psshKey">The psshKey of the track this session is associated with.</param>
                        internal Session(DRM drm, Player.NativeBindings.MLMediaPlayerPSSHEntry psshKey)
                        {
                            this.Drm = drm;
                            this.psshKey = psshKey;
                        }

                        /// <summary>
                        /// Finalizer for releasing native resources.
                        /// </summary>
                        ~Session()
                        {
                            if (MagicLeapNativeBindings.MLHandleIsValid(this.Drm.handle))
                                this.Close();
                        }

                        /// <summary>
                        /// The drm this session is associated with.
                        /// </summary>
                        [Obsolete("Drm has been deprecated and will be removed in a future release.")]
                        public DRM Drm { get; private set; }

                        /// <summary>
                        /// Determines if the session is prepared or not.
                        /// </summary>
                        [Obsolete("IsPrepared has been deprecated and will be removed in a future release.")]
                        public bool IsPrepared { get; private set; }

                        /// <summary>
                        /// The id of the session.
                        /// </summary>
                        internal NativeBindings.MLMediaDRMByteArray id;

                        /// <summary>
                        /// The keySetId of the session.
                        /// </summary>
                        internal NativeBindings.MLMediaDRMByteArray keySetId;

                        /// <summary>
                        /// The psshKey of the track this session is associated with.
                        /// </summary>
                        private Player.NativeBindings.MLMediaPlayerPSSHEntry psshKey;

                        /// <summary>
                        /// Opens a new session.
                        /// </summary>
                        [Obsolete("Open has been deprecated and will be removed in a future release.")]
                        public MLResult Open()
                        {
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMOpenSession(this.Drm.handle, out this.id);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMOpenSession));
                            byte[] sessionData = new byte[this.id.Length];
                            Marshal.Copy(this.id.Data, sessionData, 0, (int)this.id.Length);
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// Prepare DRM for the selected media (either audio or video) track with the CurrentSession member.
                        /// </summary>
                        [Obsolete("Prepare has been deprecated and will be removed in a future release.")]
                        public MLResult Prepare()
                        {
                            MLMedia.Player.NativeBindings.MLMediaPlayerTrackDRMSessionInfo sessionInfo = new Player.NativeBindings.MLMediaPlayerTrackDRMSessionInfo(this.Drm.TrackType);
                            sessionInfo.AllocResources(this.Drm.scheme, this.id);
                            MLResult.Code resultCode = MLMedia.Player.NativeBindings.MLMediaPlayerPrepareTrackDRM(this.Drm.mediaPlayer.handle, ref sessionInfo);
                            this.IsPrepared = MLResult.DidNativeCallSucceed(resultCode, nameof(MLMedia.Player.NativeBindings.MLMediaPlayerPrepareTrackDRM));
                            sessionInfo.FreeResources();

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// Closes a session that was previously opened.
                        /// </summary>
                        [Obsolete("Close has been deprecated and will be removed in a future release.")]
                        public MLResult Close()
                        {
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMByteArrayRelease(ref this.keySetId);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMByteArrayRelease));

                            resultCode = NativeBindings.MLMediaDRMCloseSession(this.Drm.handle, ref this.id);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMCloseSession));

                            this.IsPrepared = false;
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// A key request/response exchange occurs between the app and a license server to obtain or release keys used to decrypt
                        /// encrypted content. MLMediaDRMGetKeyRequest() is used to obtain an opaque key request byte array that is delivered to the
                        /// license server.  The opaque key request byte array is returned in out_key_request.  request The recommended URL to
                        /// deliver the key request to is returned in out_key_request.  default_URL.  After the app has received the key request response
                        /// from the server, it should deliver to the response to the DRM engine plugin using the method
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("KeysRequest has been deprecated and will be removed in a future release.")]
                        public MLResult KeysRequest(out byte[] requestData)
                        {
                            NativeBindings.MLMediaDRMKeyRequestInputParam keyRequestParam = new NativeBindings.MLMediaDRMKeyRequestInputParam("cenc", KeyType.Streaming);
                            keyRequestParam.AllocResources(this.psshKey);
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMGetKeyRequest(this.Drm.handle, ref this.id, ref keyRequestParam, out NativeBindings.MLMediaDRMRequestMessage keyRequest);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMGetKeyRequest));
                            keyRequestParam.FreeResources();
                            requestData = new byte[keyRequest.Request.Length];
                            Marshal.Copy(keyRequest.Request.Data, requestData, 0, requestData.Length);
                            NativeBindings.MLMediaDRMRequestMessageRelease(ref keyRequest);
                            return MLResult.Create(resultCode);
                        }


                        /// <summary>
                        /// A key response is received from the license server by the app, then it is provided to the DRM engine plugin using
                        /// MLMediaDRMProvideKeyResponse().  When the response is for an offline key request, a key_set_id is returned that can be used to
                        /// later restore the keys to a new session with restoreKeys().  When the response is for a streaming or release request, a
                        /// null key_set_id is returned.
                        /// </summary>
                        [Obsolete("KeysResponse has been deprecated and will be removed in a future release.")]
                        public MLResult KeysResponse(byte[] responseData, out byte[] keySetData)
                        {
                            NativeBindings.MLMediaDRMByteArray responseDRMByteArray = new NativeBindings.MLMediaDRMByteArray();
                            responseDRMByteArray.AllocResources(responseData);
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMProvideKeyResponse(this.Drm.handle, ref this.id, ref responseDRMByteArray, out this.keySetId);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMProvideKeyResponse));
                            responseDRMByteArray.FreeResources();
                            keySetData = new byte[keySetId.Length];
                            Marshal.Copy(keySetId.Data, keySetData, 0, (int)keySetId.Length);
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Encrypt the data referenced by input using algorithm if specified, and write the encrypted result into output.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("Encrypt has been deprecated and will be removed in a future release.")]
                        public MLResult Encrypt(CryptoKeyParam cryptoKeyParam, byte[] input, out byte[] output)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(cryptoKeyParam.KeySetId);

                            NativeBindings.MLMediaDRMByteArray initialVectorByteArray = new NativeBindings.MLMediaDRMByteArray();
                            initialVectorByteArray.AllocResources(cryptoKeyParam.InitialVector);

                            NativeBindings.MLMediaDRMCryptoInputParam cryptoKeyParamNative = new NativeBindings.MLMediaDRMCryptoInputParam();
                            cryptoKeyParamNative.AllocResources(cryptoKeyParam.Algorithm, keySetIdByteArray, initialVectorByteArray);

                            NativeBindings.MLMediaDRMByteArray inputByteArray = new NativeBindings.MLMediaDRMByteArray();
                            inputByteArray.AllocResources(input);

                            MLResult.Code resultCode = NativeBindings.MLMediaDRMEncrypt(this.Drm.handle, ref this.id, ref cryptoKeyParamNative, ref inputByteArray, out NativeBindings.MLMediaDRMByteArray outputByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMEncrypt));
                            output = outputByteArray.Bytes;

                            keySetIdByteArray.FreeResources();
                            initialVectorByteArray.FreeResources();
                            cryptoKeyParamNative.FreeResources();
                            inputByteArray.FreeResources();
                            NativeBindings.MLMediaDRMByteArrayRelease(ref outputByteArray);

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Decrypt the data referenced by input using algorithm if specified, and write the encrypted result into output.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("Decrypt has been deprecated and will be removed in a future release.")]
                        public MLResult Decrypt(CryptoKeyParam cryptoKeyParam, byte[] input, out byte[] output)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(cryptoKeyParam.KeySetId);

                            NativeBindings.MLMediaDRMByteArray initialVectorByteArray = new NativeBindings.MLMediaDRMByteArray();
                            initialVectorByteArray.AllocResources(cryptoKeyParam.InitialVector);

                            NativeBindings.MLMediaDRMCryptoInputParam cryptoKeyParamNative = new NativeBindings.MLMediaDRMCryptoInputParam();
                            cryptoKeyParamNative.AllocResources(cryptoKeyParam.Algorithm, keySetIdByteArray, initialVectorByteArray);

                            NativeBindings.MLMediaDRMByteArray inputByteArray = new NativeBindings.MLMediaDRMByteArray();
                            inputByteArray.AllocResources(input);

                            MLResult.Code resultCode = NativeBindings.MLMediaDRMDecrypt(this.Drm.handle, ref this.id, ref cryptoKeyParamNative, ref inputByteArray, out NativeBindings.MLMediaDRMByteArray outputByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMDecrypt));
                            output = outputByteArray.Bytes;

                            keySetIdByteArray.FreeResources();
                            initialVectorByteArray.FreeResources();
                            cryptoKeyParamNative.FreeResources();
                            inputByteArray.FreeResources();
                            NativeBindings.MLMediaDRMByteArrayRelease(ref outputByteArray);

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Generate a signature using the specified algorithm (if provided) over the message data and store the signature.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("Sign has been deprecated and will be removed in a future release.")]
                        public MLResult Sign(HMACParam hmacParam, byte[] message, out byte[] signature)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(hmacParam.KeySetId);

                            NativeBindings.MLMediaDRMHMACInputParam hmacParamNative = new NativeBindings.MLMediaDRMHMACInputParam();
                            hmacParamNative.AllocResources(hmacParam.Algorithm, keySetIdByteArray);

                            NativeBindings.MLMediaDRMByteArray messageByteArray = new NativeBindings.MLMediaDRMByteArray();
                            messageByteArray.AllocResources(message);

                            MLResult.Code resultCode = NativeBindings.MLMediaDRMSign(this.Drm.handle, ref this.id, ref hmacParamNative, ref messageByteArray, out NativeBindings.MLMediaDRMByteArray signatureByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMSign));
                            signature = signatureByteArray.Bytes;

                            keySetIdByteArray.FreeResources();
                            hmacParamNative.FreeResources();
                            messageByteArray.FreeResources();
                            NativeBindings.MLMediaDRMByteArrayRelease(ref signatureByteArray);

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Generate a signature using the specified algorithm (if provided) over the message data and store the signature.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("Sign has been deprecated and will be removed in a future release.")]
                        public MLResult Sign(RSAParam rsaParam, byte[] message, out byte[] signature)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(rsaParam.WrappedRsaKey);

                            NativeBindings.MLMediaDRMRSAInputParam rsaParamNative = new NativeBindings.MLMediaDRMRSAInputParam();
                            rsaParamNative.AllocResources(rsaParam.Algorithm, keySetIdByteArray);

                            NativeBindings.MLMediaDRMByteArray messageByteArray = new NativeBindings.MLMediaDRMByteArray();
                            messageByteArray.AllocResources(message);

                            MLResult.Code resultCode = NativeBindings.MLMediaDRMSignRSA(this.Drm.handle, ref this.id, ref rsaParamNative, ref messageByteArray, out NativeBindings.MLMediaDRMByteArray signatureByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMSignRSA));
                            signature = signatureByteArray.Bytes;

                            keySetIdByteArray.FreeResources();
                            rsaParamNative.FreeResources();
                            messageByteArray.FreeResources();
                            NativeBindings.MLMediaDRMByteArrayRelease(ref signatureByteArray);

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Perform a signature verification using the specified algorithm (if specified) over the message data referenced by the
                        /// message parameter.  The key must have been loaded into the session using MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("Verify has been deprecated and will be removed in a future release.")]
                        public MLResult Verify(HMACParam hmacParam, byte[] message, byte[] signature, out bool isMatch)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(hmacParam.KeySetId);

                            NativeBindings.MLMediaDRMHMACInputParam hmacParamNative = new NativeBindings.MLMediaDRMHMACInputParam();
                            hmacParamNative.AllocResources(hmacParam.Algorithm, keySetIdByteArray);

                            NativeBindings.MLMediaDRMByteArray messageByteArray = new NativeBindings.MLMediaDRMByteArray();
                            messageByteArray.AllocResources(message);

                            NativeBindings.MLMediaDRMByteArray signatureByteArray = new NativeBindings.MLMediaDRMByteArray();
                            signatureByteArray.AllocResources(signature);

                            MLResult.Code resultCode = NativeBindings.MLMediaDRMVerify(this.Drm.handle, ref this.id, ref hmacParamNative, ref messageByteArray, ref signatureByteArray, out isMatch);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMVerify));

                            keySetIdByteArray.FreeResources();
                            hmacParamNative.FreeResources();
                            messageByteArray.FreeResources();
                            signatureByteArray.FreeResources();

                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Request an informative description of the key status for the session.  The status is in the form of {key, value} pairs.
                        /// Since DRM license policies vary by vendor, the specific status field names are determined by each DRM vendor.  Refer to
                        /// your DRM provider documentation for definitions of the field names for a particular DRM engine plugin.
                        /// </summary>
                        [Obsolete("GetKeyValues has been deprecated and will be removed in a future release.")]
                        public MLResult GetKeyValues(out KeyValuePair<string, string>[] keyValues)
                        {
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMQueryKeyStatus(this.Drm.handle, ref this.id, out NativeBindings.MLMediaDRMKeyValueArray keyValuesNative);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMQueryKeyStatus));
                            keyValues = keyValuesNative.KeyValues;
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Restore persisted offline keys into a new session.  key_set_id identifies the keys to load, obtained from a prior call
                        /// to MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [Obsolete("RestoreKeys has been deprecated and will be removed in a future release.")]
                        public MLResult RestoreKeys(byte[] keySetId)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(keySetId);
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMRestoreKeys(this.Drm.handle, ref this.id, ref keySetIdByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMRestoreKeys));
                            keySetIdByteArray.FreeResources();
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Add a {key, value} pair to the array of {key, value} pairs.
                        /// </summary>
                        [Obsolete("AddKeyValue has been deprecated and will be removed in a future release.")]
                        public MLResult AddKeyValue(KeyValuePair<string, string> pair, out KeyValuePair<string, string>[] allKeys)
                        {
                            NativeBindings.MLMediaDRMKeyValue keyValueNative = new NativeBindings.MLMediaDRMKeyValue(pair);
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMKeyValueArrayAdd(ref keyValueNative, out NativeBindings.MLMediaDRMKeyValueArray keyValuesNative);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMKeyValueArrayAdd));
                            allKeys = keyValuesNative.KeyValues;
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Allocate array of {key, value} pairs.
                        /// </summary>
                        [Obsolete("AllocateKeyValueArray has been deprecated and will be removed in a future release.")]
                        public MLResult AllocateKeyValueArray(uint size, out KeyValuePair<string, string>[] allKeys)
                        {
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMKeyValueArrayAllocate(size, out NativeBindings.MLMediaDRMKeyValueArray keyValuesNative);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMKeyValueArrayAllocate));
                            allKeys = keyValuesNative.KeyValues;
                            return MLResult.Create(resultCode);
                        }

                        /// <summary>
                        /// ***CURRENTLY UNTESTED ON CAPI***
                        /// Remove the current keys from a session.
                        /// </summary>
                        [Obsolete("RemoveKeys has been deprecated and will be removed in a future release.")]
                        public MLResult RemoveKeys(byte[] keySetId)
                        {
                            NativeBindings.MLMediaDRMByteArray keySetIdByteArray = new NativeBindings.MLMediaDRMByteArray();
                            keySetIdByteArray.AllocResources(keySetId);
                            MLResult.Code resultCode = NativeBindings.MLMediaDRMRemoveKeys(this.Drm.handle, ref this.id, ref keySetIdByteArray);
                            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaDRMRemoveKeys));
                            keySetIdByteArray.FreeResources();
                            return MLResult.Create(resultCode);
                        }
                    }

                    /// <summary>
                    /// MediaPlayer DRM Info for a Media Track.
                    /// </summary>
                    public struct Info
                    {
                        /// <summary>
                        /// The UUIDs provided by the MLMediaPlayerTrackDRMInfo structure.
                        /// </summary>
                        internal MagicLeapNativeBindings.MLUUID[] UUIDS;

                        /// <summary>
                        /// The PSSH keys provided by the MLMediaPlayerTrackDRMInfo structure.
                        /// </summary>
                        internal Player.NativeBindings.MLMediaPlayerPSSHEntry[] PSSHEntries;

                        /// <summary>
                        /// Media track type, can be either audio or video.
                        /// </summary>
                        public Track.Type TrackType;
                    };

                    /// <summary>
                    /// MediaDRM event info associated with a session.
                    /// </summary>
                    public readonly struct Event
                    {
                        /// <summary>
                        /// Provides initilization of the <see cref="Event" /> class with the given parameters.
                        /// </summary>
                        internal Event(byte[] data, byte[] sessionId, EventType type, int extra)
                        {
                            this.Data = data;
                            this.SessionId = sessionId;
                            this.Type = type;
                            this.Extra = extra;
                        }

                        /// <summary>
                        /// Optional data that may be associated with the event.
                        /// </summary>
                        public readonly byte[] Data;

                        /// <summary>
                        /// DRM session id associated with the event. 
                        /// </summary>
                        public readonly byte[] SessionId;

                        /// <summary>
                        /// Type of Media DRM event.
                        /// </summary>
                        public readonly EventType Type;

                        /// <summary>
                        /// Extra Secondary error code.
                        /// </summary>
                        public readonly int Extra;

                        public override string ToString() => $"data:  {this.Data}, sessionId:  {this.SessionId}, type:  {this.Type}, extra:  {this.Extra}";

                    }

                    /// <summary>
                    /// MediaDRM expiration update info associated with a session.
                    /// </summary>
                    public readonly struct Expiration
                    {
                        /// <summary>
                        /// Provides initilization of the <see cref="Event" /> class with the given parameters.
                        /// </summary>
                        internal Expiration(byte[] sessionId, ulong timems)
                        {
                            this.SessionId = sessionId;
                            this.TimeMs = timems;
                        }

                        /// <summary>
                        /// DRM session id associated with the event. 
                        /// </summary>
                        public readonly byte[] SessionId;

                        /// <summary>
                        /// The new expiration time for the keys in the session. The time is in milliseconds ,relative to the Unix epoch. 
                        /// A time of 0 indicates that the keys never expire.
                        /// </summary>
                        public readonly ulong TimeMs;

                        public override string ToString() => $"sessionId:  {this.SessionId}, timeMs:  {this.TimeMs}";

                    }

                    /// <summary>
                    /// MediaDRM session key info.
                    /// </summary>
                    public readonly struct Key
                    {
                        /// <summary>
                        /// Provides initilization of the <see cref="Key" /> class with the given parameters.
                        /// </summary>
                        internal Key(byte[] id, KeyStatus status)
                        {
                            this.Id = id;
                            this.Status = status;
                        }

                        /// <summary>
                        /// Id of the key.
                        /// </summary>
                        public readonly byte[] Id;

                        /// <summary>
                        /// Status code of the corresponding key.
                        /// </summary>
                        public readonly KeyStatus Status;

                        public override string ToString() => $"Id: {this.Id}, Status: {this.Status}";

                    }

                    /// <summary>
                    /// MediaDRM session keys status change info.
                    /// </summary>
                    public readonly struct KeyStatuses
                    {
                        /// <summary>
                        /// Provides initilization of the <see cref="KeyStatuses" /> class with the given parameters.
                        /// </summary>
                        internal KeyStatuses(byte[] sessionId, Key[] keys, bool hasNewUseableKey)
                        {
                            this.SessionId = sessionId;
                            this.Keys = keys;
                            this.HasNewUseableKey = hasNewUseableKey;
                        }

                        /// <summary>
                        /// DRM session id associated with the event. 
                        /// </summary>
                        public readonly byte[] SessionId;

                        /// <summary>
                        /// Key entries.
                        /// </summary>
                        public readonly Key[] Keys;

                        /// <summary>
                        /// Indicates if a key has been added that is usable, which may trigger an attempt to resume playback on the media stream if it is currently blocked waiting for a key.
                        /// </summary>
                        public readonly bool HasNewUseableKey;

                        public override string ToString()
                        {
                            string keysString = string.Empty;
                            foreach (Key key in this.Keys)
                                keysString += key + ", ";

                            return $"sessionId:  {this.SessionId}, hasNewUseableKey: {this.HasNewUseableKey} keys: {keysString}";
                        }
                    }
                }
            }
        }
    }
}

