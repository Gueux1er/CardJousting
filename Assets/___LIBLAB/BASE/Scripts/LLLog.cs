using System;
using UnityEngine;

/*public class Debug
{
    private static bool DEBUG = false;
    
    public static void Log(string message)
    {
        if(Application.isEditor || DEBUG)
            UnityEngine.Debug.Log(message);
    }

    public static void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
    
    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }
    
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }
}*/

namespace LibLabSystem
{
    public static class LLLog
    {
        private const string TAG = "LLSystem";

        public static void Log(string tag, string message)
        {
            if(Application.isEditor)
                UnityEngine.Debug.Log(Format(tag, message));
        }

        public static void LogE(string tag, string message)
        {
            if(Application.isEditor)
                UnityEngine.Debug.LogError(Format(tag, message));
        }

        public static void LogW(string tag, string message)
        {
            if(Application.isEditor)
                UnityEngine.Debug.LogWarning(Format(tag, message));
        }

        private static string Format(string tag, string message)
        {
            return string.Format("{0} - {1} [{2}] - <b>{3}</b>", DateTime.Now, TAG, tag, message);
        }
    }
}