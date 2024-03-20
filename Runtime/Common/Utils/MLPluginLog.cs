// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Diagnostics;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Used to print logs within the plugin.
    /// </summary>
    public static class MLPluginLog
    {
        /// <summary>
        /// Different verbosity levels of which logs to print.
        /// Modify this to change the verbosity of the Magic Leap APIs.
        /// </summary>
        public enum VerbosityLevel : uint
        {
            /// <summary>
            /// Don't print any MLPlugin logs.
            /// </summary>
            Silent,

            /// <summary>
            /// Print only MLPlugin error logs.
            /// </summary>
            ErrorsOnly,

            /// <summary>
            /// Print MLPlugin error and warning logs.
            /// </summary>
            WarningsAndErrors,

            /// <summary>
            /// Print all MLPlugin logs.
            /// </summary>
            Verbose,
        }

        internal abstract class ScopedLog : IDisposable
        {
            private string m_ScopeName;
            private bool m_ShowStackTrace;

            protected string scopeName => m_ScopeName;

            protected ScopedLog(string scopeName, bool showStackTrace = false)
            {
                m_ScopeName = scopeName;
                Log("Enter");
            }

            public void Dispose()
            {
                Log("Exit");
            }

            protected virtual string FormatLogMessage(string message) => $"[{scopeName}]: {message}";

            protected virtual void LogInternal(string message, LogType logType = LogType.Log)
            {
                UnityEngine.Debug.LogFormat(logType, m_ShowStackTrace ? LogOption.None : LogOption.NoStacktrace, null, FormatLogMessage(message));
            }

            [Conditional("DEVELOPMENT_BUILD")]
            public void Log(string message, LogType logType = LogType.Log)
                => LogInternal(message, logType);
        }

        /// <summary>
        /// Gets or sets current level of logs to print.
        /// </summary>
        public static VerbosityLevel Level { get; set; } = VerbosityLevel.ErrorsOnly;

        /// <summary>
        /// Prints the given log message.
        /// </summary>
        /// <param name="logMsg">The message to print.</param>
        public static void Debug(object logMsg)
        {
            if (MLPluginLog.Level == MLPluginLog.VerbosityLevel.Verbose)
            {
                UnityEngine.Debug.Log(logMsg);
            }
        }

        /// <summary>
        /// Prints the given log message with formatting.
        /// </summary>
        /// <param name="logMsg">The formatted message to print.</param>
        /// <param name="args">The arguments to pass the formatted log message</param>
        public static void DebugFormat(string logMsg, params object[] args)
        {
            if (MLPluginLog.Level == MLPluginLog.VerbosityLevel.Verbose)
            {
                UnityEngine.Debug.LogFormat(logMsg, args);
            }
        }

        /// <summary>
        /// Prints the given log message as a warning.
        /// </summary>
        /// <param name="logMsg">The warning to print.</param>
        public static void Warning(object logMsg)
        {
            if (MLPluginLog.Level == MLPluginLog.VerbosityLevel.Verbose || MLPluginLog.Level == MLPluginLog.VerbosityLevel.WarningsAndErrors)
            {
                UnityEngine.Debug.LogWarning("Warning: " + logMsg);
            }
        }

        /// <summary>
        /// Prints the given log message as a warning with formatting.
        /// </summary>
        /// <param name="logMsg">The formatted warning message to print.</param>
        /// <param name="args">The arguments to pass the formatted message</param>
        public static void WarningFormat(string logMsg, params object[] args)
        {
            if (MLPluginLog.Level == MLPluginLog.VerbosityLevel.Verbose || MLPluginLog.Level == MLPluginLog.VerbosityLevel.WarningsAndErrors)
            {
                UnityEngine.Debug.LogWarningFormat("Warning: " + logMsg, args);
            }
        }

        /// <summary>
        /// Prints the given log message as an error.
        /// </summary>
        /// <param name="logMsg">The error to print.</param>
        public static void Error(object logMsg)
        {
            if (MLPluginLog.Level != MLPluginLog.VerbosityLevel.Silent)
            {
                UnityEngine.Debug.LogError("Error: " + logMsg);
            }
        }

        /// <summary>
        /// Prints the given log message as an error with formatting.
        /// </summary>
        /// <param name="logMsg">The formatted error message to print.</param>
        /// <param name="args">The arguments to pass the formatted message</param>
        public static void ErrorFormat(string logMsg, params object[] args)
        {
            if (MLPluginLog.Level != MLPluginLog.VerbosityLevel.Silent)
            {
                UnityEngine.Debug.LogErrorFormat("Error: " + logMsg, args);
            }
        }
    }
}
