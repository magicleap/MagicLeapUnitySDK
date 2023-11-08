// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    //Disabling WebRTC deprecated warning 
    #pragma warning disable 618
    
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

        [SerializeField]
        private bool autoResizeNativeRenderer = false;

        // Only relevant for YUV / RGB frames
        private MLWebRTC.VideoSink.Frame currentFrame;

        private Timer fpsTimer;

        private Texture2D[] rawVideoTexturesYUV = new Texture2D[MLWebRTC.VideoSink.Frame.NativeImagePlanesLength[MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888]];
        private Texture2D[] rawVideoTexturesRGB = new Texture2D[MLWebRTC.VideoSink.Frame.NativeImagePlanesLength[MLWebRTC.VideoSink.Frame.OutputFormat.RGBA_8888]];
        private RenderTexture rawVideoTextureNative;
        private MLWebRTC.VideoSink.Renderer nativeBufferRenderer;

        private byte[] yChannelBuffer;
        private byte[] uChannelBuffer;
        private byte[] vChannelBuffer;

        private static readonly string[] samplerNamesYUV = new string[] { "_MainTex", "_UTex", "_VTex" };

        public MLWebRTC.VideoSink VideoSink { get; set; }

        void Awake()
        {
            VideoSink = MLWebRTC.VideoSink.Create(out MLResult result);
            if (autoResizeNativeRenderer)
            {
                VideoSink.OnFrameResolutionChanged += OnFrameResolutionChanged;
            }
            VideoSink.OnStreamChanged += OnVideoSinkStreamChanged;
            fpsTimer = new Timer(1000);
        }

        private void OnFrameResolutionChanged(uint newWidth, uint newHeight)
        {
            CreateNativeBufferRenderTarget(newWidth, newHeight);
        }

        private void OnVideoSinkStreamChanged(MLWebRTC.MediaStream stream)
        {
            if (nativeBufferRenderer != null)
            {
                nativeBufferRenderer.Cleanup();
            }

            if (rawVideoTextureNative != null)
            {
                rawVideoTextureNative.Release();
            }

            nativeBufferRenderer = null;
            rawVideoTextureNative = null;

            if (stream == null)
            {
                nativeRenderer.enabled = false;
                yuvRenderer.enabled = false;
                rgbRenderer.enabled = false;
            }
        }

        private void Update()
        {
            if (VideoSink.Stream == null)
            {
                nativeRenderer.enabled = false;
                yuvRenderer.enabled = false;
                rgbRenderer.enabled = false;
                return;
            }

            StartFrameTimer();

            // Only check for and acquire frame from VideoSink here if we have not yet determined
            // the format for the stream to be NativeSurface - in which case nativeBufferRenderer 
            // will have been initialized
            if (nativeBufferRenderer == null)
            {
                if (VideoSink.IsNewFrameAvailable())
                {
                    if (VideoSink.AcquireNextAvailableFrame(out currentFrame))
                    {
                        if (currentFrame.Format == MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer)
                        {
                            // We release the frame immediately so that the
                            // YcbcrRenderer can drive the acquisition and release.
                            VideoSink.ReleaseFrame();
                            nativeBufferRenderer = new MLWebRTC.VideoSink.Renderer(VideoSink);
                            CreateNativeBufferRenderTarget(currentFrame.NativeFrame.Width, currentFrame.NativeFrame.Height);
                        }
                        else
                        {
                            RenderWebRTCFrame(currentFrame);
                        }
                    }
                }
            }

            if (nativeBufferRenderer != null && currentFrame.Format == MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer)
            {
                nativeRenderer.enabled = true;
                yuvRenderer.enabled = false;
                rgbRenderer.enabled = false;
                nativeBufferRenderer.Render();
            }

            ResetFrameTimer();
        }

        public void ToggleVisible(bool showRenderer)
        {
            yuvRenderer.enabled = showRenderer;
            rgbRenderer.enabled = showRenderer;
            nativeRenderer.enabled = showRenderer;
        }

        private void RenderWebRTCFrame(MLWebRTC.VideoSink.Frame frame)
        {
            if (frame.Format == MLWebRTC.VideoSink.Frame.OutputFormat.NativeBuffer || frame.ImagePlanes == null)
            {
                // this method should only be used for CPU buffers (YUV or RGB format)
                return;
            }

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

                if (frame.Format == MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888)
                {
                    nativeRenderer.enabled = false;
                    rgbRenderer.enabled = false;
                    yuvRenderer.enabled = true;
                    RenderWebRTCFrameYUV(frame);
                }
                else // RGBA_8888
                {
                    nativeRenderer.enabled = false;
                    yuvRenderer.enabled = false;
                    rgbRenderer.enabled = true;
                    RenderWebRTCFrameRGB(frame);
                }
            }
            else
            {
                Debug.Log($"{gameObject.name}: frame.ImagePlanes is empty! ");
            }
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
            if (planeData == null)
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
            if (frameWidth == 0 || frameHeight == 0)
            {
                return;
            }

            if (rawVideoTextureNative == null || rawVideoTextureNative.width != frameWidth || rawVideoTextureNative.height != frameHeight)
            {
                rawVideoTextureNative = new RenderTexture((int)frameWidth, (int)frameHeight, 0, RenderTextureFormat.ARGB32);
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

        private void StartFrameTimer()
        {
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
        }

        private void ResetFrameTimer()
        {
            fpsTimer.Reset();
        }
    }

}
