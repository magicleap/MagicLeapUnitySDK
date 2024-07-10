#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapFeature
    {
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            base.GetValidationChecks(rules, targetGroup);

#if !UNITY_2023_1_OR_NEWER
            getDefaultTextureCompressionFormat ??= TryGetPlayerSettingsMethod("GetDefaultTextureCompressionFormat");
            isTextureCompressionAPIOk = TryGetDXTCEnum();
            isTextureCompressionAPIOk &= IsDefaultTextureCompressionAPIValid(getDefaultTextureCompressionFormat);
#endif

            if (targetGroup == BuildTargetGroup.Android)
            {
                foreach (var rule in MagicLeapProjectRules)
                {
                    rules.Add(rule);
                }
            }
            else if (targetGroup == BuildTargetGroup.Standalone)
            {
                foreach(var rule in MagicLeapProjectRulesEditor)
                {
                    rules.Add(rule);
                }
            }
        }

        private ValidationRule[] MagicLeapProjectRules => new ValidationRule[]
        {
            // must build for x86-64
            new ValidationRule(this)
            {
                message = "Target architectures must include x86-64",
                checkPredicate = () => PlayerSettings.Android.targetArchitectures.HasFlag(AndroidArchitecture.X86_64),
                fixIt = () =>
                {
                    var arch = PlayerSettings.Android.targetArchitectures | AndroidArchitecture.X86_64;
                    PlayerSettings.Android.buildApkPerCpuArchitecture = (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.None);
                    PlayerSettings.Android.targetArchitectures = arch;
                },
                fixItMessage = "Set PlayerSettings Target Architecture to contain x86-64",
                error = true,
                errorEnteringPlaymode = false
            },
            // require Vulkan
            new ValidationRule(this)
            {
                message = "Vulkan must be specified as the default Graphics API.",
                checkPredicate = () =>
                {
                    var currentApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    return !PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android) &&
                                        currentApis.Length > 0 &&
                                        currentApis[0] == GraphicsDeviceType.Vulkan;
                },
                fixIt = () =>
                {
                    var currentApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);

                    var required = new GraphicsDeviceType[] { GraphicsDeviceType.Vulkan };
                    var graphicAPIs = required.Union(currentApis);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, graphicAPIs.ToArray());
                },
                fixItMessage = "Set PlayerSettings 'Graphic Apis' to only contain Vulkan on Android target",
                error = true,
                errorEnteringPlaymode = false
            },
            // require DXT compression
            new ValidationRule(this)
            {
                message = "Only DXT texture compression is supported.",
#if !UNITY_2023_1_OR_NEWER
                checkPredicate = () => IsDefaultTextureCompressionFormatDxtForTarget(BuildTargetGroup.Android),
                // For now we must let the user fix the setting manually, there is no simple way to update
                fixIt = () => SettingsService.OpenProjectSettings("Project/Player"),
                fixItMessage = "Open Player Settings and manually update Texture compression format.",
#else
                checkPredicate = () =>
                {
                    var first = PlayerSettings.Android.textureCompressionFormats[0];
                    return first == TextureCompressionFormat.DXTC || first == TextureCompressionFormat.DXTC_RGTC;
                },
                fixIt = () =>
                {
                    var dxtc = new TextureCompressionFormat[] { TextureCompressionFormat.DXTC };
                    var formats = dxtc.Union(PlayerSettings.Android.textureCompressionFormats).ToArray();
                    PlayerSettings.Android.textureCompressionFormats = formats;
                },
                fixItMessage = "Set DXTC as default texture compression formats",
#endif
                error = true,
                errorEnteringPlaymode = true,
            },
            // set target devices to 'any'
            new ValidationRule(this)
            {
                message = "Must target all Android Devices.",
                checkPredicate = () => PlayerSettings.Android.androidTargetDevices == AndroidTargetDevices.AllDevices,
                fixIt = () => PlayerSettings.Android.androidTargetDevices = AndroidTargetDevices.AllDevices,
                fixItMessage = "Set Target Devices to \"All Devices\".",
                error = true
            }
        };

        private ValidationRule[] MagicLeapProjectRulesEditor => new ValidationRule[]
        {
            // require Vulkan
            new ValidationRule(this)
            {
                message = "Vulkan must be specified as the default Graphics API.",
                checkPredicate = () =>
                {
                    var currentApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
                    return !PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows) &&
                                        currentApis.Length > 0 &&
                                        currentApis[0] == GraphicsDeviceType.Vulkan;
                },
                fixIt = () =>
                {
                    var currentApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows, false);

                    var required = new GraphicsDeviceType[] { GraphicsDeviceType.Vulkan };
                    var graphicAPIs = required.Union(currentApis);
                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, graphicAPIs.ToArray());
                },
                fixItMessage = "Set PlayerSettings 'Graphic Apis' to only contain Vulkan on Android target",
                error = true,
                errorEnteringPlaymode = false
            },
            // Multi-pass rendering
            new ValidationRule(this)
            {
                message = "Multi-pass rendering is required in order for Play Mode to render both eyes.",
                checkPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    return settings.renderMode == OpenXRSettings.RenderMode.MultiPass;
                },
                fixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    settings.renderMode = OpenXRSettings.RenderMode.MultiPass;
                },
                fixItMessage = "Set Render Mode to Multi-pass.",
                error = true,
                errorEnteringPlaymode = true
            },
            // Use system default runtime
            new ValidationRule(this)
            {
                message = "The Application Simulator needs to use your system's default OpenXR runtime.",
                checkPredicate = () => IsPlaymodeRuntimeSetToSystemDefault(),
                fixIt = () => SetPlaymodeRuntimeToSystemDefault(),
                fixItMessage = "Set Play Mode OpenXR Runtime to \"System Default\"",
                error = true,
                errorEnteringPlaymode = false
            }
        };

        private static bool IsPlaymodeRuntimeSetToSystemDefault()
        {
            if (Application.isPlaying)
            {
                return true;
            }
            try
            {
                var selectorClass = Type.GetType("UnityEditor.XR.OpenXR.OpenXRRuntimeSelector,Unity.XR.OpenXR.Editor.dll");
                if (selectorClass != null && selectorClass.IsClass)
                {
                    var selectedIndexField = selectorClass.GetField("selectedRuntimeIndex", BindingFlags.NonPublic | BindingFlags.Static);
                    if (selectedIndexField != null && selectedIndexField.FieldType == typeof(int))
                    {
                        int selectedIndex = (int)selectedIndexField.GetValue(null);
                        return selectedIndex == 0;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SetPlaymodeRuntimeToSystemDefault()
        {
            if (Application.isPlaying)
                return;

            var selectorClass = Type.GetType("UnityEditor.XR.OpenXR.OpenXRRuntimeSelector,Unity.XR.OpenXR.Editor.dll");
            if (selectorClass != null && selectorClass.IsClass)
            {
                var selectedIndexField = selectorClass.GetField("selectedRuntimeIndex", BindingFlags.NonPublic | BindingFlags.Static);
                if (selectedIndexField != null && selectedIndexField.FieldType == typeof(int))
                {
                    selectedIndexField.SetValue(null, 0);
                    Environment.SetEnvironmentVariable("XR_SELECTED_RUNTIME_JSON", "");
                }
            }
        }

        #region Texture Compression Format reflection
        // Before Unity 2023.1 there is no public API in PlayerSettings for getting or setting "Texture compression format" so we need to use reflection
        // unnecessary in 2023.1: https://docs.unity3d.com/2023.1/Documentation/ScriptReference/PlayerSettings.Android-textureCompressionFormats.html
#if !UNITY_2023_1_OR_NEWER
        private static bool isTextureCompressionAPIOk;
        private static MethodInfo getDefaultTextureCompressionFormat;
        private static int dxtcEnumValue;
        private static int dxtcRGTCEnumValue;
        
        private static bool TryGetDXTCEnum()
        {
            dxtcEnumValue = -1;
            dxtcRGTCEnumValue = -1;
            try
            {
                var textureCompressionFormatEnum = Type.GetType("UnityEditor.TextureCompressionFormat,UnityEditor.dll");
                if (textureCompressionFormatEnum != null && textureCompressionFormatEnum.IsEnum)
                {
                    string[] enumNames = textureCompressionFormatEnum.GetEnumNames();
                    Array enumValues = textureCompressionFormatEnum.GetEnumValues();
                    for (int i = 0; i < enumValues.Length; ++i)
                    {
                        if (enumNames[i] == "DXTC")
                            dxtcEnumValue = Convert.ToInt32(enumValues.GetValue(i));
                        if (enumNames[i] == "DXTC_RGTC")
                            dxtcRGTCEnumValue = Convert.ToInt32(enumValues.GetValue(i));
                    }
                }
                return dxtcEnumValue != -1 && dxtcRGTCEnumValue != -1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static MethodInfo TryGetPlayerSettingsMethod(string methodName)
        {
            MethodInfo playerSettingsMethod;
            try
            {
                var playerSettingsType = Type.GetType("UnityEditor.PlayerSettings,UnityEditor.dll");
                playerSettingsMethod = playerSettingsType?.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            }
            catch (Exception)
            {
                return null;
            }

            return playerSettingsMethod;
        }

        private static bool ValidateEnumParameter(ParameterInfo param, string enumName, string parameterName)
        {
            return param.Name == parameterName && param.ParameterType.Name == enumName && param.ParameterType.IsEnum;
        }

        private static bool IsDefaultTextureCompressionAPIValid(MethodInfo s_GetDefaultTextureCompressionFormat)
        {
            if (s_GetDefaultTextureCompressionFormat == null || s_GetDefaultTextureCompressionFormat.MemberType != MemberTypes.Method)
                return false;
            var getterReturnType = s_GetDefaultTextureCompressionFormat.ReturnType;
            if (!getterReturnType.IsEnum || getterReturnType.Name != "TextureCompressionFormat")
                return false;
            var getterParameters = s_GetDefaultTextureCompressionFormat.GetParameters();
            if (getterParameters.Length != 1
                || !ValidateEnumParameter(getterParameters[0], "BuildTargetGroup", "platform"))
                return false;

            return dxtcEnumValue != -1 || dxtcRGTCEnumValue != -1;
        }

        private static bool IsDefaultTextureCompressionFormatDxtForTarget(BuildTargetGroup buildTargetGroup)
        {
            if (!isTextureCompressionAPIOk || getDefaultTextureCompressionFormat == null)
                return true;

            object enabledStateResult = getDefaultTextureCompressionFormat.Invoke(null, new object[] { buildTargetGroup });
            var textureCompression = Convert.ToInt32(enabledStateResult);
            return textureCompression == dxtcEnumValue || textureCompression == dxtcRGTCEnumValue;
        }
#endif
#endregion
    }
}
#endif
