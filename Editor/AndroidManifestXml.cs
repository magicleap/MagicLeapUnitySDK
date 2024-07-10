// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2022-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEditor;

namespace MagicLeap
{
    internal class AndroidManifestXml : XmlDocument
    {
        public XmlElement ManifestElement { get; private set; }

        public static string AssetPath = "Assets/Plugins/Android/AndroidManifest.xml";

        public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        protected XmlNamespaceManager nsMgr;
        private string path;
        XmlNodeList permissionNodes;

        public AndroidManifestXml(string path)
        {
            this.path = path;
            using (var reader = new XmlTextReader(this.path))
            {
                reader.Read();
                base.Load(reader);
            }

            ManifestElement = SelectSingleNode("/manifest") as XmlElement;

            nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);

            permissionNodes = SelectNodes("manifest/uses-permission", nsMgr);
        }

        public string Save()
        {
            return SaveAs(path);
        }

        public string SaveAs(string path)
        {
            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                Save(writer);
            }

            return path;
        }

        public string[] GetIncludedPermissions()
        {
            if(permissionNodes != null)
            {
                List<string> permissions = new List<string>();
                for(int i = 0; i < permissionNodes.Count; i++)
                {
                    var node = permissionNodes[i];
                    string name = node.Attributes["android:name"].Value;
                    permissions.Add(name);
                }
                return permissions.ToArray();
            }
            return new string[0];
        }

        public void AddPermission(string permissionName)
        {
            if(Array.IndexOf(GetIncludedPermissions(), permissionName) >= 0)
            {
                // permission already exists
                return;
            }
            XmlNode metadataTag = ManifestElement.AppendChild(CreateElement("uses-permission"));
            metadataTag.Attributes.Append(CreateAndroidAttribute("name", $"{permissionName}")); 
        }

        public void RemovePermission(string permissionName)
        {
            if (Array.IndexOf(GetIncludedPermissions(), permissionName) == -1)
            {
                // permission doesn't exists
                return;
            }
            List<XmlNode> matchingNodes = new List<XmlNode>();
            for (int i = 0; i < permissionNodes.Count; i++)
            {
                var node = permissionNodes[i];
                if (node.Attributes["android:name"].Value == permissionName)
                {
                    matchingNodes.Add(node);
                }
            }
            // remove all matching in case of duplicates
            foreach (var node in matchingNodes)
            {
                ManifestElement.RemoveChild(node);
            }
        }

        public void UpdateOrCreateAttribute(XmlElement xmlParentElement, string tag, string key, string value)
        {
            // Get all child nodes that match the tag and see if value already exists
            var xmlNodeList = xmlParentElement.SelectNodes(tag);
            foreach (XmlNode node in xmlNodeList)
            {
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (attrib.Value == value)
                    {
                        return;
                    }
                }
            }

            XmlElement newElement = CreateElement(tag);
            newElement.SetAttribute(key, AndroidXmlNamespace, value);
            xmlParentElement.AppendChild(newElement);
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }
    }
}
