using System.Collections.Generic;
using Unity.VisualScripting;

namespace AxesCore
{
    public class CoreEngine
    {
        public static void SetMMode(MMode mode)
        {

        }
        public static void SetUPM(UPM upm) { Core.upm = upm; Core.mode = CoreMode.done; }

        public static void SetFeedRate(float f) { Core.feedRate = f; Core.mode = CoreMode.done; }

        public static void SetSpindleSpeed(float s) { Core.spindleSpeed = s; Core.mode = CoreMode.done; }

        public static void SetDwellTime(float d) { Core.dwellTime = d; Core.mode = CoreMode.done; }

        public static void RapidMove() => Core.group[1] = GMode.G00;
        public static void LinearFeedMove() => Core.group[1] = GMode.G01;
        public static void ClockwiseArcFeedMove() => Core.group[1] = GMode.G02;
        public static void CounterClockwiseArcFeedMove() => Core.group[1] = GMode.G03;
        public static void ClockwiseCircle() => Core.group[1] = GMode.G12;
        public static void CounterClockwiseCircle() => Core.group[1] = GMode.G13;
        
        public static void PositionModeAbsolute()
        {
            Core.group[3] = GMode.G90;
            Core.positionMode = PositionMode.absolute;
        }

        public static void PositionModeIncremental()
        {
            Core.group[3] = GMode.G91;
            Core.positionMode = PositionMode.incremental;
        }

        public static void ArcModeAbsolute()
        {
            Core.group[4] = GMode.G901;
            Core.arcMode = PositionMode.arcAbsolute;
        }

        public static void ArcModeIncremental()
        {
            Core.group[4] = GMode.G911;
            Core.arcMode = PositionMode.arcIncremental;
        }

        public static void XYPlaneSelect() => Core.group[2] = GMode.G17;
        public static void ZXPlaneSelect() => Core.group[2] = GMode.G18;
        public static void YZPlaneSelect() => Core.group[2] = GMode.G19;

        public static void ResetCoord()
        {
            Core.coord = new();
        }
    }

    public enum CoreMode : int
    {
        pre, waiting, running, done, coord, drawStart, drawEnd, drawing
    }

    public class Core
    {
        public static UPM upm;
        public static PositionMode arcMode;
        public static PositionMode positionMode;
        public static float feedRate = 15;
        public static float dwellTime = 0;
        public static bool exactStop = false;
        public static float spindleSpeed;

        public static CoreMode mode;
        public static Coord coord;
        public static GMode[] group;

        public static void Init()
        {
            CoreEngine.ResetCoord();
            mode = CoreMode.pre;
            upm = UPM.millimeters;
            group = new GMode[16]; //Group 0 to Group 16
        }
    }
}

