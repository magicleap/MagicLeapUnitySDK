using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.XR.ARSubsystems;
#if UNITY_MAGICLEAP || UNITY_ANDROID
using UnityEngine.XR.MagicLeap.Native;
#endif
namespace UnityEngine.XR.MagicLeap
{
    internal sealed class ImageDatabase : MutableRuntimeReferenceImageLibrary
    {
        internal bool Changed { get; set; }

        internal bool EnforceMovingImagesMax { get; set; }

        internal int MaxMovingImages { get; set; }

        private List<ulong> movingImageTargetHandles = new List<ulong>();
        public class XRImgContainer
        {
            public XRImgContainer(XRReferenceImage refImg)
            {
                this.refImg = refImg;
            }

            public XRReferenceImage refImg;
        }

        internal XRTrackedImage[] TrackedImagesArray
        {
            get
            {
                var trackedImages = new XRTrackedImage[referenceImages.Count];
                int i = 0;
                foreach (var refImgAndTargetHandle in referenceImages)
                {
                    XRReferenceImage refImg = refImgAndTargetHandle.Key;
                    ulong imageHandle = refImgAndTargetHandle.Value;

                    ulong hand = (ulong)Random.Range(1, Int32.MaxValue);
                    XRTrackedImage img = new XRTrackedImage(new TrackableId(imageHandle, hand), refImg.guid, default, refImg.size, TrackingState.None, IntPtr.Zero);
                    trackedImages[i++] = img;
                }
                return trackedImages;
            }
        }

        internal Dictionary<XRReferenceImage, ulong> referenceImages = new Dictionary<XRReferenceImage, ulong>();

        private GCHandle imgDatabaseHandle;

        // Serialized database header bytes
        public const ulong kMagicBytes = 0x4e8d5ce7045df074;

        // Job handle is received from the subsystem that created this database.  Used to gate
        // add image jobs until the native tracker handle resource has completed (~6000ms).
        JobHandle m_CreateAssociatedNativeTrackerJobHandle;

        struct AddImageJob : IJob
        {
            [NativeDisableUnsafePtrRestriction]
            public IntPtr imgDatabasePtr;

            [DeallocateOnJobCompletion]
            [NativeDisableUnsafePtrRestriction]
            public NativeArray<byte> textureData;

            [DeallocateOnJobCompletion]
            [NativeDisableUnsafePtrRestriction]
            public NativeArray<byte> name;

            [NativeDisableUnsafePtrRestriction]
            public IntPtr nativeProvider;

            [NativeDisableUnsafePtrRestriction]
            public IntPtr database;

            public int width;

            public int height;

            [NativeDisableUnsafePtrRestriction]
            public IntPtr referenceImagePtr;

            public unsafe void Execute()
            {
                var success = AddImage();
            }

            private bool AddImage()
            {

                GCHandle handle = GCHandle.FromIntPtr(imgDatabasePtr);
                var imgDB = handle.Target as ImageDatabase;

                handle = GCHandle.FromIntPtr(referenceImagePtr);
                var container = (handle.Target as XRImgContainer);

                var referenceImage = container.refImg;
                if (imgDB.referenceImages.ContainsKey(referenceImage))
                    return false;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLImageTracker.Target.Settings settings = new MLImageTracker.Target.Settings()
                {
                    Name = referenceImage.name,
                    LongerDimension = referenceImage.size.x > referenceImage.size.y ? referenceImage.size.x : referenceImage.size.y,
                    IsEnabled = true,
                    IsStationary = true
                };

                ulong targetHandle = MagicLeap.Native.MagicLeapNativeBindings.InvalidHandle;
                var textureData = this.textureData.ToArray();
                var resultCode = MLImageTracker.NativeBindings.MLImageTrackerAddTargetFromArray(ImageTrackingSubsystem.MagicLeapProvider.trackerHandle, ref settings, textureData, (uint)this.width, (uint)this.height, MLImageTracker.ImageFormat.RGBA, ref targetHandle);
                if (!MLResult.IsOK(resultCode))
                    return false;
                imgDB.referenceImages.Add(referenceImage, targetHandle);
                imgDB.Changed = true;
#endif
                handle.Free();
                return true;
            }

        }

        public ImageDatabase(XRReferenceImageLibrary serializedLibrary, JobHandle imageTrackerCreationJobHandle)
        {
            m_CreateAssociatedNativeTrackerJobHandle = imageTrackerCreationJobHandle;

            this.imgDatabaseHandle = GCHandle.Alloc(this, GCHandleType.Weak);

            if (serializedLibrary != null && serializedLibrary.count > 0)
                DeserializeImageDatabaseFile(serializedLibrary);
        }

        ~ImageDatabase()
        {
            this.imgDatabaseHandle.Free();
        }

