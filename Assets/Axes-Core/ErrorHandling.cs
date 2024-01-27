using Unity;
using System.Collections.Generic;
using UnityEngine;

namespace AxesCore
{
    public class ErrorHandler
    {
        List<string> logs;
        List<string> errors;

        public void Log(string log)
        {
            logs.Add(log);
            Debug.Log(log);
        }

        public void Error(string error)
        {
            logs.Add(error);
            Debug.LogError(error);
        }
    }
}
