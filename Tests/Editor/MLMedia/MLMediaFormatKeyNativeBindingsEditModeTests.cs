using System.Reflection;
using NUnit.Framework;

namespace UnitySDKEditorTests
{
    public partial class MLMediaFormatKey : NativeBindingsTests
    {
        [SetUp]
        public void SetupNativeBindings()
        {
            var apiType = typeof(UnityEngine.XR.MagicLeap.MLMediaFormatKey);
            nativeBindings = apiType.GetNestedType("NativeBindings", BindingFlags.NonPublic);
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_Encoded_Target_Level_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_Encoded_Target_Level");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_DRC_Boost_Factor_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_DRC_Boost_Factor");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_DRC_Attenuation_Factor_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_DRC_Attenuation_Factor");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_DRC_Heavy_Compression_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_DRC_Heavy_Compression");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_DRC_Target_Reference_Level_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_DRC_Target_Reference_Level");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_Max_Output_Channel_Count_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_Max_Output_Channel_Count");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_Profile_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_Profile");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_SBR_Mode_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_SBR_Mode");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Bit_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Bit_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Bit_Rate_Mode_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Bit_Rate_Mode");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Capture_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Capture_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Channel_Count_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Channel_Count");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Channel_Mask_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Channel_Mask");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Color_Format_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Color_Format");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Duration_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Duration");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_FLAC_Compression_Level_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_FLAC_Compression_Level");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Frame_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Frame_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_I_Frame_Interval_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_I_Frame_Interval");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Intra_Refresh_Period_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Intra_Refresh_Period");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_ADTS_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_ADTS");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_Autoselect_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_Autoselect");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_Default_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_Default");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_Forced_Subtitle_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_Forced_Subtitle");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Language_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Language");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Input_Size_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Input_Size");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Mime_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Mime");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_PCM_Encoding_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_PCM_Encoding");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Push_Blank_Buffers_On_Stop_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Push_Blank_Buffers_On_Stop");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Repeat_Previous_Frame_After_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Repeat_Previous_Frame_After");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Sample_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Sample_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Stride_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Stride");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Temporal_Layering_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Temporal_Layering");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crop_Left_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crop_Left");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crop_Right_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crop_Right");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crop_Bottom_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crop_Bottom");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crop_Top_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crop_Top");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Operating_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Operating_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Latency_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Latency");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Video_Bitrate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Video_Bitrate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Request_Sync_Frame_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Request_Sync_Frame");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Set_Suspend_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Set_Suspend");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Set_Suspend_Time_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Set_Suspend_Time");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Drop_Before_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Drop_Before");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Drop_After_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Drop_After");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Parameter_Offset_Time_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Parameter_Offset_Time");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Priority_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Priority");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_B_Frames_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_B_Frames");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Intra_Refresh_Mode_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Intra_Refresh_Mode");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Intra_Refresh_CIR_Num_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Intra_Refresh_CIR_Num");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Intra_Refresh_AIR_Num_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Intra_Refresh_AIR_Num");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Intra_Refresh_AIR_Ref_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Intra_Refresh_AIR_Ref");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Profile_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Profile");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Level_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Level");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Prepend_Header_To_Sync_Frames_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Prepend_Header_To_Sync_Frames");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Color_Range_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Color_Range");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Color_Standard_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Color_Standard");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Color_Transfer_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Color_Transfer");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_HDR_Static_Info_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_HDR_Static_Info");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD0_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD0");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD1_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD1");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD2_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD2");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD_Avc_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD_Avc");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_CSD_Hevc_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_CSD_Hevc");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Album_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Album");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Albumart_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Albumart");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Albumartist_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Albumartist");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Artist_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Artist");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Audio_Presentation_Info_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Audio_Presentation_Info");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Audio_Presentation_Presentation_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Audio_Presentation_Presentation_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Audio_Presentation_Program_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Audio_Presentation_Program_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Audio_Session_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Audio_Session_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Author_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Author");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Bits_Per_Sample_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Bits_Per_Sample");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Cdtracknumber_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Cdtracknumber");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Compilation_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Compilation");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Complexity_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Complexity");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Composer_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Composer");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Create_Input_Surface_Suspended_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Create_Input_Surface_Suspended");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Default_Iv_Size_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Default_Iv_Size");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Encrypted_Byte_Block_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Encrypted_Byte_Block");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Encrypted_Sizes_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Encrypted_Sizes");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Iv_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Iv");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Key_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Key");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Mode_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Mode");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Plain_Sizes_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Plain_Sizes");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Crypto_Skip_Byte_Block_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Crypto_Skip_Byte_Block");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_D263_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_D263");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Date_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Date");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Discnumber_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Discnumber");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Display_Crop_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Display_Crop");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Display_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Display_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Display_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Display_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Encoder_Delay_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Encoder_Delay");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Encoder_Padding_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Encoder_Padding");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Esds_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Esds");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Exif_Offset_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Exif_Offset");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Exif_Size_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Exif_Size");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Frame_Count_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Frame_Count");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Genre_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Genre");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Grid_Columns_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Grid_Columns");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Grid_Rows_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Grid_Rows");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Haptic_Channel_Count_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Haptic_Channel_Count");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Hdr10_Plus_Info_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Hdr10_Plus_Info");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Icc_Profile_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Icc_Profile");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_Sync_Frame_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_Sync_Frame");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Location_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Location");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Loop_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Loop");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Lyricist_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Lyricist");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Manufacturer_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Manufacturer");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Bit_Rate_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Bit_Rate");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Fps_To_Encoder_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Fps_To_Encoder");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Max_Pts_Gap_To_Encoder_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Max_Pts_Gap_To_Encoder");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Mpeg_User_Data_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Mpeg_User_Data");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Mpeg2_Stream_Header_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Mpeg2_Stream_Header");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_PCM_Big_Endian_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_PCM_Big_Endian");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Pssh_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Pssh");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Rotation_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Rotation");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Sar_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Sar_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Sar_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Sar_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Sei_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Sei");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Slice_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Slice_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Target_Time_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Target_Time");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Temporal_Layer_Count_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Temporal_Layer_Count");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Temporal_Layer_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Temporal_Layer_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Text_Format_Data_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Text_Format_Data");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Thumbnail_CSD_Hevc_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Thumbnail_CSD_Hevc");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Thumbnail_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Thumbnail_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Thumbnail_Time_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Thumbnail_Time");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Thumbnail_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Thumbnail_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Tile_Height_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Tile_Height");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Tile_Width_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Tile_Width");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Time_Us_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Time_Us");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Title_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Title");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Track_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Track_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Track_Index_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Track_Index");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Valid_Samples_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Valid_Samples");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Year_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Year");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_AAC_DRC_Effect_Type_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_AAC_DRC_Effect_Type");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Quality_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Quality");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Output_Reorder_Depth_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Output_Reorder_Depth");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Is_Timed_Text_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Is_Timed_Text");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Ca_System_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Ca_System_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Ca_Session_Id_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Ca_Session_Id");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Ca_Private_Data_Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Ca_Private_Data");
        }

        [Test]
        public void NativeBinding_MLMediaFormatGetKey_Feature__Exists()
        {
            AssertThatMethodExists("MLMediaFormatGetKey_Feature_");
        }
    }
}