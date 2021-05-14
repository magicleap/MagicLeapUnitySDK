using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// Sets up Visual Studio Project Files for this package to show the same
    /// folder structure in the IDE as in the file explorer.
    /// Without this class, all files in this package show up under the same
    /// root folder when its added as a local package to a Unity project.
    /// What's worse is that if the package contains files of the same name
    /// in 2 different paths, Visual Studio only shows one of those files
    /// and updating one ends up deleting the other.
    /// </summary>
    public class VisualStudioProjectFileHook : AssetPostprocessor
    {
        /// <summary>
        /// Necessary for XLinq to save the xml project file in utf8
        /// </summary>
        class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public static string OnGeneratedCSProject(string path, string content)
        {
            // Only change for project files starting with "LuminUnity"
            // i.e. "LuminUnity.csproj" & "LuminUnity-Editor.csproj"
            if (!Path.GetFileName(path).StartsWith("LuminUnity"))
            {
                return content;
            }

            // Parse the document and make some changes
            XDocument document = XDocument.Parse(content);
            AdjustDocument(document);

            // Save the changes using the Utf8StringWriter
            Utf8StringWriter str = new Utf8StringWriter();
            document.Save(str);

            return str.ToString();
        }

        static void AdjustDocument(XDocument document)
        {
            // Get namespace of document
            XNamespace ns = document.Root.Name.Namespace;

            string[] elementsToConsider = { "Compile", "None" };

            foreach (string element in elementsToConsider)
            {
                // Get all elements
                IEnumerable<XElement> compileElements = document.Root.Descendants(ns + element);

                // Regex to find which part of Include attribute of Compile element to use for Link element value.
                Regex regex = new Regex(@"\\(Packages)\\.*\.(cs|asmdef|asmref|shader)$");

                // Add child Link element to each Compile element
                foreach (XElement el in compileElements)
                {
                    string fileName = el.Attribute("Include").Value;
                    Match match = regex.Match(fileName);
                    if (match.Success)
                    {
                        // Substr from 1 to exclude initial slash character
                        XElement link = new XElement(ns + "Link")
                        {
                            Value = match.Value.Substring(1)
                        };

                        el.Add(link);
                    }
                }
            }
        }
    }
}
