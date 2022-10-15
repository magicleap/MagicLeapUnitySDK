// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;
using static UnityEditor.XR.MagicLeap.Permission;

namespace UnityEditor.XR.MagicLeap
{
    internal class PermissionsSettingsProviderPresenter : SettingsProvider
    {
        internal const string missingMLSDKPathText = "Unity Editor is missing Magic Leap SDK. To fix that go to: " +
            "'Preferences...' > 'External tools' > 'Magic Leap' and set the Magic Leap SDK path. " +
            "Then click the 'Synchronize' button below to update the permissions list " +
            "against the Magic Leap C SDK specified in the editor preferences.";

        private const string PathToUss =
            @"Packages\com.magicleap.unitysdk\Editor\SettingsProviders\Permissions\SettingsProviderStyle.uss";

        private const string instructionsText = "Use this page to add or remove permissions to your AndroidManifest.xml template located " +
            "in Assets/Plugins/Android. Change the Minimum API Level dropdown to filter the list of permissions by their minimum API level.";

        private const string missingManifestText = "AndroidManifest.xml not found. If you want to add or remove permissions from this page, " +
            "you should first create a template AndroidManifest.xml under Assets/Plugins/Android. Go to Player Settings -> Android -> Publishing " +
            "Settings and enable \"Custom Main Manifest\" to automatically generate the template.";

        private readonly string MinimumApiLevelEditorPrefKey = "MagicLeap.Permissions.MinimumAPILevelDropdownValue_" + PlayerSettings.applicationIdentifier;

        private VisualElement root;
        private VisualElement scrollView;
        private DropdownField ApiLevelDropdown;

        private readonly string settingName;
        private readonly Dictionary<ProtectionLevel, Foldout> foldoutGroups = new();
        private readonly Dictionary<string, PermissionSettingsProperty> renderedSettingProperties = new();

        private List<Permission> permissions = new List<Permission>();
        private PermissionGroup[] permissionGroups;
        private string savedMinimumApiLevelChoice = null;
        private List<int> apiLevelDropdownChoices = new List<int>();
        private int minimumApiLevelChosen = 1;
        private readonly static IReadOnlyCollection<string> mandatoryPermissions = new HashSet<string> { };

        AndroidManifestXml androidManifest;

        internal PermissionsSettingsProviderPresenter(
            string path,
            string settingName) : base(path,
            SettingsScope.Project)
        {
            this.settingName = settingName;
            if (File.Exists(AndroidManifestXml.AssetPath))
            {
                androidManifest = new AndroidManifestXml(AndroidManifestXml.AssetPath);
                foreach (var mandatoryPermission in mandatoryPermissions)
                {
                    androidManifest.AddPermission(mandatoryPermission);
                }
                androidManifest.Save();
            }
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            rootElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(PathToUss));
            root = rootElement;
            if (MagicLeapSDKUtil.SdkAvailable)
            {
                PermissionSettingsLoader permissionSettingsLoader = new PermissionSettingsLoader();

                PermissionJson[] jsonEntries = permissionSettingsLoader.settingJson.Settings;

                permissions = PermissionParser.ParsePermissionsFromArray(jsonEntries);

                savedMinimumApiLevelChoice = EditorPrefs.GetString(MinimumApiLevelEditorPrefKey, "API Level 20");

                var groups = new Dictionary<string, List<Permission>>();
                foreach (var permission in permissions)
                {
                    var name = permission.Level.ToString();
                    if (!groups.ContainsKey(name))
                    {
                        groups[name] = new List<Permission>();
                    }
                    groups[name].Add(permission);
                }

                var groupList = new List<PermissionGroup>();
                foreach (var group in groups)
                {
                    var permissionGroup = new PermissionGroup();
                    permissionGroup.Name = group.Key;
                    permissionGroup.Permissions = group.Value.ToArray();
                    groupList.Add(permissionGroup);
                }
                permissionGroups = groupList.ToArray();
            }

            RenderView();
        }

        #region Render functions

        private void SynchronizeView()
        {
            foldoutGroups.Clear();
            renderedSettingProperties.Clear();
            RenderView();
        }

        private void RenderView()
        {
            renderedSettingProperties.Clear();
            root.Clear();

            scrollView = new ScrollView() { mode = ScrollViewMode.Vertical };
            root.Add(scrollView);

            RenderLabel();

            RenderInstructions();

            if (MagicLeapSDKUtil.SdkAvailable)
            {
                RenderFields();
            }
            else
            {
                RenderSdkInfoBox();
            }
        }

