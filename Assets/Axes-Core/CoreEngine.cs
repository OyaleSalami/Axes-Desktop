using System;

namespace AxesCore
{
    public class CoreEngine
    {
        public static void SetMMode(MMode mode)
        {

        }

        #region Tool Functions
        public static void SetTool(Tool tool)
        {
            Core.tool = tool;
        }

        public static void SetToolDiameter(float value)
        {
            Core.toolDiameter = value;
        }

        public static void SetToolHeight(float value)
        {
            Core.toolHeight = value;
        }
        #endregion

        #region Group 0 Functions

        public static void SetDwellMode()
        {
            Core.coordMode = CoordMode.dwell;
            CoreEngine.SetCoordMode(CoordMode.dwell);
        }

        public static void SetDwellTime(float d)
        {
            Core.dwellTime = d;
        }

        public static void SetUPM(UPM upm) { Core.upm = upm; }

        public static void SetFeedRate() { SetFeedRate(15); }
        public static void SetFeedRate(float f) { Core.feedRate = f; }

        public static void SetSpindleSpeed(float s) { Core.spindleSpeed = s; }

        #endregion

        #region Group 1 Functions
        public static void RapidMove()
        {
            Core.group[1] = GMode.G00;
            Core.coordMode = CoordMode.draw;
        }
        public static void LinearFeedMove()
        {
            Core.group[1] = GMode.G01;
            Core.coordMode = CoordMode.draw;
        }
        public static void ClockwiseArcFeedMove()
        {
            Core.group[1] = GMode.G02;
            Core.coordMode = CoordMode.draw;
        }
        public static void CounterClockwiseArcFeedMove()
        {
            Core.group[1] = GMode.G03;
            Core.coordMode = CoordMode.draw;
        }
        public static void ClockwiseCircle()
        {
            Core.group[1] = GMode.G12;
            Core.coordMode = CoordMode.draw;
        }
        public static void CounterClockwiseCircle()
        {
            Core.group[1] = GMode.G13;
            Core.coordMode = CoordMode.draw;
        }
        #endregion

        #region Group 3 & 4 Functions
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
        #endregion

        #region Group 2 Functions
        public static void XYPlaneSelect()
        {
            Core.group[2] = GMode.G17;
            Core.planeMode = PlaneMode.XY;
        }
        public static void ZXPlaneSelect()
        {
            Core.group[2] = GMode.G18;
            Core.planeMode = PlaneMode.ZX;
        }
        public static void YZPlaneSelect()
        {
            Core.group[2] = GMode.G19;
            Core.planeMode = PlaneMode.YZ;
        }
        #endregion

        /// <summary>Resets the coordinates in Core to zeroes</summary>
        public static void ResetCoord()
        {
            Core.coord = new();
        }

        public static void SetCoordMode(CoordMode mode)
        {
            Core.coordMode = mode;
        }

        /// <summary>Change the unit per measure to Inches</summary>
        public static void SetInch()
        {
            Core.group[6] = GMode.G20;
            Core.upm = UPM.inches;
        }

        /// <summary>Change the unit per measure to Millimeters</summary>
        public static void SetMilli()
        {
            Core.group[6] = GMode.G21;
            Core.upm = UPM.millimeters;
        }

        /// <summary>Changes the scale of the simulator</summary>
        public static void SetScale(float _scale)
        {
            Core.scale = _scale;
        }

        /// <summary>Resets the scale of the simulator</summary>
        public static void Cancelscale()
        {
            Core.scale = 1;
        }

        #region Group 12 Functions
        public static void SetFixtureOffset(int offset)
        {
            Core.fixtureOffset = offset;
        }
        public static void SetFixtureOffset1()
        {
            Core.group[12] = GMode.G54;
            SetFixtureOffset(1);
        }
        public static void SetAdditionalFixtureOffset()
        {
            Core.group[12] = GMode.G541;
            CoreEngine.SetCoordMode(CoordMode.addFixtureOffset);
        }
        public static void SetFixtureOffset2()
        {
            Core.group[12] = GMode.G55;
            SetFixtureOffset(2);
        }
        public static void SetFixtureOffset3()
        {
            Core.group[12] = GMode.G56;
            SetFixtureOffset(3);
        }
        public static void SetFixtureOffset4()
        {
            Core.group[12] = GMode.G57;
            SetFixtureOffset(4);
        }
        public static void SetFixtureOffset5()
        {
            Core.group[12] = GMode.G58;
            SetFixtureOffset(5);
        }
        public static void SetFixtureOffset6()
        {
            Core.group[12] = GMode.G59;
            SetFixtureOffset(6);
            CoreEngine.SetCoordMode(CoordMode.fixtureOffset);
        }
        #endregion

        public static void HandleCoordinates()
        {
            try
            {
                if(Core.coord.f != 0)
                {
                    SetFeedRate(Core.coord.f);
                }

                //Handle Fixture Offset
                if (Core.coordMode == CoordMode.fixtureOffset)
                {
                    SetFixtureOffset(Int32.Parse(Core.coord.p.ToString()));
                }
                else if (Core.coordMode == CoordMode.addFixtureOffset)
                {
                    SetFixtureOffset(Int32.Parse((Core.coord.p + 6).ToString()));
                }
                else if (Core.coordMode == CoordMode.dwell)
                {
                    SetDwellTime(Core.coord.p);
                    Core.mode = CoreMode.dwellStart;
                }
                else
                {
                    Core.mode = CoreMode.drawStart;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error handling the coordinates: " + e);
            }
        }
    }

    public class Core
    {
        public static UPM upm;
        public static PositionMode arcMode;
        public static PositionMode positionMode;
        public static PlaneMode planeMode;
        public static Tool tool;
        public static float toolDiameter = 1.0f;
        public static float toolHeight = 1.0f;
        public static int fixtureOffset = 1;
        public static float scale = 1;
        public static float feedRate = 15;
        public static float dwellTime = 0;
        public static bool exactStop = false;
        public static float spindleSpeed;

        public static CoreMode mode;
        public static Coord coord;
        public static CoordMode coordMode;
        public static GMode[] group;

        public static void Init()
        {
            CoreEngine.ResetCoord();
            mode = CoreMode.waiting;
            upm = UPM.millimeters;
            planeMode = PlaneMode.XY;
            positionMode = PositionMode.absolute;
            arcMode = PositionMode.arcAbsolute;
            group = new GMode[16]; //Defines Group 0 to Group 16
            scale = 1f;
        }
    }
}

