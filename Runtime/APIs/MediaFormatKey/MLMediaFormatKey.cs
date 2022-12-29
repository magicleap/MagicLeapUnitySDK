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
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMediaFormat description goes here.
    /// </summary>
    public partial class MLMediaFormatKey
    {
        public static string AAC_Encoded_Target_Level => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_Encoded_Target_Level());

        public static string AAC_DRC_Boost_Factor => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_DRC_Boost_Factor());

        public static string AAC_DRC_Attenuation_Factor => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_DRC_Attenuation_Factor());

        public static string AAC_DRC_Heavy_Compression => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_DRC_Heavy_Compression());

        public static string AAC_DRC_Target_Reference_Level => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_DRC_Target_Reference_Level());

        public static string AAC_Max_Output_Channel_Count => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_Max_Output_Channel_Count());

        public static string AAC_Profile => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_Profile());

        public static string AAC_SBR_Mode => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_SBR_Mode());

        public static string Bit_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Bit_Rate());

        public static string Bit_Rate_Mode => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Bit_Rate_Mode());

        public static string Capture_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Capture_Rate());

        public static string Channel_Count => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Channel_Count());

        public static string Channel_Mask => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Channel_Mask());

        public static string Color_Format => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Color_Format());

        public static string Duration => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Duration());

        public static string FLAC_Compression_Level => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_FLAC_Compression_Level());

        public static string Frame_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Frame_Rate());

        public static string Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Height());

        public static string I_Frame_Interval => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_I_Frame_Interval());

        public static string Intra_Refresh_Period => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Intra_Refresh_Period());

        public static string Is_ADTS => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_ADTS());

        public static string Is_Autoselect => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_Autoselect());

        public static string Is_Default => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_Default());

        public static string Is_Forced_Subtitle => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_Forced_Subtitle());

        public static string Language => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Language());

        public static string Max_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Height());

        public static string Max_Input_Size => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Input_Size());

        public static string Max_Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Width());

        public static string Mime => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Mime());

        public static string PCM_Encoding => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_PCM_Encoding());

        public static string Push_Blank_Buffers_On_Stop => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Push_Blank_Buffers_On_Stop());

        public static string Repeat_Previous_Frame_After => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Repeat_Previous_Frame_After());

        public static string Sample_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Sample_Rate());

        public static string Stride => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Stride());

        public static string Temporal_Layering => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Temporal_Layering());

        public static string Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Width());

        public static string Crop_Left => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crop_Left());

        public static string Crop_Right => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crop_Right());

        public static string Crop_Bottom => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crop_Bottom());

        public static string Crop_Top => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crop_Top());

        public static string Operating_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Operating_Rate());

        public static string Latency => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Latency());

        public static string Parameter_Video_Bitrate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Video_Bitrate());

        public static string Parameter_Request_Sync_Frame => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Request_Sync_Frame());

        public static string Parameter_Set_Suspend => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Set_Suspend());

        public static string Parameter_Set_Suspend_Time => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Set_Suspend_Time());

        public static string Parameter_Drop_Before => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Drop_Before());

        public static string Parameter_Drop_After => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Drop_After());

        public static string Parameter_Offset_Time => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Parameter_Offset_Time());

        public static string Priority => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Priority());

        public static string Max_B_Frames => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_B_Frames());

        public static string Intra_Refresh_Mode => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Intra_Refresh_Mode());

        public static string Intra_Refresh_CIR_Num => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Intra_Refresh_CIR_Num());

        public static string Intra_Refresh_AIR_Num => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Intra_Refresh_AIR_Num());

        public static string Intra_Refresh_AIR_Ref => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Intra_Refresh_AIR_Ref());

        public static string Profile => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Profile());

        public static string Level => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Level());

        public static string Prepend_Header_To_Sync_Frames => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Prepend_Header_To_Sync_Frames());

        public static string Color_Range => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Color_Range());

        public static string Color_Standard => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Color_Standard());

        public static string Color_Transfer => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Color_Transfer());

        public static string HDR_Static_Info => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_HDR_Static_Info());

        public static string CSD => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD());

        public static string CSD0 => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD0());

        public static string CSD1 => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD1());

        public static string CSD2 => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD2());

        public static string CSD_Avc => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD_Avc());

        public static string CSD_Hevc => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_CSD_Hevc());

        public static string Album => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Album());

        public static string Albumart => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Albumart());

        public static string Albumartist => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Albumartist());

        public static string Artist => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Artist());

        public static string Audio_Presentation_Info => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Audio_Presentation_Info());

        public static string Audio_Presentation_Presentation_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Audio_Presentation_Presentation_Id());

        public static string Audio_Presentation_Program_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Audio_Presentation_Program_Id());

        public static string Audio_Session_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Audio_Session_Id());

        public static string Author => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Author());

        public static string Bits_Per_Sample => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Bits_Per_Sample());

        public static string Cdtracknumber => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Cdtracknumber());

        public static string Compilation => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Compilation());

        public static string Complexity => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Complexity());

        public static string Composer => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Composer());

        public static string Create_Input_Surface_Suspended => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Create_Input_Surface_Suspended());

        public static string Crypto_Default_Iv_Size => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Default_Iv_Size());

        public static string Crypto_Encrypted_Byte_Block => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Encrypted_Byte_Block());

        public static string Crypto_Encrypted_Sizes => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Encrypted_Sizes());

        public static string Crypto_Iv => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Iv());

        public static string Crypto_Key => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Key());

        public static string Crypto_Mode => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Mode());

        public static string Crypto_Plain_Sizes => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Plain_Sizes());

        public static string Crypto_Skip_Byte_Block => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Crypto_Skip_Byte_Block());

        public static string D263 => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_D263());

        public static string Date => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Date());

        public static string Discnumber => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Discnumber());

        public static string Display_Crop => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Display_Crop());

        public static string Display_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Display_Height());

        public static string Display_Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Display_Width());

        public static string Encoder_Delay => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Encoder_Delay());

        public static string Encoder_Padding => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Encoder_Padding());

        public static string Esds => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Esds());

        public static string Exif_Offset => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Exif_Offset());

        public static string Exif_Size => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Exif_Size());

        public static string Frame_Count => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Frame_Count());

        public static string Genre => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Genre());

        public static string Grid_Columns => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Grid_Columns());

        public static string Grid_Rows => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Grid_Rows());

        public static string Haptic_Channel_Count => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Haptic_Channel_Count());

        public static string Hdr10_Plus_Info => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Hdr10_Plus_Info());

        public static string Icc_Profile => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Icc_Profile());

        public static string Is_Sync_Frame => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_Sync_Frame());

        public static string Location => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Location());

        public static string Loop => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Loop());

        public static string Lyricist => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Lyricist());

        public static string Manufacturer => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Manufacturer());

        public static string Max_Bit_Rate => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Bit_Rate());

        public static string Max_Fps_To_Encoder => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Fps_To_Encoder());

        public static string Max_Pts_Gap_To_Encoder => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Max_Pts_Gap_To_Encoder());

        public static string Mpeg_User_Data => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Mpeg_User_Data());

        public static string Mpeg2_Stream_Header => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Mpeg2_Stream_Header());

        public static string PCM_Big_Endian => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_PCM_Big_Endian());

        public static string Pssh => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Pssh());

        public static string Rotation => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Rotation());

        public static string Sar_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Sar_Height());

        public static string Sar_Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Sar_Width());

        public static string Sei => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Sei());

        public static string Slice_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Slice_Height());

        public static string Target_Time => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Target_Time());

        public static string Temporal_Layer_Count => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Temporal_Layer_Count());

        public static string Temporal_Layer_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Temporal_Layer_Id());

        public static string Text_Format_Data => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Text_Format_Data());

        public static string Thumbnail_CSD_Hevc => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Thumbnail_CSD_Hevc());

        public static string Thumbnail_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Thumbnail_Height());

        public static string Thumbnail_Time => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Thumbnail_Time());

        public static string Thumbnail_Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Thumbnail_Width());

        public static string Tile_Height => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Tile_Height());

        public static string Tile_Width => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Tile_Width());

        public static string Time_Us => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Time_Us());

        public static string Title => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Title());

        public static string Track_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Track_Id());

        public static string Track_Index => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Track_Index());

        public static string Valid_Samples => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Valid_Samples());

        public static string Year => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Year());

        public static string AAC_DRC_Effect_Type => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_AAC_DRC_Effect_Type());

        public static string Quality => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Quality());

        public static string Output_Reorder_Depth => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Output_Reorder_Depth());

        public static string Is_Timed_Text => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Is_Timed_Text());

        public static string Ca_System_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Ca_System_Id());

        public static string Ca_Session_Id => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Ca_Session_Id());

        public static string Ca_Private_Data => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Ca_Private_Data());

        public static string Feature_ => Marshal.PtrToStringAnsi(NativeBindings.MLMediaFormatGetKey_Feature_());

    }
}