        private void RenderLabel()
        {
            Label label = new(settingName);
            label.AddToClassList("settings-label");

            scrollView.Add(label);
        }

        private void RenderInstructions()
        {
            HelpBox instructions = new HelpBox();
            if (androidManifest != null)
            {
                instructions.text = instructionsText;
                instructions.messageType = HelpBoxMessageType.Info;
            }
            else
            {
                instructions.text = missingManifestText;
                instructions.messageType = HelpBoxMessageType.Warning;
            }
            instructions.AddToClassList("settings-help-box");
            root.Add(instructions);
        }

        private void RenderSdkInfoBox()
        {
            HelpBox hlpbx = new HelpBox(missingMLSDKPathText, HelpBoxMessageType.Error);
            hlpbx.AddToClassList("settings-help-box");

            root.Add(hlpbx);
        }

        private void RenderFields()
        {
            renderedSettingProperties.Clear();
            foldoutGroups.Clear();
            RenderApiLevel();
            foreach (var group in permissionGroups)
            {
                RenderGroup(group);
            }
        }

        private void RenderApiLevel()
        {
            ApiLevelDropdown = new DropdownField("Minimum API Level");

            int minimumApiLevel = permissions.Min(p => p.MinimumApiLevel);
            int maximumApiLevel = permissions.Max(p => p.MinimumApiLevel);

            for (int i = minimumApiLevel; i <= maximumApiLevel; ++i)
            {
                apiLevelDropdownChoices.Add(i);
                ApiLevelDropdown.choices.Add($"API Level {i}");
            }
            var currentChoice = savedMinimumApiLevelChoice;
            int selectedIndex = ApiLevelDropdown.choices.IndexOf(currentChoice);
            minimumApiLevelChosen = (selectedIndex >= 0) ? apiLevelDropdownChoices[selectedIndex] : minimumApiLevel;
            ApiLevelDropdown.SetValueWithoutNotify(currentChoice);
            ApiLevelDropdown.RegisterValueChangedCallback(OnApiLevelChange);
            scrollView.Add(ApiLevelDropdown);
        }

        private void OnApiLevelChange(ChangeEvent<string> evt)
        {
            savedMinimumApiLevelChoice = evt.newValue;
            minimumApiLevelChosen = apiLevelDropdownChoices[ApiLevelDropdown.choices.IndexOf(savedMinimumApiLevelChoice)];
            EditorPrefs.SetString(MinimumApiLevelEditorPrefKey, savedMinimumApiLevelChoice);
            RenderView();
        }

        private void RenderGroup(PermissionGroup permissionGroup)
        {
            foreach (var permission in permissionGroup.permissions)
            {
                if (minimumApiLevelChosen >= permission.MinimumApiLevel)
                {
                    RenderPermission(permission);
                }
            }
        }

        private void RenderPermission(Permission permission)
        {
            var level = permission.Level;
            var isMandatory = mandatoryPermissions.Contains(permission.Name);
            var property = new PermissionSettingsProperty(permission, isMandatory);
            if (!foldoutGroups.TryGetValue(level, out _))
            {
                CreateFoldout(level);
            }
            foldoutGroups.TryGetValue(level, out Foldout foldout);
            property.RenderProperty(foldout);

            if (androidManifest != null)
            {
                List<string> manifestPermissions = new List<string>(androidManifest.GetIncludedPermissions());
                property.Toggle.SetValueWithoutNotify(manifestPermissions.Contains(permission.Name));
                property.Toggle.RegisterValueChangedCallback(OnPermissionToggled);
            }
            if (!renderedSettingProperties.TryGetValue(permission.Name, out _))
            {
                renderedSettingProperties.Add(permission.Name, property);
            }
        }

        private void OnPermissionToggled(ChangeEvent<bool> evt)
        {
            AssetDatabase.Refresh();
            if (File.Exists(AndroidManifestXml.AssetPath))
            {
                androidManifest = new AndroidManifestXml(AndroidManifestXml.AssetPath);
            }
            else
            {
                androidManifest = null;
            }
            RenderView();
        }

        private void CreateFoldout(ProtectionLevel category)
        {
            var foldout = new Foldout();
            foldout.AddToClassList("settings-foldout");
            foldout.text = category.ToString();
            foldout.tooltip = LevelDescriptions[category];
            scrollView.Add(foldout);
            foldoutGroups.Add(category, foldout);
        }
        #endregion
    }
}
