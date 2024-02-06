using Unity;
using System.Collections.Generic;
using UnityEngine;

namespace AxesCore
{
    public class ErrorHandler
    {
        public static List<string> logs;
        public static List<string> errors;

        public static void Init()
        {
            logs = new();
            errors = new();
        }

        public static void Log(string log)
        {
            logs.Add(log);
            Debug.Log(log);
        }

        public static void Error(string error)
        {
            logs.Add(error);
            Debug.LogError(error);
        }
    }
}
