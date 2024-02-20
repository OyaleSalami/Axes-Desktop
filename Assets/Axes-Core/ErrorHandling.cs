using System.Collections.Generic;
using UnityEngine;

namespace AxesCore
{
    /// <summary>Handles error and log statements</summary>
    public class ErrorHandler
    {
        /// <summary>A list of all the logs collected during the program running</summary>
        public static List<string> logs;

        /// <summary>Create new logs to store the error and debug commands!</summary>
        public static void Init()
        {
            logs = new();
        }

        /// <summary>Adds a debug statement to the logs</summary>
        public static void Log(string log)
        {
            logs.Add("[Log]: " + log);
            Debug.Log(log);
        }

        /// <summary>Adds an error statement to the logs</summary>
        public static void Error(string error)
        {
            logs.Add("[Error]: " + error);
            Debug.LogError(error);
        }
    }
}
