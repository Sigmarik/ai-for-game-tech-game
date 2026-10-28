using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ExperimentLogger
{
    private static string logFilePath;
    private static readonly object fileLock = new object();
    private static bool initialized = false;

    public static string LogFilePath => logFilePath;

    private static void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        string directory = Path.Combine(Application.persistentDataPath, "ExperimentLogs");
        Directory.CreateDirectory(directory);

        string fileName = $"session_{DateTime.Now:yyyyMMdd_HHmmss_fff}.log";
        logFilePath = Path.Combine(directory, fileName);

        File.WriteAllText(logFilePath,
            $"# Experiment session started at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n" +
            $"# Unity {Application.unityVersion} | {SystemInfo.operatingSystem}\n");

        Debug.Log($"[ExperimentLogger] Logging to {logFilePath}");
    }

    public static void Log(string tag, string message)
    {
        EnsureInitialized();

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string line = $"[{timestamp}] [{tag}] {message}";

        lock (fileLock)
        {
            try
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[ExperimentLogger] Failed to write log: {exception.Message}");
            }
        }

        Debug.Log(line);
    }

    public static void LogEvent(string tag, string message, params string[] keyValuePairs)
    {
        StringBuilder builder = new StringBuilder(message);
        for (int pairIndex = 0; pairIndex + 1 < keyValuePairs.Length; pairIndex += 2)
        {
            builder.Append(' ');
            builder.Append(keyValuePairs[pairIndex]);
            builder.Append("=\"");
            builder.Append(EscapeValue(keyValuePairs[pairIndex + 1]));
            builder.Append('"');
        }
        Log(tag, builder.ToString());
    }

    private static string EscapeValue(string value)
    {
        if (value == null) return "";
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}