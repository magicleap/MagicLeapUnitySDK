using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityEditor.XR.MagicLeap
{
    class MacOSDependencyChecker
    {
        const string kRegexPattern = @"\t(.+) \(compatibility version \d{1,4}\.\d{1,4}\.\d{1,4}, current version \d{1,4}\.\d{1,4}\.\d{1,4}\)";

        public class DependencyMap
        {
            public string file = null;
            public List<string> dependencies = new List<string>();
        }

        internal static IEnumerable<string> LaunchOtool(string filepath)
        {
            var psi = new ProcessStartInfo {
                FileName = "/usr/bin/otool",
                Arguments = string.Format("-L \"{0}\"", filepath),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process p = Process.Start(psi))
            {
                var output = p.StandardOutput.ReadToEnd();
                var error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                return output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
        }

        internal static DependencyMap GetDependencies(string file)
        {
            var regex = new Regex(kRegexPattern);
            var dm = new DependencyMap { file = file };
            var output = LaunchOtool(file);
            foreach (var line in output)
            {
                var m = regex.Match(line);
                if (m.Success)
                {
                    var dep_path = m.Groups[1].Value;
                    foreach (var prefix in new string[] { "@loader_path", "@rpath"} )
                        dep_path = dep_path.Replace(prefix, Path.GetDirectoryName(file));
                    dm.dependencies.Add(dep_path);
                }
            }
            return dm;
        }

        internal static void Migrate(string src, string dest)
        {
            var psi = new ProcessStartInfo {
                FileName = "lipo",
                Arguments = string.Format("-create \"{0}\" -output \"{1}\"", src, dest),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (Process p = Process.Start(psi))
                p.WaitForExit();

            psi = new ProcessStartInfo {
                FileName = "install_name_tool",
                Arguments = string.Format("-id \"{0}\" \"{0}\"", dest),
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (Process p = Process.Start(psi))
                p.WaitForExit();
        }

        internal static void MigrateWithDependencies(string src, string dest)
        {
            var original_deps = GetDependencies(src);
            Migrate(src, dest);
            var new_deps = GetDependencies(dest);
            var missing = new List<string>();
            foreach (var dep in new_deps.dependencies)
            {
                string depRelPath = Path.Combine(Path.GetDirectoryName(dest), dep);
                if (!File.Exists(depRelPath))
                    missing.Add(dep);
            }
            foreach (var item in missing)
            {
                var dep_path = Path.GetFullPath(item);
                if (!File.Exists(dep_path))
                {
                    var dep_file = Path.GetFileName(item);
                    foreach (var old_dep in original_deps.dependencies)
                    {
                        if (Path.GetFileName(old_dep) == dep_file)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(dep_path));
                            if (File.Exists(old_dep))
                                File.Copy(old_dep, dep_path);
                        }
                    }
                }
            }
        }
    }
}
