// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    public class MLWebRTCVideoSinkBehavior : MonoBehaviour
    {
        [SerializeField, Tooltip("The display to show the remote NativeBuffer video capture on.")]
        private Renderer nativeRenderer = null;

        [SerializeField, Tooltip("The display to show the remote YUV video capture on.")]
        private Renderer yuvRenderer = null;

        [SerializeField, Tooltip("The display to show the remote RGB video capture on.")]
        private Renderer rgbRenderer = null;

        [SerializeField]
        private UnityEngine.UI.Text fpsText;

        // Only relevant for YUV / RGB frames
        private MLWebRTC.VideoSink.Frame currentFrame;

        private bool hasDeterminedFrameFormat;
#if UNITY_MAGICLEAP || UNITY_ANDROID
        private Timer fpsTimer;
#endif

        private Texture2D[] rawVideoTexturesYUV = new Texture2D[MLWebRTC.VideoSink.Frame.NativeImagePlanesLength[MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888]];
        private Texture2D[] rawVideoTexturesRGB = new Texture2D[MLWebRTC.VideoSink.Frame.NativeImagePlanesLength[MLWebRTC.VideoSink.Frame.OutputFormat.RGBA_8888]];
        private RenderTexture rawVideoTextureNative;
        private MLWebRTC.VideoSink.Renderer nativeBufferRenderer;

        private byte[] yChannelBuffer;
        private byte[] uChannelBuffer;
        private byte[] vChannelBuffer;

        private static readonly string[] samplerNamesYUV = new string[] { "_MainTex", "_UTex", "_VTex" };

        public MLWebRTC.VideoSink VideoSink { get; set; }

        private static bool loggedOnce = false;

        void Awake()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            VideoSink = MLWebRTC.VideoSink.Create(out MLResult result);
            VideoSink.OnFrameResolutionChanged += OnFrameResolutionChanged;
            fpsTimer = new Timer(1000);
            hasDeterminedFrameFormat = false;
#endif
        }

        private void OnFrameResolutionChanged(uint newWidth, uint newHeight)
        {
            CreateNativeBufferRenderTarget(newWidth, newHeight);
        }

        private void Update()
        {
            // TODO : also impl for VideoSink.Renderer.
#if UNITY_MAGICLEAP || UNITY_ANDROID
            float fps = 1.0f / fpsTimer.Elapsed();

            if (fpsText != null)
            {
                fpsText.text = string.Format("{0:0.00}", fps);
                if (fps >= 24f)
                {
                    fpsText.color = Color.green;
                }
                else if (fps >= 16f)
                {
                    fpsText.color = Color.yellow;
                }
                else
                {
                    fpsText.color = Color.red;
                }

            }
#endif

            if (!hasDeterminedFrameFormat)
            {
                if (VideoSink.IsNewFrameAvailable())
                {
                    if (VideoSink.AcquireNextAvailableFrame(out currentFrame))
                    {
                        hasDeterminedFrameFormat = true;

                        if (currentFrame.Format == MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer)
                        {
                            nativeBufferRenderer = new MLWebRTC.VideoSink.Renderer(VideoSink);
                            // TODO : we're only doing this once, need to do this every time
                            // a new frame is acquired to be sure that our render target is of the
                            // right size.
                            CreateNativeBufferRenderTarget(currentFrame.NativeFrame.Width, currentFrame.NativeFrame.Height);

                            nativeRenderer.enabled = true;
                            yuvRenderer.enabled = false;
                            rgbRenderer.enabled = false;

                            // We release the frame immediately so that from next frame onwards
                            // the ycbcrrenderer can drive the acquisition and release.
                            // TODO : change so that we dont waste the first frame here.
                            VideoSink.ReleaseFrame();
                        }
                        else
                        {
                            hasDeterminedFrameFormat = false;
                            nativeRenderer.enabled = false;
                            RenderWebRTCFrame(currentFrame);
                        }
                    }
                }
            }
            else
            {
                if (nativeBufferRenderer != null)
                {
                    nativeBufferRenderer.Render();
                }
                else
                {
                    // TODO : do the frame acquisition & release cycle here and
                    // call the render func of the appropriate format.
                    if (!loggedOnce)
                    {
                        Debug.LogError("Frame of unsupported type.");
                    }
                }
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            fpsTimer.Reset();
#endif
        }

        public void ToggleVisible(bool showRenderer)
        {
            yuvRenderer.enabled = showRenderer;
            rgbRenderer.enabled = showRenderer;
            nativeRenderer.enabled = showRenderer;
        }

        private void RenderWebRTCFrame(MLWebRTC.VideoSink.Frame frame)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (frame.Format != MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer)
            {
                if (frame.ImagePlanes == null)
                {
                    return;
                }
            }

            // TODO : also impl for VideoSink.Renderer.
            float fps = 1.0f / fpsTimer.Elapsed();
            
            if (fpsText != null)
            {
                fpsText.text = string.Format("{0:0.00}", fps);
                if (fps >= 24f)
                {
                    fpsText.color = Color.green;
                }
                else if (fps >= 16f)
                {
                    fpsText.color = Color.yellow;
                }
                else
                {
                    fpsText.color = Color.red;
                }

            }

            if (frame.Format != MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer)
            {
                if (frame.ImagePlanes.Length >= 1)
                {
                    float aspectRatio = frame.ImagePlanes[0].Width / (float)frame.ImagePlanes[0].Height;
                    float scaleWidth = transform.lossyScale.y * aspectRatio;

                    // sets the plane to the aspect ratio of the frame
                    if (transform.lossyScale.x != scaleWidth)
                    {
                        Transform parent = transform.parent;
                        transform.parent = null;
                        transform.localScale = new Vector3(scaleWidth, transform.localScale.y, transform.localScale.z);
                        transform.parent = parent;
                    }
                }
                else
                {
                    Debug.Log($"{gameObject.name}: frame.ImagePlanes is empty! ");
                }
            }

            switch (frame.Format)
            {
                case MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888:
                    {
                        nativeRenderer.enabled = false;
                        rgbRenderer.enabled = false;
                        yuvRenderer.enabled = true;
                        RenderWebRTCFrameYUV(frame);
                        break;
                    }

                case MLWebRTC.VideoSink.Frame.OutputFormat.RGBA_8888:
                    {
                        nativeRenderer.enabled = false;
                        yuvRenderer.enabled = false;
                        rgbRenderer.enabled = true;
                        RenderWebRTCFrameRGB(frame);
                        break;
                    }
            }

            fpsTimer.Reset();
#endif
        }

        private void RenderWebRTCFrameYUV(MLWebRTC.VideoSink.Frame frame)
        {
            UpdateYUVTextureChannel(ref rawVideoTexturesYUV[0], frame.ImagePlanes[0], yuvRenderer, samplerNamesYUV[0], yChannelBuffer);
            UpdateYUVTextureChannel(ref rawVideoTexturesYUV[1], frame.ImagePlanes[1], yuvRenderer, samplerNamesYUV[1], uChannelBuffer);
            UpdateYUVTextureChannel(ref rawVideoTexturesYUV[2], frame.ImagePlanes[2], yuvRenderer, samplerNamesYUV[2], vChannelBuffer);
        }

        private void UpdateYUVTextureChannel(ref Texture2D channelTexture, MLWebRTC.VideoSink.Frame.PlaneInfo planeInfo,
                                             Renderer renderer, string samplerName, byte[] newTextureChannel)
        {
            byte[] planeData = new byte[planeInfo.Stride * planeInfo.Height * planeInfo.BytesPerPixel];
            Marshal.Copy(planeInfo.DataPtr, planeData, 0, planeData.Length);
            if(planeData == null)
            {
                return;
            }

            if (channelTexture != null && (channelTexture.width != planeInfo.Width || channelTexture.height != planeInfo.Height))
            {
                Destroy(channelTexture);
                channelTexture = null;
            }
            if (channelTexture == null)
            {
                channelTexture = new Texture2D((int)planeInfo.Width, (int)(planeInfo.Height), TextureFormat.Alpha8, false)
                {
                    filterMode = FilterMode.Bilinear
                };

                renderer.material.SetTexture(samplerName, channelTexture);
            }
            
            int pixelStride = (int)(planeInfo.Stride / planeInfo.Width);

            if (pixelStride == 1)
            {
                channelTexture.LoadRawTextureData(planeInfo.DataPtr, (int)planeInfo.Size);
                channelTexture.Apply();
            }
            else
            {
                if (newTextureChannel == null || newTextureChannel.Length != (planeInfo.Width * planeInfo.Height))
                {
                    newTextureChannel = new byte[planeInfo.Width * planeInfo.Height];
                }

                for (int y = 0; y < planeInfo.Height; y++)
                {
                    for (int x = 0; x < planeInfo.Width; x++)
                    {
                        newTextureChannel[y * planeInfo.Width + x] = planeData[y * planeInfo.Stride + x * pixelStride];
                    }
                }
                channelTexture.LoadRawTextureData(newTextureChannel);
                channelTexture.Apply();
            }
        }

        private void RenderWebRTCFrameRGB(MLWebRTC.VideoSink.Frame frame)
        {
            for (int i = 0; i < rawVideoTexturesRGB.Length; ++i)
            {
                MLWebRTC.VideoSink.Frame.PlaneInfo imagePlane = frame.ImagePlanes[i];
                Texture2D rawVideoTextureRGB = rawVideoTexturesRGB[i];

                int width = (int)(imagePlane.Stride / imagePlane.BytesPerPixel);
                if (rawVideoTextureRGB == null || rawVideoTextureRGB.width != width || rawVideoTextureRGB.height != imagePlane.Height)
                {
                    rawVideoTextureRGB = new Texture2D(width, (int)imagePlane.Height, TextureFormat.RGBA32, false);
                    rawVideoTextureRGB.filterMode = FilterMode.Bilinear;
                    rgbRenderer.material.mainTexture = rawVideoTextureRGB;
                    rgbRenderer.material.mainTextureScale = new Vector2(1.0f, -1.0f);
                }

                rawVideoTexturesRGB[i] = rawVideoTextureRGB;
                rawVideoTextureRGB.LoadRawTextureData(imagePlane.DataPtr, (int)imagePlane.Size);
                rawVideoTextureRGB.Apply();
            }
        }

        private void CreateNativeBufferRenderTarget(uint frameWidth, uint frameHeight)
        {
            if (rawVideoTextureNative == null || rawVideoTextureNative.width != frameWidth || rawVideoTextureNative.height != frameHeight)
            {
                rawVideoTextureNative = new RenderTexture((int)frameWidth, (int)frameHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                nativeRenderer.material.mainTexture = rawVideoTextureNative;
                nativeBufferRenderer.SetRenderBuffer(rawVideoTextureNative);

                float aspectRatio = frameWidth / (float)frameHeight;
                float scaleWidth = transform.lossyScale.y * aspectRatio;

                // sets the plane to the aspect ratio of the frame
                if (transform.lossyScale.x != scaleWidth)
                {
                    Transform parent = transform.parent;
                    transform.parent = null;
                    transform.localScale = new Vector3(scaleWidth, transform.localScale.y, transform.localScale.z);
                    transform.parent = parent;
                }
            }
        }

        void OnDestroy()
        {
            if (nativeBufferRenderer != null)
            {
                nativeBufferRenderer.Cleanup();
            }
        }
    }

}