        static readonly TextureFormat[] k_SupportedFormats =
        {
            TextureFormat.Alpha8,
            TextureFormat.R8,
            TextureFormat.RFloat,
            TextureFormat.RGB24,
            TextureFormat.RGBA32,
            TextureFormat.ARGB32,
            TextureFormat.BGRA32,
        };

        public override int supportedTextureFormatCount => k_SupportedFormats.Length;

        protected override TextureFormat GetSupportedTextureFormatAtImpl(int index) => k_SupportedFormats[index];

        protected override JobHandle ScheduleAddImageJobImpl(
            NativeSlice<byte> imageBytes,
            Vector2Int sizeInPixels,
            TextureFormat format,
            XRReferenceImage referenceImage,
            JobHandle inputDeps)
        {

            var grayscaleImage = new NativeArray<byte>(
                sizeInPixels.x * sizeInPixels.y,
                Allocator.Persistent,
                NativeArrayOptions.UninitializedMemory);

            var jobDependencies = JobHandle.CombineDependencies(m_CreateAssociatedNativeTrackerJobHandle, inputDeps);

            var conversionHandle = ImageConversionJob.Schedule(imageBytes, sizeInPixels, format, grayscaleImage, jobDependencies);

            var refImgContainer = new XRImgContainer(referenceImage);
            GCHandle refImg = GCHandle.Alloc(refImgContainer);
            return new AddImageJob
            {
                imgDatabasePtr = (IntPtr)this.imgDatabaseHandle,
                referenceImagePtr = (IntPtr)refImg,
                textureData = grayscaleImage,
                width = sizeInPixels.x,
                height = sizeInPixels.y,
                name = new NativeArray<byte>(Encoding.UTF8.GetBytes(referenceImage.name + "\0"), Allocator.Persistent),
            }.Schedule(conversionHandle);
        }

        protected override XRReferenceImage GetReferenceImageAt(int index)
        {
            int i = 0;
            var vals = this.referenceImages.Keys; 
            foreach (var v in vals)
            {
                if (i == index)
                    return v;
                ++i;
            }
            return default;
        }

        public override int count => this.referenceImages.Count;

        unsafe void DeserializeImageDatabaseFile(XRReferenceImageLibrary serializedLibrary)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            for (int i = 0; i < serializedLibrary.count; ++i)
            {
                var referenceImage = serializedLibrary[i];
                var widthInPixels = referenceImage.texture.width;
                var heightInPixels = referenceImage.texture.height;

                var refImgContainer = new XRImgContainer(referenceImage);
                GCHandle refImg = GCHandle.Alloc(refImgContainer);

                var imageData = MLTextureUtils.ConvertToByteArray(referenceImage.texture, out int numChannels);

                new AddImageJob
                {
                    imgDatabasePtr = (IntPtr)this.imgDatabaseHandle,
                    referenceImagePtr = (IntPtr)refImg,
                    textureData = new NativeArray<byte>(imageData, Allocator.Persistent),
                    width = widthInPixels,
                    height = heightInPixels,
                    name = new NativeArray<byte>(Encoding.UTF8.GetBytes(referenceImage.name + "\0"), Allocator.Persistent),
                }.Schedule(m_CreateAssociatedNativeTrackerJobHandle);
            }
#endif
        }

        internal MLResult UpdateImageTarget(XRReferenceImage referenceImage, bool isEnabled, bool isStationary)
        {

            if (!referenceImages.ContainsKey(referenceImage))
                return MLResult.Create(MLResult.Code.InvalidParam, "XRReferenceImage is not apart of the current image database.");

            ulong imageTargetHandle = referenceImages[referenceImage];

            if (!isStationary && movingImageTargetHandles.Contains(imageTargetHandle))
                return MLResult.Create(MLResult.Code.Ok);

            if (EnforceMovingImagesMax && movingImageTargetHandles.Count + 1 > MaxMovingImages)
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "Exceeds the allowed maximum moving image count");
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLImageTracker.Target.Settings settings = new MLImageTracker.Target.Settings()
            {
                Name = referenceImage.name,
                LongerDimension = referenceImage.size.x > referenceImage.size.y ? referenceImage.size.x : referenceImage.size.y,
                IsEnabled = isEnabled,
                IsStationary = isStationary
            };

            MLResult.Code resultCode = MLImageTracker.NativeBindings.MLImageTrackerUpdateTargetSettings(ImageTrackingSubsystem.MagicLeapProvider.trackerHandle, imageTargetHandle, ref settings);

            if (!isStationary)
                movingImageTargetHandles.Add(imageTargetHandle);

            if (isStationary && movingImageTargetHandles.Contains(imageTargetHandle))
                movingImageTargetHandles.Remove(imageTargetHandle);

            return MLResult.Create(resultCode);
#else
            return MLResult.Create(MLResult.Code.Ok);
#endif
        }
    }
}
