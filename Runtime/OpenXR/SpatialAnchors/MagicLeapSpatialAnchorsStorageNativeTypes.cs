using System;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.SpatialAnchors
{
    internal enum XrSpatialAnchorsStorageStructTypes : ulong
        {
            XrTypeSpatialAnchorsCreateStorageInfo = 1000141000U,
            XrTypeSpatialAnchorsQueryInfoRadius = 1000141001U,
            XrTypeSpatialAnchorsQueryCompletion = 1000141002U,
            XrTypeSpatialAnchorsCreateInfoFromUUIDs = 1000141003U,
            XrTypeSpatialAnchorsPublishInfo = 1000141004U,
            XrTypeSpatialAnchorsPublishCompletion = 1000141005U,
            XrTypeSpatialAnchorsDeleteInfo = 1000141006U,
            XrTypeSpatialAnchorsDeleteCompletion = 1000141007U,
            XrTypeSpatialAnchorsUpdateExpirationInfo = 1000141008U,
            XrTypeSpatialAnchorsUpdateExpirationCompletion = 1000141009U
        }

        internal struct XrSpatialAnchorsCreateStorageInfo
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrSpatialAnchorsQueryInfoBaseHeader
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrSpatialAnchorsQueryInfoRadius
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal ulong BaseSpace;
            internal Vector3 Center;
            internal long Time;
            internal float Radius;
        }

        internal unsafe struct XrSpatialAnchorsQueryCompletion
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal XrResult FutureResult;
            internal uint UuidCapacityInput;
            internal uint UuidCountOutput;
            internal XrUUID* Uuids;
        }

        internal unsafe struct XrSpatialAnchorsCreateInfoFromUuids
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal ulong Storage;
            internal uint UuidCount;
            internal XrUUID* Uuids;
        }

        internal unsafe struct XrSpatialAnchorsPublishInfo
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal uint AnchorCount;
            internal ulong* Anchors;
            internal ulong Expiration;
        }

        internal unsafe struct XrSpatialAnchorsPublishCompletion
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal XrResult FutureResult;
            internal uint UuidCount;
            internal XrUUID* Uuids;
        }

        internal unsafe struct XrSpatialAnchorsDeleteInfo
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal uint UuidCount;
            internal XrUUID* Uuids;
        }

        internal struct XrSpatialAnchorsDeleteCompletion
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal XrResult FutureResult;
        }

        internal unsafe struct XrSpatialAnchorsUpdateExpirationInfo
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal uint UuidCount;
            internal XrUUID* Uuids;
            internal ulong Expiration;
        }

        internal struct XrSpatialAnchorsUpdateExpirationCompletion
        {
            internal XrSpatialAnchorsStorageStructTypes Type;
            internal IntPtr Next;
            internal XrResult FutureResult;
        }
}
