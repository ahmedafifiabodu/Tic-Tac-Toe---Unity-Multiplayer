using UnityEngine;

public static class Logging
{
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message) => Debug.Log(message);

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    internal static void LogWarning(object message) => Debug.LogWarning(message);

    [System.Diagnostics.Conditional("ENABLE_LOG")]
    internal static void LogError(object message) => Debug.LogError(message);
}