using System.Collections.Generic;
namespace AxesCore
{
    public class CommandDefinitions
    {
        public static Dictionary<string, GMode> gModes = new Dictionary<string, GMode>();
        public static Dictionary<string, MMode> mModes = new Dictionary<string, MMode>();

        public delegate void OperationHandler();

        /// <summary>A dictionary to store all function handlers matching them to a key(type)</summary>
        public Dictionary<GMode, OperationHandler> opHandlers;

        public CommandDefinitions()
        {
            //Initialize G-Modes
            gModes.Add("G0", GMode.G00); //Rapid Move
            gModes.Add("G00", GMode.G00); //Rapid Move
            gModes.Add("G1", GMode.G01); //Linear Feed Move
            gModes.Add("G01", GMode.G01); //Linear Feed Move
            gModes.Add("G2", GMode.G02); //Clockwise Arc Move
            gModes.Add("G02", GMode.G02); //Clockwise Arc Move
            gModes.Add("G3", GMode.G03); //Counter-Clockwise Arc
            gModes.Add("G03", GMode.G03); //Counter-Clockwise Arc
            gModes.Add("G4", GMode.G04); //Dwell
            gModes.Add("G04", GMode.G04); //Dwell
            gModes.Add("G9", GMode.G09); //Exact Stop
            gModes.Add("G09", GMode.G09); //Exact Stop
            gModes.Add("G10", GMode.G10); //Fixture And Tool Offsetting
            gModes.Add("G12", GMode.G12); //Clockwise Circle
            gModes.Add("G13", GMode.G13); //Counter Clockwise Circle
            gModes.Add("G15", GMode.G15); //Polar Coordinate Cancel
            gModes.Add("G16", GMode.G16); //Polar Coordinate
            gModes.Add("G17", GMode.G17); //XY Plane Select
            gModes.Add("G18", GMode.G18); //ZX Plane Select
            gModes.Add("G19", GMode.G19); //YZ Plane Select
            gModes.Add("G20", GMode.G20); //Inch
            gModes.Add("G21", GMode.G21); //Millimeter
            gModes.Add("G28", GMode.G28); //ZX Plane Select
            gModes.Add("G30", GMode.G30); //2nd,3rd,4th Zero Return
            gModes.Add("G31", GMode.G31); //Probe Function
            gModes.Add("G32", GMode.G32); //Threading*
            gModes.Add("G40", GMode.G40); //Cutter Compensation Cancel
            gModes.Add("G41", GMode.G41); //Cutter Compensation Left
            gModes.Add("G42", GMode.G42); //Cutter Compensation Right
            gModes.Add("G43", GMode.G43); //Tool Length Offset + Enable
            gModes.Add("G44", GMode.G44); //Tool Length Offset - Enable
            gModes.Add("G49", GMode.G49); //Tool Length Offset - Cancel
            gModes.Add("G50", GMode.G50); //Cancel Scaling
            gModes.Add("G51", GMode.G51); //Scale Axes
            gModes.Add("G52", GMode.G52); //Local Coordinate System Shift
            gModes.Add("G53", GMode.G53); //Machine Coordinate System
            gModes.Add("G54", GMode.G54); //Fixture Offset 1
            gModes.Add("G54.1", GMode.G541); //Additional Fixture Offset
            gModes.Add("G55", GMode.G55); //Fixture Offset 2
            gModes.Add("G56", GMode.G56); //Fixture Offset 3
            gModes.Add("G57", GMode.G57); //Fixture Offset 4
            gModes.Add("G58", GMode.G58); //Fixture Offset 5
            gModes.Add("G59", GMode.G59); //Fixture Offset 6
            gModes.Add("G60", GMode.G60); //Unidirectional Approach
            gModes.Add("G61", GMode.G61); //Exact Stop Mode
            gModes.Add("G64", GMode.G64); //Cutting Mode (Constant Velocity)
            gModes.Add("G65", GMode.G65); //Macro Call
            gModes.Add("G66", GMode.G66); //Macro Modal Call
            gModes.Add("G67", GMode.G67); //Macro Modal Call Cancel
            gModes.Add("G68", GMode.G68); //Coordinate System Rotation
            gModes.Add("G69", GMode.G69); //Coordinate System Rotation Cancel
            gModes.Add("G73", GMode.G73); //High Speed Peck Drilling
            gModes.Add("G74", GMode.G74); //LH Tapping*
            gModes.Add("G76", GMode.G76); //Fine Boring*
            gModes.Add("G80", GMode.G80); //Canned Cycle Cancel
            gModes.Add("G81", GMode.G81); //Hole Drilling
            gModes.Add("G82", GMode.G82); //Spot Face
            gModes.Add("G83", GMode.G83); //Deep Hole Peck Drilling
            gModes.Add("G84", GMode.G84); //RH Tapping*
            gModes.Add("G84.2", GMode.G842); //RH Rigid Tapping*
            gModes.Add("G84.3", GMode.G843); //LH Rigid Tapping*
            gModes.Add("G85", GMode.G85); //Boring, Retract at Feed, Spindle On 
            gModes.Add("G86", GMode.G86); //Boring, Retract at Feed, Spindle Off
            gModes.Add("G87", GMode.G87); //Back Boring*
            gModes.Add("G88", GMode.G88); //Boring, Manual Retract
            gModes.Add("G89", GMode.G89); //Boring, Dwell, Retract at Feed, Spindle On
            gModes.Add("G90", GMode.G90); //Absolute Position Mode
            gModes.Add("G90.1", GMode.G901); //Arc Centre Absolute Mode
            gModes.Add("G91", GMode.G91); //Incremental Position Mode
            gModes.Add("G91.1", GMode.G911); //Arc Centre Incremental Mode
            gModes.Add("G92", GMode.G92); //Local Coordinate System Setting
            gModes.Add("G92.1", GMode.G921); //Local Coordinate System Cancel
            gModes.Add("G93", GMode.G93); //Inverse Time Feed
            gModes.Add("G94", GMode.G94); //Feed Per Minute
            gModes.Add("G95", GMode.G95); //Feed Per Revolution*
            gModes.Add("G96", GMode.G96); //Constant Surface Speed*
            gModes.Add("G97", GMode.G97); //Constant Speed
            gModes.Add("G98", GMode.G98); //Initial Point Return
            gModes.Add("G99", GMode.G99); //R Point Return
        }

        public void InitializeHandlers()
        {
            opHandlers.Add(GMode.G00, RapidMove);
        }

        public void RapidMove()
        {
            CoreEngine.group1 = GMode.G01;
        }
    }

    public struct Coord
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public struct Axes
    {
        public float a { get; set; }
        public float b { get; set; }
        public float c { get; set; }
    }

    public class Operators
    {

    }

    public enum OperationType : int
    {
        RapidMove, LinearFeedMove, ArcFeedMove
    }

    public enum GMode : int
    {
        G00, G01, G02, G03, G04, G05, G06, G07, G08, G09, G10, G11,
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

    }
}