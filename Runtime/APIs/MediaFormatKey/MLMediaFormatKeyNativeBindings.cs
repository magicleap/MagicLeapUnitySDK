// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMediaFormat description goes here.
    /// </summary>
    public partial class MLMediaFormatKey
    {
        /// <summary>
        /// See ml_media_format.h for additional comments.
        /// </summary>
        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Internal DLL used by the API.
            /// </summary>
            private const string MLMediaFormatKeyDll = "ml_sdk_loader";

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_Encoded_Target_Level();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_DRC_Boost_Factor();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_DRC_Attenuation_Factor();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_DRC_Heavy_Compression();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_DRC_Target_Reference_Level();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_Max_Output_Channel_Count();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_Profile();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_SBR_Mode();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Bit_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Bit_Rate_Mode();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Capture_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Channel_Count();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Channel_Mask();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Color_Format();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Duration();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_FLAC_Compression_Level();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Frame_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_I_Frame_Interval();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Intra_Refresh_Period();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_ADTS();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_Autoselect();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_Default();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_Forced_Subtitle();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Language();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Input_Size();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Mime();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_PCM_Encoding();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Push_Blank_Buffers_On_Stop();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Repeat_Previous_Frame_After();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Sample_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Stride();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Temporal_Layering();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crop_Left();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crop_Right();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crop_Bottom();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crop_Top();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Operating_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Latency();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Video_Bitrate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Request_Sync_Frame();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Set_Suspend();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Set_Suspend_Time();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Drop_Before();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Drop_After();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Parameter_Offset_Time();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Priority();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_B_Frames();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Intra_Refresh_Mode();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Intra_Refresh_CIR_Num();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Intra_Refresh_AIR_Num();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Intra_Refresh_AIR_Ref();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Profile();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Level();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Prepend_Header_To_Sync_Frames();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Color_Range();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Color_Standard();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Color_Transfer();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_HDR_Static_Info();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD0();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD1();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD2();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD_Avc();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_CSD_Hevc();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Album();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Albumart();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Albumartist();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Artist();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Audio_Presentation_Info();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Audio_Presentation_Presentation_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Audio_Presentation_Program_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Audio_Session_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Author();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Bits_Per_Sample();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Cdtracknumber();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Compilation();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Complexity();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Composer();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Create_Input_Surface_Suspended();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Default_Iv_Size();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Encrypted_Byte_Block();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Encrypted_Sizes();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Iv();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Key();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Mode();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Plain_Sizes();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Crypto_Skip_Byte_Block();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_D263();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Date();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Discnumber();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Display_Crop();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Display_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Display_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Encoder_Delay();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Encoder_Padding();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Esds();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Exif_Offset();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Exif_Size();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Frame_Count();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Genre();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Grid_Columns();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Grid_Rows();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Haptic_Channel_Count();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Hdr10_Plus_Info();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Icc_Profile();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_Sync_Frame();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Location();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Loop();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Lyricist();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Manufacturer();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Bit_Rate();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Fps_To_Encoder();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Max_Pts_Gap_To_Encoder();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Mpeg_User_Data();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Mpeg2_Stream_Header();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_PCM_Big_Endian();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Pssh();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Rotation();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Sar_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Sar_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Sei();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Slice_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Target_Time();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Temporal_Layer_Count();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Temporal_Layer_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Text_Format_Data();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Thumbnail_CSD_Hevc();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Thumbnail_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Thumbnail_Time();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Thumbnail_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Tile_Height();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Tile_Width();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Time_Us();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Title();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Track_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Track_Index();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Valid_Samples();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Year();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_AAC_DRC_Effect_Type();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Quality();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Output_Reorder_Depth();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Is_Timed_Text();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Ca_System_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Ca_Session_Id();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Ca_Private_Data();

            [DllImport(MLMediaFormatKeyDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLMediaFormatGetKey_Feature_();

        }
    }
}
