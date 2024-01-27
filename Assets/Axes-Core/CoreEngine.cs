using System.Collections.Generic;

namespace AxesCore
{
    public class CoreEngine
    {
        public static GMode group0;
        public static GMode group1;
        public static GMode group2;
        public static GMode group3;
        public static GMode group4;
        public static GMode group6;
        public static GMode group7;
        public static GMode group8;
        public static GMode group10;
        public static GMode group11;
        public static GMode group14;
        public static GMode group16;

        static List<GMode> activeModes;

        internal static void SetGMode(GMode mode)
        {
            //Set the group mode
            if (mode == GMode.G00 || mode == GMode.G01 || mode == GMode.G02 || mode == GMode.G03 || mode == GMode.G12 || mode == GMode.G13)
            {
                group1 = mode;
            }
        }

        internal static void SetMMode(MMode mode)
        {

        }

        public static void SetFeedRate(float f)
        {
            Core.feedRate = f;
            Core.mode = CoreMode.done;
        }

        public static void SetRPM(float r)
        {
            Core.rpm = r;
            Core.mode = CoreMode.done;
        }
        public static void SetUPM(float u)
        {
            Core.upm = u;
            Core.mode = CoreMode.done;
        }
        public static void SetSpindleSpeed(float s)
        {
            Core.spindleSpeed = s;
            Core.mode = CoreMode.done;
        }

        public static void SetCoord()
        {

            Core.mode = CoreMode.coord;
        }

        public static void SetDwellTime(float d)
        {
            Core.dwellTime = d;
            Core.mode = CoreMode.done;
        }

        public static void AddMode(GMode mode)
        {
            if (activeModes.Contains(mode))
            {
                //The mode is already active, no need to add it
            }
            else if (true) //Check For Cancel Modes
            {

            }
            else //Add the mode
            {
                activeModes.Add(mode);
            }
        }
    }
    public enum CoreMode : int
    {
        pre, waiting, running, done, coord
    }

    public class Core
    {
        public static float upm;
        public static float rpm;
        public static float feedRate = 15;
        public static float dwellTime = 0;
        public static float spindleSpeed;
        public static Dictionary<string, float> coords;
        public static CoreMode mode;

        public static void Init()
        {
            coords = new Dictionary<string, float>();
            coords.Add("x", 0);
            coords.Add("y", 0);
            coords.Add("z", 0);
            coords.Add("a", 0);
            coords.Add("b", 0);
            coords.Add("c", 0);
            mode = CoreMode.pre;
        }
    }
}

