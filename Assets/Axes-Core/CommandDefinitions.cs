using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AxesCore
{
    public class CommandDefinitions
    {
        public static Dictionary<string, GMode> gModes;
        public static Dictionary<string, MMode> mModes;
        public static Dictionary<string, Tool> tools;

        public delegate void OperationHandler();

        /// <summary>A dictionary to store all function handlers matching them to a key(type)</summary>
        public static Dictionary<GMode, OperationHandler> opHandlers;

        /// <summary>Initializes the dictionaries that hold the commands</summary>
        public static void Init()
        {
            gModes = new Dictionary<string, GMode>();
            mModes = new Dictionary<string, MMode>();
            tools = new Dictionary<string, Tool>();

            //Initialize G-Modes
            gModes.Add("G0", GMode.G00); gModes.Add("g0", GMode.G00);    //Rapid Move
            gModes.Add("G00", GMode.G00); gModes.Add("g00", GMode.G00);   //Rapid Move
            gModes.Add("G1", GMode.G01); gModes.Add("g1", GMode.G01);    //Linear Feed Move
            gModes.Add("G01", GMode.G01); gModes.Add("g01", GMode.G01);   //Linear Feed Move
            gModes.Add("G2", GMode.G02); gModes.Add("g2", GMode.G02);    //Clockwise Arc Move
            gModes.Add("G02", GMode.G02); gModes.Add("g02", GMode.G02);   //Clockwise Arc Move
            gModes.Add("G3", GMode.G03); gModes.Add("g3", GMode.G03);    //Counter-Clockwise Arc
            gModes.Add("G03", GMode.G03); gModes.Add("g03", GMode.G03);   //Counter-Clockwise Arc
            gModes.Add("G4", GMode.G04); gModes.Add("g4", GMode.G04);    //Dwell
            gModes.Add("G04", GMode.G04); gModes.Add("g04", GMode.G04);   //Dwell
            gModes.Add("G9", GMode.G09); gModes.Add("g9", GMode.G09);    //Exact Stop
            gModes.Add("G09", GMode.G09); gModes.Add("g09", GMode.G09);   //Exact Stop
            gModes.Add("G10", GMode.G10); gModes.Add("g10", GMode.G10);   //Fixture And Tool Offsetting
            gModes.Add("G12", GMode.G12); gModes.Add("g12", GMode.G12);    //Clockwise Circle
            gModes.Add("G13", GMode.G13); gModes.Add("g13", GMode.G13);    //Counter Clockwise Circle
            gModes.Add("G15", GMode.G15); gModes.Add("g15", GMode.G15);    //Polar Coordinate Cancel
            gModes.Add("G16", GMode.G16); gModes.Add("g16", GMode.G16);    //Polar Coordinate
            gModes.Add("G17", GMode.G17); gModes.Add("g17", GMode.G17);    //XY Plane Select
            gModes.Add("G18", GMode.G18); gModes.Add("g18", GMode.G18);    //ZX Plane Select
            gModes.Add("G19", GMode.G19); gModes.Add("g19", GMode.G19);    //YZ Plane Select
            gModes.Add("G20", GMode.G20); gModes.Add("g20", GMode.G20);    //Inch
            gModes.Add("G21", GMode.G21); gModes.Add("g21", GMode.G21);    //Millimeter
            gModes.Add("G28", GMode.G28); gModes.Add("g28", GMode.G28);    //ZX Plane Select
            gModes.Add("G30", GMode.G30); gModes.Add("g30", GMode.G30);    //2nd,3rd,4th Zero Return
            gModes.Add("G31", GMode.G31); gModes.Add("g31", GMode.G31);    //Probe Function
            gModes.Add("G32", GMode.G32); gModes.Add("g32", GMode.G32);    //Threading*
            gModes.Add("G40", GMode.G40); gModes.Add("g40", GMode.G40);    //Cutter Compensation Cancel
            gModes.Add("G41", GMode.G41); gModes.Add("g41", GMode.G41);    //Cutter Compensation Left
            gModes.Add("G42", GMode.G42); gModes.Add("g42", GMode.G42);    //Cutter Compensation Right
            gModes.Add("G43", GMode.G43); gModes.Add("g43", GMode.G43);    //Tool Length Offset + Enable
            gModes.Add("G44", GMode.G44); gModes.Add("g44", GMode.G44);    //Tool Length Offset - Enable
            gModes.Add("G49", GMode.G49); gModes.Add("g49", GMode.G49);    //Tool Length Offset - Cancel
            gModes.Add("G50", GMode.G50); gModes.Add("g50", GMode.G50);    //Cancel Scaling
            gModes.Add("G51", GMode.G51); gModes.Add("g51", GMode.G51);    //Scale Axes
            gModes.Add("G52", GMode.G52); gModes.Add("g52", GMode.G52);    //Local Coordinate System Shift
            gModes.Add("G53", GMode.G53); gModes.Add("g53", GMode.G53);    //Machine Coordinate System
            gModes.Add("G54", GMode.G54); gModes.Add("g54", GMode.G54);    //Fixture Offset 1
            gModes.Add("G54.1", GMode.G541); gModes.Add("g54.1", GMode.G541); //Additional Fixture Offset
            gModes.Add("G55", GMode.G55); gModes.Add("g55", GMode.G55);    //Fixture Offset 2
            gModes.Add("G56", GMode.G56); gModes.Add("g56", GMode.G56);    //Fixture Offset 3
            gModes.Add("G57", GMode.G57); gModes.Add("g57", GMode.G57);    //Fixture Offset 4
            gModes.Add("G58", GMode.G58); gModes.Add("g58", GMode.G58);    //Fixture Offset 5
            gModes.Add("G59", GMode.G59); gModes.Add("g59", GMode.G59);    //Fixture Offset 6
            gModes.Add("G60", GMode.G60); gModes.Add("g60", GMode.G60);    //Unidirectional Approach
            gModes.Add("G61", GMode.G61); gModes.Add("g61", GMode.G61);    //Exact Stop Mode
            gModes.Add("G64", GMode.G64); gModes.Add("g64", GMode.G64);    //Cutting Mode (Constant Velocity)
            gModes.Add("G65", GMode.G65); gModes.Add("g65", GMode.G65);    //Macro Call
            gModes.Add("G66", GMode.G66); gModes.Add("g66", GMode.G66);    //Macro Modal Call
            gModes.Add("G67", GMode.G67); gModes.Add("g67", GMode.G67);    //Macro Modal Call Cancel
            gModes.Add("G68", GMode.G68); gModes.Add("g68", GMode.G68);    //Coordinate System Rotation
            gModes.Add("G69", GMode.G69); gModes.Add("g69", GMode.G69);    //Coordinate System Rotation Cancel
            gModes.Add("G73", GMode.G73); gModes.Add("g73", GMode.G73);    //High Speed Peck Drilling
            gModes.Add("G74", GMode.G74); gModes.Add("g74", GMode.G74);    //LH Tapping*
            gModes.Add("G76", GMode.G76); gModes.Add("g76", GMode.G76);    //Fine Boring*
            gModes.Add("G80", GMode.G80); gModes.Add("g80", GMode.G80);    //Canned Cycle Cancel
            gModes.Add("G81", GMode.G81); gModes.Add("g81", GMode.G81);    //Hole Drilling
            gModes.Add("G82", GMode.G82); gModes.Add("g82", GMode.G82);    //Spot Face
            gModes.Add("G83", GMode.G83); gModes.Add("g83", GMode.G83);    //Deep Hole Peck Drilling
            gModes.Add("G84", GMode.G84); gModes.Add("g84", GMode.G84);    //RH Tapping*
            gModes.Add("G84.2", GMode.G842); gModes.Add("g84.2", GMode.G842); //RH Rigid Tapping*
            gModes.Add("G84.3", GMode.G843); gModes.Add("g84.3", GMode.G843); //LH Rigid Tapping*
            gModes.Add("G85", GMode.G85); gModes.Add("g85", GMode.G85);    //Boring, Retract at Feed, Spindle On 
            gModes.Add("G86", GMode.G86); gModes.Add("g86", GMode.G86);    //Boring, Retract at Feed, Spindle Off
            gModes.Add("G87", GMode.G87); gModes.Add("g87", GMode.G87);    //Back Boring*
            gModes.Add("G88", GMode.G88); gModes.Add("g88", GMode.G88);    //Boring, Manual Retract
            gModes.Add("G89", GMode.G89); gModes.Add("g89", GMode.G89);    //Boring, Dwell, Retract at Feed, Spindle On
            gModes.Add("G90", GMode.G90); gModes.Add("g90", GMode.G90);    //Absolute Position Mode
            gModes.Add("G90.1", GMode.G901); gModes.Add("g90.1", GMode.G901); //Arc Centre Absolute Mode
            gModes.Add("G91", GMode.G91); gModes.Add("g91", GMode.G91);    //Incremental Position Mode
            gModes.Add("G91.1", GMode.G911); gModes.Add("g91.1", GMode.G911); //Arc Centre Incremental Mode
            gModes.Add("G92", GMode.G92); gModes.Add("g92", GMode.G92);    //Local Coordinate System Setting
            gModes.Add("G92.1", GMode.G921); gModes.Add("g92.1", GMode.G921); //Local Coordinate System Cancel
            gModes.Add("G93", GMode.G93); gModes.Add("g93", GMode.G93);    //Inverse Time Feed
            gModes.Add("G94", GMode.G94); gModes.Add("g94", GMode.G94);    //Feed Per Minute
            gModes.Add("G95", GMode.G95); gModes.Add("g95", GMode.G95);    //Feed Per Revolution*
            gModes.Add("G96", GMode.G96); gModes.Add("g96", GMode.G96);    //Constant Surface Speed*
            gModes.Add("G97", GMode.G97); gModes.Add("g97", GMode.G97);    //Constant Speed
            gModes.Add("G98", GMode.G98); gModes.Add("g98", GMode.G98);    //Initial Point Return
            gModes.Add("G99", GMode.G99); gModes.Add("g99", GMode.G99);    //R Point Return

            InitializeHandlers();
        }

        /// <summary>Initializes the Function handlers matching them to their respective keys</summary>
        public static void InitializeHandlers()
        {
            opHandlers = new Dictionary<GMode, OperationHandler>();

            //Group 0 Operations
            opHandlers.Add(GMode.G04, CoreEngine.SetDwellMode);

            //Group 1 Modal Operations
            opHandlers.Add(GMode.G00, CoreEngine.RapidMove);
            opHandlers.Add(GMode.G01, CoreEngine.LinearFeedMove);
            opHandlers.Add(GMode.G02, CoreEngine.ClockwiseArcFeedMove);
            opHandlers.Add(GMode.G03, CoreEngine.CounterClockwiseArcFeedMove);
            opHandlers.Add(GMode.G12, CoreEngine.ClockwiseCircle);
            opHandlers.Add(GMode.G13, CoreEngine.CounterClockwiseCircle);

            //Group 12 Modal Operators
            opHandlers.Add(GMode.G541, CoreEngine.SetAdditionalFixtureOffset);
            opHandlers.Add(GMode.G54, CoreEngine.SetFixtureOffset1);
            opHandlers.Add(GMode.G55, CoreEngine.SetFixtureOffset2);
            opHandlers.Add(GMode.G56, CoreEngine.SetFixtureOffset3);
            opHandlers.Add(GMode.G57, CoreEngine.SetFixtureOffset4);
            opHandlers.Add(GMode.G58, CoreEngine.SetFixtureOffset5);
            opHandlers.Add(GMode.G59, CoreEngine.SetFixtureOffset6);

            opHandlers.Add(GMode.G90, CoreEngine.PositionModeAbsolute);
            opHandlers.Add(GMode.G91, CoreEngine.PositionModeIncremental);
            opHandlers.Add(GMode.G901, CoreEngine.ArcModeAbsolute);
            opHandlers.Add(GMode.G911, CoreEngine.ArcModeIncremental);

            //Group 2 Modal Operators
            opHandlers.Add(GMode.G17, CoreEngine.XYPlaneSelect);
            opHandlers.Add(GMode.G18, CoreEngine.ZXPlaneSelect);
            opHandlers.Add(GMode.G19, CoreEngine.YZPlaneSelect);

            //Group 6 Modal Operators
            opHandlers.Add(GMode.G20, CoreEngine.SetInch);
            opHandlers.Add(GMode.G21, CoreEngine.SetMilli);

            //Group 5 Modal Operators
            opHandlers.Add(GMode.G94, CoreEngine.SetFeedRate);

            //Group 7 Modal Operators
            opHandlers.Add(GMode.G40, CoreEngine.CancelCutterCompensation);
            opHandlers.Add(GMode.G41, CoreEngine.SetCutterCompensationLeft);
            opHandlers.Add(GMode.G42, CoreEngine.SetCutterCompensationRight);

            //Group 8 Modal Operators
            opHandlers.Add(GMode.G49, CoreEngine.ToolLengthOffsetCancel);
            opHandlers.Add(GMode.G43, CoreEngine.SetToolLengthOffsetPositive);
            opHandlers.Add(GMode.G44, CoreEngine.SetToolLengthOffsetNegative);

            //Group 16 Modal Operators
            opHandlers.Add(GMode.G73, CoreEngine.HighSpeedPeck);
            opHandlers.Add(GMode.G74, CoreEngine.LHTapping);
            opHandlers.Add(GMode.G76, CoreEngine.FineBoring);
            opHandlers.Add(GMode.G80, CoreEngine.CancelCannedCycle);
        }
    }

    /// <summary>Coordinates</summary>
    public class Coord
    {
        public float x, y, z, a, b, c, i, j, k, e, d, f, h, p, q, l, r, s, t;

        /// <summary>Sets all the coordinate values to zero</summary>
        public Coord()
        {
            x = 0; y = 0; z = 0;
            a = 0; b = 0; c = 0;
            i = 0; j = 0; k = 0;
            e = 0; d = 0; f = 0; h = 0;
            p = 0; q = 0; l = 0;
            r = 0; s = 0; t = 0;
        }

        public bool isZero()
        {
            if (x == 0 && y == 0 && z == 0 && a == 0 && b == 0 && c == 0 && i == 0 && j == 0 && k == 0 && r == 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>Preparatory Codes (GCodes)</summary>
    public enum GMode : int
    {
        n, G00, G01, G02, G03, G04, G05, G06, G07, G08, G09, G10, G11,
        G12, G13, G14, G15, G16, G17, G18, G19, G20, G21, G22, G23,
        G24, G25, G26, G27, G28, G29, G30, G31, G32, G33, G34, G35,
        G36, G37, G38, G39, G40, G41, G42, G43, G44, G45, G46, G47,
        G48, G49, G50, G51, G52, G53, G54, G541, G55, G56, G57, G58,
        G59, G60, G61, G62, G63, G64, G65, G66, G67, G68, G69, G70,
        G71, G72, G73, G74, G75, G76, G77, G78, G79, G80, G81, G82,
        G83, G84, G842, G843, G85, G86, G87, G88, G89, G90, G901, G91,
        G911, G92, G921, G93, G94, G95, G96, G97, G98, G99, G100
    }

    public enum MMode : int
    {
        M30
    }

    public enum Tool : int
    {

    }

    /// <summary>Unit Per Measure</summary>
    public enum UPM : int
    {
        inches, millimeters, degrees
    }

    public enum PositionMode : int
    {
        absolute, incremental, arcAbsolute, arcIncremental
    }

    /// <summary>Describes which plane is selected</summary>
    public enum PlaneMode : int
    {
        XY, ZX, YZ
    }

    /// <summary>Describes what should be done with the coordinates passed</summary>
    public enum CoordMode : int
    {
        nil, draw, dwell, fixtureOffset, addFixtureOffset, toolDiameter,
        cutterCompensation, toolLengthOffsetNegative, toolLengthOffsetPositive,
        highSpeedPeck, lhTapping, fineBoring, scale
    }

    /// <summary>Describes the state(process) the simulator is in</summary>
    public enum CoreMode : int
    {
        normal, start, waiting, running, drawStart, drawEnd, dwellStart, dwellEnd, EOF,
        startPeck, cannedCycleDone
    }

    /// <summary>Shows whether a file has been loaded or not</summary>
    public enum LoadMode : int
    {
        loaded, unloaded
    }

    public enum CutterCompensationMode : int
    {
        none, left, right
    }

    public class AppSettings
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Axes Core/";
        private static string filename = "Settings.ini";
        string filePath = Path.Combine(path, filename);

        public float speed;
        public float defaultFeedrate;
        public float maxFeedrate;

        public AppSettings()
        {
            speed = 10;
            defaultFeedrate = 50;
            maxFeedrate = 2000;
        }

        public AppSettings(Dictionary<string, object> settings)
        {
            try
            {
                speed = float.Parse(settings["speed"].ToString());
                defaultFeedrate = float.Parse(settings["defaultFeedrate"].ToString());
                maxFeedrate = float.Parse(settings["maxFeedrate"].ToString());
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error Loading Application Settings: " + e);
            }
        }

        public void Load() //Load settings file from local storage
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string temp = File.ReadAllText(filePath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(temp);

                    speed = settings.speed;
                    defaultFeedrate = settings.defaultFeedrate;
                    maxFeedrate = settings.maxFeedrate;
                }
                catch (Exception e)
                {
                    ErrorHandler.Error("Error Loading Application Settings: " + e);
                }
            }
            ErrorHandler.Log("Application Settings Loaded");
        }

        public void Save()
        {
            string temp = this.ToJson();
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using (StreamWriter sw = new(fs))
            {
                sw.Write(temp);
                sw.Close(); sw.Dispose();
            }
            fs.Close(); fs.Dispose();
        }

        public void SetSpeed(float value)
        {
            speed = value;
            Save();
        }

        public void SetDefaultFeedrate(float value)
        {
            defaultFeedrate = value;
            Save();
        }

        public void SetMaxFeedrate(float value)
        {
            maxFeedrate = value;
            Save();
        }

        /// <summary>Returns This Object As A Json String</summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}