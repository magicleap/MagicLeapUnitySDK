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
    using UnityEngine.XR.MagicLeap.Native;

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
                    /// See ml_media_drm.h for additional comments.
                    /// </summary>
                    internal class NativeBindings : MagicLeapNativeBindings
                    {
                        private const string PROPERTY_VENDOR = "vendor";
                        private const string PROPERTY_VERSION = "version";
                        private const string PROPERTY_DESCRIPTION = "description";
                        private const string PROPERTY_ALGORITHMS = "algorithms";
                        private const string PROPERTY_DEVICE_UNIQUE_ID = "deviceUniqueId";

                        public delegate void OnEventDelegate(ulong handle, [In] ref MLMediaDRMEventInfo info);
                        public delegate void OnExpirationUpdateDelegate(ulong handle, [In] ref MLMediaDRMExpirationUpdateInfo info);
                        public delegate void OnKeyStatusChangeDelegate(ulong handle, [In] ref MLMediaDRMKeyStatusInfo info);

                        internal static readonly Dictionary<Property, string> Properties = new Dictionary<Property, string>()
                        {
                            { Property.Vendor, PROPERTY_VENDOR},
                            { Property.Version, PROPERTY_VERSION},
                            { Property.Description, PROPERTY_DESCRIPTION},
                            { Property.Algorithms, PROPERTY_ALGORITHMS},
                            { Property.DeviceUniqueId, PROPERTY_DEVICE_UNIQUE_ID},
                        };

                        /// <summary>
                        /// Data type containing {key, value} pair.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMKeyValue
                        {
                            public MLMediaDRMKeyValue(KeyValuePair<string, string> pair)
                            {
                                this.Key = pair.Key;
                                this.Value = pair.Value;
                            }

                            /// <summary>
                            /// Key.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public readonly string Key;

                            /// <summary>
                            /// Value.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public readonly string Value;

                            public KeyValuePair<string, string> KeyValue => new KeyValuePair<string, string>(this.Key, this.Value);
                        };

                        /// <summary>
                        /// Data type containing byte array buffer and the size.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMByteArray
                        {
                            /// <summary>
                            /// Byte array buffer.
                            /// </summary>
                            public IntPtr Data;

                            /// <summary>
                            /// Size of Data.
                            /// </summary>
                            public ulong Length;

                            public void AllocResources(byte[] data)
                            {
                                this.Data = Marshal.AllocHGlobal(data.Length);
                                Marshal.Copy(data, 0, this.Data, data.Length);
                                this.Length = (ulong)data.Length;
                            }

                            public void FreeResources()
                            {
                                Marshal.FreeHGlobal(this.Data);
                            }

                            public byte[] Bytes
                            {
                                get
                                {
                                    if (IntPtr.Zero == this.Data)
                                        return new byte[0];

                                    byte[] bytes = new byte[this.Length];
                                    Marshal.Copy(this.Data, bytes, 0, (int)this.Length);
                                    return bytes;
                                }
                            }
                        };

                        /// <summary>
                        /// Data type that encapsulates algorithm, key_set_id for HMAC based Sign/Verify.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMHMACInputParam
                        {
                            /// <summary>
                            /// The MAC algorithm used for the MAC-HASH operation. "HmacSHA256" is the only supported algorithm now.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public string Algorithm;

                            /// <summary>
                            /// The identifier for the key to be used.
                            /// </summary>
                            public IntPtr KeySetId;

                            internal void AllocResources(string algorithm, MLMediaDRMByteArray keySetId)
                            {
                                this.Algorithm = algorithm;
                                this.KeySetId = Marshal.AllocHGlobal(Marshal.SizeOf(keySetId));
                                Marshal.StructureToPtr(keySetId, this.KeySetId, false);
                            }

                            internal void FreeResources()
                            {
                                Marshal.FreeHGlobal(this.KeySetId);
                            }
                        };

                        /// <summary>
                        /// Data type that encapsulates algorithm, wrapped_key for RSA operation.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMRSAInputParam
                        {

                            /// <summary>
                            /// The MAC algorithm used for padding/signing. "HmacSHA256" is the only supported algorithm now.
                            /// </summary>
                            public string Algorithm;

                            /// <summary>
                            /// The wrapped RSA key to be used.
                            /// </summary>
                            public IntPtr WrappedRsaKey;

                            internal void AllocResources(string algorithm, MLMediaDRMByteArray keySetId)
                            {
                                this.Algorithm = algorithm;
                                this.WrappedRsaKey = Marshal.AllocHGlobal(Marshal.SizeOf(keySetId));
                                Marshal.StructureToPtr(keySetId, this.WrappedRsaKey, false);
                            }

                            internal void FreeResources()
                            {
                                Marshal.FreeHGlobal(this.WrappedRsaKey);
                            }
                        };

                        /// <summary>
                        /// Data type containing list of byte array buffers and the size.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMByteArrayList
                        {
                            /// <summary>
                            /// Byte array buffer.
                            /// </summary>
                            public IntPtr Ptr;

                            /// <summary>
                            /// Length of #ptr.
                            /// </summary>
                            public uint Length;

                            internal byte[][] ByteArrays
                            {
                                get
                                {
                                    byte[][] byteArrays = new byte[this.Length][];
                                    IntPtr walkPtr = this.Ptr;
                                    for (int i = 0; i < this.Length; ++i)
                                    {
                                        NativeBindings.MLMediaDRMByteArray secureStopNative = Marshal.PtrToStructure<MLMediaDRMByteArray>(walkPtr);
                                        byteArrays[i] = secureStopNative.Bytes;
                                        walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<IntPtr>());
                                    }

                                    return byteArrays;
                                }
                            }
                        };

                        /// <summary>
                        /// Data type containing array of {key, value} pair.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMKeyValueArray
                        {
                            /// <summary>
                            /// Array of {key, value} pair.
                            /// </summary>
                            public readonly IntPtr Ptr;

                            /// <summary>
                            /// Length of #ptr.
                            /// </summary>
                            public readonly uint Length;

                            /// <summary>
                            /// Index.
                            /// </summary>
                            public readonly uint Index;

                            public KeyValuePair<string, string>[] KeyValues
                            {
                                get
                                {
                                    KeyValuePair<string, string>[] keyValues = new KeyValuePair<string, string>[this.Length];
                                    IntPtr walkPtr = this.Ptr;
                                    for (uint i = this.Index; i < this.Length; ++i)
                                    {
                                        MLMediaDRMKeyValue keyVal = Marshal.PtrToStructure<MLMediaDRMKeyValue>(walkPtr);
                                        keyValues[i] = keyVal.KeyValue;
                                        walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<IntPtr>());
                                    }

                                    return keyValues;
                                }
                            }
                        };

                        /// <summary>
                        /// Data type that encapsulates Key Request input arguments.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMKeyRequestInputParam
                        {
                            internal MLMediaDRMKeyRequestInputParam(string mimeType, KeyType keyType)
                            {
                                this.MimeType = mimeType;
                                this.KeyType = keyType;
                                this.InitData = IntPtr.Zero;
                                this.OptionalParams = IntPtr.Zero;
                            }

                            internal void AllocResources(Player.NativeBindings.MLMediaPlayerPSSHEntry psshEntry)
                            {
                                this.InitData = Marshal.AllocHGlobal(Marshal.SizeOf<NativeBindings.MLMediaDRMByteArray>());
                                NativeBindings.MLMediaDRMByteArray pssh = new NativeBindings.MLMediaDRMByteArray();
                                pssh.Data = psshEntry.data;
                                pssh.Length = psshEntry.size;
                                Marshal.StructureToPtr(pssh, this.InitData, false);
                            }

                            internal void FreeResources()
                            {
                                Marshal.FreeHGlobal(this.InitData);
                                Marshal.FreeHGlobal(this.OptionalParams);
                            }

                            /// <summary>
                            /// The container-specific data, its meaning is interpreted based on the mime-type provided in the mime_type parameter.  It
                            /// could contain, for example, the content ID, key ID or other data obtained from the content metadata that is required in
                            /// generating the key request.
                            /// </summary>
                            public IntPtr InitData;

                            /// <summary>
                            /// The mime type of the content.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public string MimeType;

                            /// <summary>
                            /// The type of the request.  The request may be to acquire keys for streaming or offline content, or to release previously
                            /// acquired keys, which are identified by a key_set_id.
                            /// </summary>
                            public KeyType KeyType;

                            /// <summary>
                            /// Optional parameters included in the key request message to allow a client application to provide additional message
                            /// parameters to the server.
                            /// </summary>
                            public readonly IntPtr OptionalParams;
                        };

                        /// <summary>
                        /// Data type that encapsulates either of the following along with an URL: For Key Request Message: an opaque key request
                        /// byte array that should be delivered to the license server.  For Provision Request Message: an opaque provision request
                        /// byte array that should be delivered to the provisioning server.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMRequestMessage
                        {
                            /// <summary>
                            /// The opaque request byte array.
                            /// </summary>
                            public readonly NativeBindings.MLMediaDRMByteArray Request;

                            /// <summary>
                            /// The recommended URL to deliver the request to.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public readonly string DefaultURL;
                        };

                        /// <summary>
                        /// Data type that encapsulates algorithm, key_set_id and IV for Encryption/Decryption.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMCryptoInputParam
                        {
                            /// <summary>
                            /// The cipher mode used for the crypto operation. "AES/CBC/NoPadding" is the only supported algorithm now.
                            /// </summary>
                            [MarshalAs(UnmanagedType.LPStr)]
                            public string Algorithm;

                            /// <summary>
                            /// The identifier for the key to be used.
                            /// </summary>
                            public IntPtr KeySetId;

                            /// <summary>
                            /// The initial vector used for the crypto operation.
                            /// </summary>
                            public IntPtr InitialVector;

                            internal void AllocResources(string algorithm, MLMediaDRMByteArray keySetId, MLMediaDRMByteArray initialVector)
                            {
                                this.Algorithm = algorithm;

                                this.KeySetId = Marshal.AllocHGlobal(Marshal.SizeOf(keySetId));
                                Marshal.StructureToPtr(keySetId, this.KeySetId, false);

                                this.InitialVector = Marshal.AllocHGlobal(Marshal.SizeOf(initialVector));
                                Marshal.StructureToPtr(initialVector, this.InitialVector, false);
                            }

                            internal void FreeResources()
                            {
                                Marshal.FreeHGlobal(this.KeySetId);
                                Marshal.FreeHGlobal(this.InitialVector);
                            }
                        };

                        /// <summary>
                        /// Data type containing array of {key, value} pair.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMEventCallbacks
                        {
                            internal static MLMediaDRMEventCallbacks Create()
                            {
                                MLMediaDRMEventCallbacks callbacks = new MLMediaDRMEventCallbacks();
                                callbacks.version = 1;
                                callbacks.onEvent = NativeBindings.OnEvent;
                                callbacks.onExpirationUpdate = NativeBindings.OnExpirationUpdate;
                                callbacks.onKeyStatusChange = NativeBindings.OnKeyStatusChange;
                                return callbacks;
                            }

                            private uint version;

                            public OnEventDelegate onEvent;

                            public OnExpirationUpdateDelegate onExpirationUpdate;

                            public OnKeyStatusChangeDelegate onKeyStatusChange;
                        };

                        /// <summary>
                        /// MediaDRM event info associated with a session.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMEventInfo
                        {
                            /// <summary>
                            /// DRM session id associated with the event. 
                            /// </summary>
                            public readonly IntPtr sessionIdByteArray;

                            /// <summary>
                            /// Type of Media DRM event.
                            /// </summary>
                            public readonly EventType eventType;

                            /// <summary>
                            /// Extra Secondary error code.
                            /// </summary>
                            public readonly int extra;

                            /// <summary>
                            /// Optional data that may be associated with the event.
                            /// </summary>
                            public readonly IntPtr eventDataByteArray;

                            /// <summary>
                            /// User data as passed to MLMediaDRMPlayerSetEventCallbacksEx().
                            /// </summary>
                            public readonly IntPtr data;
                        };

                        /// <summary>
                        /// MediaDRM expiration update info associated with a session.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMExpirationUpdateInfo
                        {
                            /// <summary>
                            /// DRM session id associated with the event. 
                            /// </summary>
                            public readonly IntPtr sessionIdByteArray;

                            /// <summary>
                            /// The new expiration time for the keys in the session. The time is in milliseconds ,relative to the Unix epoch. 
                            /// A time of 0 indicates that the keys never expire.
                            /// </summary>
                            public readonly ulong expirationTimeMs;

                            /// <summary>
                            /// User data as passed to MLMediaDRMPlayerSetEventCallbacksEx().
                            /// </summary>
                            public readonly IntPtr data;
                        };

                        /// <summary>
                        /// MediaDRM session key status.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public readonly struct MLMediaDRMKeyStatus
                        {
                            /// <summary>
                            /// KeyID that belongs to one of th DRM session key. 
                            /// </summary>
                            public readonly MLMediaDRMByteArray keyId;

                            /// <summary>
                            /// Status code of the corresponding key.
                            /// </summary>
                            public readonly KeyStatus status;

                            public Key Data => new Key(this.keyId.Bytes, this.status);
                        }


                        /// <summary>
                        /// MediaDRM session keys status change info.
                        /// </summary>
                        [StructLayout(LayoutKind.Sequential)]
                        public struct MLMediaDRMKeyStatusInfo
                        {
                            /// <summary>
                            /// DRM session id associated with the event. 
                            /// </summary>
                            public IntPtr sessionIdByteArray;

                            /// <summary>
                            /// Number of KeyStatus entries.
                            /// </summary>
                            public ulong keyStatusCount;

                            /// <summary>
                            ///  Pointer to array of size keyStatusCount.
                            /// </summary>
                            public IntPtr keyStatuses;

                            /// <summary>
                            /// Indicates if a key has been added that is usable, which may trigger an attempt to resume playback on the media stream if it is currently blocked waiting for a key.
                            /// </summary>
                            [MarshalAs(UnmanagedType.I1)]
                            public bool hasNewUsableKey;

                            /// <summary>
                            /// User data as passed to MLMediaDRMPlayerSetEventCallbacksEx().
                            /// </summary>
                            public IntPtr data;
                        };

                        /// <summary>
                        /// Query if the given scheme identified by its UUID is supported on this device. And whether the drm plugin is able to
                        /// handle the media container format.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMIsCryptoSchemeSupported(Native.MagicLeapNativeBindings.MLUUID Uuid, IntPtr MimeType, [MarshalAs(UnmanagedType.I1)] out bool supported);

                        /// <summary>
                        /// Release Request Message.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMRequestMessageRelease(ref MLMediaDRMRequestMessage Request);

                        /// <summary>
                        /// Perform a signature verification using the specified algorithm (if specified) over the message data referenced by the
                        /// message parameter.  The key must have been loaded into the session using MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMVerify(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMHMACInputParam HMACParam, [In] ref MLMediaDRMByteArray Message, [In] ref MLMediaDRMByteArray Signature, [MarshalAs(UnmanagedType.I1)] out bool IsMatch);

                        /// <summary>
                        /// Generate a signature using the specified algorithm (if provided) over the message data and store the signature.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMSign(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMHMACInputParam HMACParam, [In] ref MLMediaDRMByteArray Message, out MLMediaDRMByteArray Signature);

                        /// <summary>
                        /// Remove the current keys from a session.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMRemoveKeys(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMByteArray KeySetId);

                        /// <summary>
                        /// String property name: identifies the maker of the DRM engine plugin.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMByteArrayAllocate(uint Size, out MLMediaDRMByteArray OutArray);

                        /// <summary>
                        /// Allocate and Copy to byte array buffer.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMByteArrayAllocAndCopy(IntPtr CopyFrom, uint Size, out MLMediaDRMByteArray OutArray);

                        /// <summary>
                        /// Allocate array of {key, value} pairs.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMKeyValueArrayAllocate(uint Size, out MLMediaDRMKeyValueArray OutArray);

                        /// <summary>
                        /// Create a
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMCreate(Native.MagicLeapNativeBindings.MLUUID uuid, out ulong handle);

                        /// <summary>
                        /// Release the
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMRelease(ulong drmHandle);

                        /// <summary>
                        /// Release byte array buffer.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMByteArrayRelease(ref MLMediaDRMByteArray array);

                        /// <summary>
                        /// Release list of byte array buffer.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMByteArrayListRelease(ref MLMediaDRMByteArrayList Array);

                        /// <summary>
                        /// Release {key, value} pair array.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMKeyValueArrayRelease(ref MLMediaDRMKeyValueArray Array);

                        /// <summary>
                        /// Add a {key, value} pair to the array of {key, value} pairs.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMKeyValueArrayAdd([In] ref MLMediaDRMKeyValue Pair, out MLMediaDRMKeyValueArray OutArray);

                        /// <summary>
                        /// Register a callback to be invoked when an event occurs.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMSetOnEventListenerEx(ulong MediaDrm, ref MLMediaDRMEventCallbacks Listener, IntPtr data);

                        /// <summary>
                        /// Opens a new session. A session ID is returned.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMOpenSession(ulong MediaDrm, out MLMediaDRMByteArray sessionId);

                        /// <summary>
                        /// Closes a session that was previously opened.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMCloseSession(ulong drmHandle, ref MLMediaDRMByteArray sessionId);

                        /// <summary>
                        /// A key request/response exchange occurs between the app and a license server to obtain or release keys used to decrypt
                        /// encrypted content.  MLMediaDRMGetKeyRequest() is used to obtain an opaque key request byte array that is delivered to the
                        /// license server.  The opaque key request byte array is returned in out_key_request.  request The recommended URL to
                        /// deliver the key request to is returned in out_key_request.  default_URL.  After the app has received the key request response
                        /// from the server, it should deliver to the response to the DRM engine plugin using the method
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetKeyRequest(ulong drmHandle, [In] ref MLMediaDRMByteArray sessionId, [In] ref MLMediaDRMKeyRequestInputParam keyRequestParam, out MLMediaDRMRequestMessage keyRequest);

                        /// <summary>
                        /// A key response is received from the license server by the app, then it is provided to the DRM engine plugin using
                        /// MLMediaDRMProvideKeyResponse().  When the response is for an offline key request, a key_set_id is returned that can be used to
                        /// later restore the keys to a new session with restoreKeys().  When the response is for a streaming or release request, a
                        /// null key_set_id is returned.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMProvideKeyResponse(ulong drmHandle, [In] ref MLMediaDRMByteArray sessionId, [In] ref MLMediaDRMByteArray response, out MLMediaDRMByteArray keySetId);

                        /// <summary>
                        /// Restore persisted offline keys into a new session.  key_set_id identifies the keys to load, obtained from a prior call
                        /// to MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMRestoreKeys(ulong MediaDrm, [In] ref MLMediaDRMByteArray sessionId, [In] ref MLMediaDRMByteArray KeySetId);

                        /// <summary>
                        /// Request an informative description of the key status for the session.  The status is in the form of {key, value} pairs.
                        /// Since DRM license policies vary by vendor, the specific status field names are determined by each DRM vendor.  Refer to
                        /// your DRM provider documentation for definitions of the field names for a particular DRM engine plugin.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMQueryKeyStatus(ulong MediaDrm, [In] ref MLMediaDRMByteArray sessionId, out MLMediaDRMKeyValueArray OutInfoMap);

                        /// <summary>
                        /// A provision request/response exchange occurs between the app and a provisioning server to retrieve a device certificate.
                        /// If provisioning is required, the #EVENT_PROVISION_REQUIRED event will be sent to the event handler.
                        /// MLMediaDRMGetProvisionRequest() is used to obtain the opaque provision request byte array that should be delivered to the provisioning
                        /// server.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetProvisionRequest(ulong drmHandle, [MarshalAs(UnmanagedType.LPStr)] string certType, out MLMediaDRMRequestMessage provisionRequest);

                        /// <summary>
                        /// After a provision response is received by the app, it is provided to the DRM engine plugin using this method.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMProvideProvisionResponse(ulong drmHandle, [In] ref MLMediaDRMByteArray response, out MLMediaDRMByteArray certificate, out MLMediaDRMByteArray wrappedKey);

                        /// <summary>
                        /// Access all secure stops.  Secure Stop: A means of enforcing limits on the number of concurrent streams per subscriber
                        /// across devices is provided via #SecureStop.  This is achieved by securely monitoring the lifetime of sessions.
                        /// Information from the server related to the current playback session is written to persistent storage on the device when each
                        /// #MediaCrypto object is created.  In the normal case, playback will be completed, the session destroyed and the Secure Stops
                        /// will be queried.  The app queries secure stops and forwards the secure stop message to the server which verifies the
                        /// signature and notifies the server side database that the session destruction has been confirmed.  The persisted record on
                        /// the client is only removed after positive confirmation that the server received the message using releaseSecureStops().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetSecureStops(ulong MediaDrm, out MLMediaDRMByteArrayList SecureStops);

                        /// <summary>
                        ///  Access secure stop by secure stop ID. \see MLMediaDRMGetSecureStops()
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetSecureStop(ulong MediaDrm, [In] ref MLMediaDRMByteArray SecureStopId, out MLMediaDRMByteArray SecureStop);

                        /// <summary>
                        /// Process the SecureStop server response message.  After authenticating the message, remove the SecureStops identified in
                        /// the response.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMReleaseSecureStops(ulong MediaDrm, [In] ref MLMediaDRMByteArray SecureStop);

                        /// <summary>
                        /// Remove all the SecureStops.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMReleaseAllSecureStops(ulong MediaDrm);

                        /// <summary>
                        /// Read a DRM engine plugin String property value, given the property name.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetPropertyString(ulong MediaDrm, [MarshalAs(UnmanagedType.LPStr)] string PropertyName, [MarshalAs(UnmanagedType.LPStr)] out string OutPropertyValue);

                        /// <summary>
                        /// Read a DRM engine plugin byte array property value, given the property name.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMGetPropertyByteArray(ulong MediaDrm, [MarshalAs(UnmanagedType.LPStr)] string PropertyName, out MLMediaDRMByteArray OutPropertyValue);

                        /// <summary>
                        /// Set a DRM engine plugin String property value.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMSetPropertyString(ulong MediaDrm, [MarshalAs(UnmanagedType.LPStr)] string PropertyName, [MarshalAs(UnmanagedType.LPStr)] string PropertyValue);

                        /// <summary>
                        /// Set a DRM engine plugin byte array property value.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMSetPropertyByteArray(ulong MediaDrm, [MarshalAs(UnmanagedType.LPStr)] string PropertyName, [In] ref MLMediaDRMByteArray PropertyValue);

                        /// <summary>
                        /// Encrypt the data referenced by input using algorithm if specified, and write the encrypted result into output.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMEncrypt(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMCryptoInputParam CryptoKeyParam, [In] ref MLMediaDRMByteArray Input, out MLMediaDRMByteArray Output);

                        /// <summary>
                        /// Decrypt the data referenced by input using algorithm if specified, and write the encrypted result into output.  The key
                        /// to use is identified by the 16 byte keyId.  The key must have been loaded into the session using
                        /// MLMediaDRMProvideKeyResponse().
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMDecrypt(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMCryptoInputParam CryptoKeyParam, [In] ref MLMediaDRMByteArray Input, out MLMediaDRMByteArray Output);

                        /// <summary>
                        /// Generate a signature using the specified RSA Key and algorithm.
                        /// </summary>
                        [DllImport(MLMediaDRMDll, CallingConvention = CallingConvention.Cdecl)]
                        public static extern MLResult.Code MLMediaDRMSignRSA(ulong MediaDrm, [In] ref MLMediaDRMByteArray SessionId, [In] ref MLMediaDRMRSAInputParam HMACParam, [In] ref MLMediaDRMByteArray Message, out MLMediaDRMByteArray Signature);

                        /// <summary>
                        /// Callback for the OnVideoSizeChanged event.
                        /// </summary>
                        [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnEventDelegate))]
                        private static void OnEvent(ulong MediaDrm, [In] ref MLMediaDRMEventInfo info)
                        {
                            GCHandle gcHandle = GCHandle.FromIntPtr(info.data);
                            DRM drm = gcHandle.Target as DRM;
                            if (drm != null)
                            {
                                MLMediaDRMByteArray eventData = Marshal.PtrToStructure<MLMediaDRMByteArray>(info.eventDataByteArray);
                                MLMediaDRMByteArray sessionId = Marshal.PtrToStructure<MLMediaDRMByteArray>(info.sessionIdByteArray);

                                // free these?

                                Event eventInfo = new Event(eventData.Bytes, sessionId.Bytes, info.eventType, info.extra);
                                MLThreadDispatch.Call(drm, eventInfo, drm.OnEvent);
                            }
                        }

                        /// <summary>
                        /// Callback for the OnVideoSizeChanged event.
                        /// </summary>
                        [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnExpirationUpdateDelegate))]
                        private static void OnExpirationUpdate(ulong MediaDrm, [In] ref MLMediaDRMExpirationUpdateInfo info)
                        {
                            GCHandle gcHandle = GCHandle.FromIntPtr(info.data);
                            DRM drm = gcHandle.Target as DRM;
                            if (drm != null)
                            {
                                MLMediaDRMByteArray sessionIdArray = Marshal.PtrToStructure<MLMediaDRMByteArray>(info.sessionIdByteArray);
                                Expiration expiration = new Expiration(sessionIdArray.Bytes, info.expirationTimeMs);
                                MLThreadDispatch.Call(drm, expiration, drm.OnExpirationUpdate);
                            }
                        }

                        /// <summary>
                        /// Callback for the OnVideoSizeChanged event.
                        /// </summary>
                        [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnKeyStatusChangeDelegate))]
                        private static void OnKeyStatusChange(ulong MediaDrm, [In] ref MLMediaDRMKeyStatusInfo info)
                        {
                            GCHandle gcHandle = GCHandle.FromIntPtr(info.data);
                            DRM drm = gcHandle.Target as DRM;
                            if (drm != null)
                            {
                                MLMediaDRMByteArray sessionIdArray = Marshal.PtrToStructure<MLMediaDRMByteArray>(info.sessionIdByteArray);
                                Key[] keys = new Key[info.keyStatusCount];
                                IntPtr walkPtr = info.keyStatuses;
                                for (ulong i = 0; i < info.keyStatusCount; ++i)
                                {
                                    keys[i] = Marshal.PtrToStructure<MLMediaDRMKeyStatus>(walkPtr).Data;
                                    walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<IntPtr>());
                                }

                                KeyStatuses keyStatuses = new KeyStatuses(sessionIdArray.Bytes, keys, info.hasNewUsableKey);
                                MLThreadDispatch.Call(drm, keyStatuses, drm.OnKeyStatusChange);
                            }
                        }
                    }
                }
            }
        }
    }
}
