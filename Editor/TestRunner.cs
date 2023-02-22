using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace MagicLeap
{
    public static class TestRunner
    {
        private static TestRunnerApi testRunnerApi;
        private static int connectedDevices = 0;
        private static string adbOutput = "";

        /// <summary>
        /// Execute API unit tests in the Unity Editor.
        /// </summary>
        public static void RunEditModeTests()
        {
            RunTests(TestMode.EditMode);
        }

        /// <summary>
        /// Execute integration and functional tests in the editor's Play Mode. Requires an active ZI session and
        /// for an App Sim Runtime path to be configured.
        /// </summary>
        public static void RunPlayModeTests()
        {
            RunTests(TestMode.PlayMode);
        }

        /// <summary>
        /// Execute integration and functional tests on a connected physical Magic Leap device. 
        /// </summary>
        public static void RunDeviceTests()
        {
            if (IsDeviceConnected())
            {
                RunTests(TestMode.PlayMode, BuildTarget.Android);
            }
            else
            {
                UnityEngine.Debug.LogError("Tests failed: unable to detect any attached adb device.");
                Application.Quit(1);
            }
        }

        private static void RunTests(TestMode mode, BuildTarget? buildTarget = null)
        {
            testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
            var filter = new Filter()
            {
                testMode = mode,
                targetPlatform = buildTarget
            };
            testRunnerApi.RegisterCallbacks(new Callbacks());
            testRunnerApi.Execute(new ExecutionSettings(filter));
        }

        private static bool IsDeviceConnected()
        {
            var androidSdkPath = AndroidExternalToolsSettings.sdkRootPath;
            if(string.IsNullOrEmpty(androidSdkPath))
            {
                androidSdkPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
                if(string.IsNullOrEmpty(androidSdkPath))
                {
                    UnityEngine.Debug.LogError("ANDROID_SDK_ROOT not configured, unable to install player for device tests.");
                    return false;
                }
            }

            var toolsPath = Path.Combine(androidSdkPath, "platform-tools");
            var adb = Path.Combine(toolsPath, "adb");
#if UNITY_EDITOR_WIN
            adb += ".exe";
#endif

            ProcessStartInfo psi = new()
            {
                FileName = adb,
                Arguments = "devices -l",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process p = new()
            {
                StartInfo = psi
            };

            p.OutputDataReceived += ProcessOutputReceived;
            p.EnableRaisingEvents = true;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            UnityEngine.Debug.Log(adbOutput);

            return (connectedDevices > 0);
        }

        private static void ProcessOutputReceived(object sender, DataReceivedEventArgs e)
        {
            adbOutput += e.Data.ToString() + "\n";
            if (e.Data.Contains("device") && !e.Data.Contains("List"))
            {
                connectedDevices++;
            }
        }

        private class Callbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                UnityEngine.Debug.Log($"Starting run of tests \"{testsToRun.FullName}\"");
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                string output = $"Test runner took {result.Duration}s and finished with state {result.TestStatus.ToString().ToUpper()}.";
                if (result.TestStatus.ToString() == "Passed")
                {
                    UnityEngine.Debug.Log(output);
                }
                else
                {
                    UnityEngine.Debug.LogError(output);
                }

                testRunnerApi.UnregisterCallbacks(this);

                string reportPath = "";
                var args = new List<String>(Environment.GetCommandLineArgs());
                bool nunit = args.Contains("-nunit");
                for (int i = 0; i < args.Count; i++)
                {
                    if (args[i] == "-testResults")
                    {
                        reportPath = args[i + 1];
                        break;
                    }
                }

                // not batchmode means this was called from an editor menu item, so always generate a results file
                if(!Application.isBatchMode)
                {
                    reportPath = Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "testResults.xml");
                }

                if (reportPath != "")
                {
                    if (!nunit)
                    {
                        Reporter.ReportJUnitXML(reportPath, result);
                    }
                    else
                    {
                        Reporter.ReportNUnitXML(reportPath, result);
                    }
                }

                if (Application.isBatchMode)
                {
                    if (result.ResultState != "Passed" && !File.Exists(reportPath))
                    {
                        EditorApplication.Exit(1);
                    }
                    else
                    {
                        // Exit with 0 even on failure if a report XML was generated so Jenkins can handle it
                        EditorApplication.Exit(0);
                    }
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
                if (!test.HasChildren)
                {
                    UnityEngine.Debug.Log($"Starting test {test.Name}");
                }
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (!result.HasChildren)
                {
                    string output = $"Test {result.Name} took {result.Duration}s and finished with result {result.TestStatus.ToString().ToUpper()}";
                    if (result.TestStatus.ToString() == "Passed")
                    {
                        UnityEngine.Debug.Log(output);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError(output);
                    }
                }
            }
        }

        private static class Reporter
        {
            public static void ReportNUnitXML(string path, ITestResultAdaptor result)
            {
                var xml = result.ToXml();
                var writer = XmlWriter.Create(path);
                xml.WriteTo(writer);
                writer.Flush();
            }

            public static void ReportJUnitXML(string path, ITestResultAdaptor result)
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlNode rootNode = xmlDoc.CreateElement("testsuites");
                XmlAttribute rootAttrTests = xmlDoc.CreateAttribute("tests");
                rootAttrTests.Value = result.PassCount.ToString();
                rootNode.Attributes.Append(rootAttrTests);
                XmlAttribute rootAttrSkipped = xmlDoc.CreateAttribute("disabled");
                rootAttrSkipped.Value = result.SkipCount.ToString();
                rootNode.Attributes.Append(rootAttrSkipped);
                XmlAttribute rootAttrFailed = xmlDoc.CreateAttribute("failures");
                rootAttrFailed.Value = result.FailCount.ToString();
                rootNode.Attributes.Append(rootAttrFailed);
                XmlAttribute rootAttrName = xmlDoc.CreateAttribute("name");
                rootAttrName.Value = result.Name;
                rootNode.Attributes.Append(rootAttrName);
                XmlAttribute rootAttrTime = xmlDoc.CreateAttribute("time");
                rootAttrTime.Value = result.Duration.ToString();
                rootNode.Attributes.Append(rootAttrTime);
                xmlDoc.AppendChild(rootNode);

                foreach (ITestResultAdaptor testSuite in result.Children.First().Children)
                {
                    XmlNode testSuiteNode = GenerateJUnitXmlNode(xmlDoc, rootNode, testSuite);
                    rootNode.AppendChild(testSuiteNode);
                }

                xmlDoc.Save(path);

                UnityEngine.Debug.Log("saved results to " + path);
            }

            private static XmlNode GenerateJUnitXmlNode(XmlDocument xmlDoc, XmlNode parent, ITestResultAdaptor result)
            {
                // leaf node: test case (method marked with [Test])
                if (result.Children.Count() == 0)
                {
                    XmlNode testCaseNode = xmlDoc.CreateElement("testcase");
                    XmlAttribute testAttrName = xmlDoc.CreateAttribute("name");
                    testAttrName.Value = result.Name;
                    testCaseNode.Attributes.Append(testAttrName);
                    XmlAttribute testAttrAssertions = xmlDoc.CreateAttribute("assertions");
                    testAttrAssertions.Value = result.AssertCount.ToString();
                    testCaseNode.Attributes.Append(testAttrAssertions);
                    XmlAttribute testAttrTime = xmlDoc.CreateAttribute("time");
                    testAttrTime.Value = result.Duration.ToString();
                    testCaseNode.Attributes.Append(testAttrTime);
                    XmlAttribute testAttrStatus = xmlDoc.CreateAttribute("status");
                    testAttrStatus.Value = result.TestStatus.ToString();
                    testCaseNode.Attributes.Append(testAttrStatus);

                    return testCaseNode;
                }
                else // namespace levels and classes that contain [Test] methods are groupings of tests called testsuites (the foldouts in the UI)
                {
                    XmlNode testSuiteNode = xmlDoc.CreateElement("testsuite");
                    XmlAttribute testSuiteAttrName = xmlDoc.CreateAttribute("name");
                    testSuiteAttrName.Value = result.Name;
                    testSuiteNode.Attributes.Append(testSuiteAttrName);
                    XmlAttribute testSuiteAttrTests = xmlDoc.CreateAttribute("tests");
                    testSuiteAttrTests.Value = result.Test.TestCaseCount.ToString();
                    testSuiteNode.Attributes.Append(testSuiteAttrTests);
                    XmlAttribute testSuiteAttrSkipped = xmlDoc.CreateAttribute("disabled");
                    testSuiteAttrSkipped.Value = result.SkipCount.ToString();
                    testSuiteNode.Attributes.Append(testSuiteAttrSkipped);
                    XmlAttribute testSuiteAttrFailed = xmlDoc.CreateAttribute("failures");
                    testSuiteAttrFailed.Value = result.FailCount.ToString();
                    testSuiteNode.Attributes.Append(testSuiteAttrFailed);
                    XmlAttribute testSuiteAttrTime = xmlDoc.CreateAttribute("time");
                    testSuiteAttrTime.Value = result.Duration.ToString();
                    testSuiteNode.Attributes.Append(testSuiteAttrTime);
                    parent.AppendChild(testSuiteNode);

                    // recurse through the result graph generated by Unity and manually create the XML structure.
                    // this way we don't need to worry about how we structure test classes/namespaces, each test gets a distinct
                    // node that can be read and displayed by an interpreter like Jenkins
                    foreach (var child in result.Children)
                    {
                        var node = GenerateJUnitXmlNode(xmlDoc, testSuiteNode, child);
                        testSuiteNode.AppendChild(node);
                    }

                    return testSuiteNode;
                }
            }
        }
    }
}
