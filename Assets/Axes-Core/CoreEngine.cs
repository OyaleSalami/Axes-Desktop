using System;
using System.Collections.Generic;
using UnityEngine;

namespace AxesCore
{
    public class CoreEngine
    {
        public static void SetMMode(MMode mode)
        {
            ErrorHandler.Log("MCode: " + mode);
        }

        #region Tool Functions
        public static void SetTool(Tool tool)
        {
            Core.tool = tool;
            ErrorHandler.Log("Tool: " + tool);
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
            Core.coordMode = CoordMode.nil;
            Core.coordList = new();
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
        public static void SetScale()
        {
            Core.scale = new Vector3(Core.coord.x, Core.coord.z , Core.coord.y);
        }

        /// <summary>Resets the scale of the simulator</summary>
        public static void Cancelscale()
        {
            Core.scale = new(1, 1, 1);
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

        #region Group 7 Functions
        public static void CancelCutterCompensation()
        {
            Core.cutterCompensationMode = CutterCompensationMode.none;
        }

        public static void SetCutterCompensationLeft()
        {
            Core.cutterCompensationMode = CutterCompensationMode.left;
            Core.coordMode = CoordMode.cutterCompensation;
        }

        public static void SetCutterCompensationRight()
        {
            Core.cutterCompensationMode = CutterCompensationMode.right;
            Core.coordMode = CoordMode.cutterCompensation;
        }
        #endregion

        #region Group 8 Functions
        public static void ToolLengthOffsetCancel()
        {
            Core.toolLengthOffset = 0f;
        }

        public static void SetToolLengthOffsetNegative()
        {
            Core.coordMode = CoordMode.toolLengthOffsetNegative;
        }

        public static void SetToolLengthOffsetPositive()
        {
            Core.coordMode = CoordMode.toolLengthOffsetPositive;
        }
        #endregion

        #region Group 16 Functions
        public static void HighSpeedPeck()
        {
            Core.coordMode = CoordMode.highSpeedPeck;
        }

        public static void LHTapping()
        {
            Core.coordMode = CoordMode.lhTapping;
        }

        public static void FineBoring()
        {
            Core.coordMode = CoordMode.fineBoring;
        }

        public static void CancelCannedCycle()
        {
            Core.cannedCycle = false;
        }
        #endregion

        public static void HandleCoordinates()
        {
            ErrorHandler.Log("Handling Coordinates");
            try
            {
                if(Core.coord.f != 0)
                {
                    SetFeedRate(Core.coord.f);
                }
                if(Core.coord.h != 0)
                {
                    SetToolHeight(Core.coord.h);
                }

                //Handle Different Use Cases Of Parameters
                ErrorHandler.Log("Passed Coord Mode: " + Core.coordMode.ToString());
                switch (Core.coordMode)
                {
                    case CoordMode.fixtureOffset:
                        SetFixtureOffset(Int32.Parse(Core.coord.p.ToString()));
                        break;
                    case CoordMode.addFixtureOffset:
                        SetFixtureOffset(Int32.Parse((Core.coord.p + 6).ToString()));
                        break;
                    case CoordMode.dwell:
                        ErrorHandler.Log("Coord Mode: dwell");
                        SetDwellTime(Core.coord.p);
                        Core.mode = CoreMode.dwellStart;
                        break;
                    case CoordMode.highSpeedPeck:
                        Core.mode = CoreMode.startPeck;
                        break;
                    case CoordMode.scale:
                        ErrorHandler.Log("Coord Mode: scale");
                        SetScale();
                        break;
                    case CoordMode.draw:
                        Core.mode = CoreMode.drawStart;
                        break;
                    default:
                        Core.mode = Core.coord.isZero() ? CoreMode.normal : CoreMode.drawStart;
                        break;
                }
                ErrorHandler.Log("Core Mode: " + Core.mode);
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error handling the coordinates: " + e);
            }
        }
    }

    /// <summary>Holds the importatnt parameters needed to run the simulator</summary>
    public class Core
    {
        /// <summary>The Unit the simulator is working with</summary>
        public static UPM upm;

        /// <summary>The current selected tool</summary>
        public static Tool tool;

        /// <summary>The plane being used</summary>
        public static PlaneMode planeMode;

        /// <summary>Arc mode: absolute or incremental</summary>
        public static PositionMode arcMode;

        /// <summary>Position mode: absolute or incremental</summary>
        public static PositionMode positionMode;

        public static CutterCompensationMode cutterCompensationMode;

        /// <summary>The sclae that should be applied to the different axes</summary>
        public static Vector3 scale;

        public static int fixtureOffset = 1;
        public static float dwellTime = 0;
        public static float feedRate = 15;
        public static float spindleSpeed = 0;
        public static float toolHeight = 1.0f;
        public static float toolDiameter = 1.0f;
        public static float toolLengthOffset = 0f;

        public static bool exactStop = false;
        public static bool cannedCycle = false;

        public static CoreMode mode;
        public static Coord coord;

        /// <summary>Determines what the coordinates should be used for</summary>
        public static CoordMode coordMode;
        public static GMode[] group;

        public static List<string> coordList;

        /// <summary>Sets/Resets the parameters for the Core</summary>
        public static void Init()
        {
            feedRate = PlayerPrefs.GetInt("feedrate");
            coord = new Coord();
            mode = CoreMode.normal; //The default state of the simulator
            upm = UPM.millimeters;
            planeMode = PlaneMode.XY;
            positionMode = PositionMode.absolute;
            arcMode = PositionMode.arcAbsolute;

            group = new GMode[16]; //Create the Group 0 to Group 16
            scale = new(1f, 1f, 1f); //Set the default scale
        }
    }
}

