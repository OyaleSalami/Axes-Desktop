using System;
using System.IO;
using UnityEngine;

namespace AxesCore
{
    /// <summary>Handles error and log statements</summary>
    public class ErrorHandler
    {
        /// <summary>Where the log files would be stored</summary>
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Axes Core/";

        /// <summary>Name of the log file</summary>
        private static readonly string filename;
        public static string filePath;
        internal static string current;

        /// <summary>Create new logs to store the error and debug commands!</summary>
        public static void Init()
        {
            //Create Log Folder if it doesn't exist
            Directory.CreateDirectory(path);

            //Create a Log File
            //The filename would be given by the current time
            string filename = DateTime.Now.ToString("MddHHmms") + ".log";
            filePath = Path.Combine(path, filename);

            //Create a log file
            //File.Create(filePath, 0, FileOptions.RandomAccess);
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate,
                                        FileAccess.ReadWrite,
                                        FileShare.ReadWrite);
        }

        /// <summary>Adds a debug statement to the logs</summary>
        public static void Log(string log, bool error = false)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    if (error != true)
                    {
                        Debug.Log(log); sw.WriteLine("[Log]: " + log);
                    }
                    else
                    {
                        Debug.LogError(error); sw.WriteLine("[Error]: " + log);
                    }

                    sw.Close(); sw.Dispose();
                    fs.Close(); fs.Dispose();
                }
            }
            current = log;
        }

        /// <summary>Adds an error statement to the logs</summary>
        public static void Error(string error)
        {
            Log(error, true);
        }
    }
}
