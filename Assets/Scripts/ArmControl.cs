using AxesCore;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
    [Header("Miscellenous")]
    [SerializeField] private GameObject arcPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject drawHolder;
    private LineRenderer currentLine;

    [SerializeField] private bool running = false;

    [Header("Effector Parameters")]
    [SerializeField] private GameObject effector; ///The effector object
    [SerializeField] private GameObject end; //The tip of the effector

    /// <summary>The start coordinate for a draw command</summary>
    public Vector3 startCoord;
    /// <summary>The end coordinate for a draw command</summary>
    public Vector3 endCoord;
    /// <summary>The centerpoint of the arc</summary>
    public Vector3 centerPoint;
    /// <summary>Distance between the start and end points</summary>
    private float d = 0f;
    /// <summary>The length of the specified arc to be drawn</summary>
    private float arcLength;
    /// <summary>The radius of the specified arc (Arc is uniform)</summary>
    public float radius;

    /// <summary>The timer for Lerping Linear Draw Commands</summary>
    private float t = 0f;
    /// <summary>The timer for Lerping Tinier Arc Draw Commands</summary>
    private float m = 0f;

    //Arc Parameters
    private float sweep;
    /// <summary>The starting angle for the arc</summary>
    private float startAngle;
    /// <summary>The ending angle for the arc</summary>
    private float endAngle;

    private float angleStep;

    /// <summary>The length of each line segment of the arc</summary>
    private float segmentLength = 0.3f;
    /// <summary>The number of segments in an arc</summary>
    private int segmentCount;
    /// <summary>The index of the current segment</summary>
    private int segmentStep;
    /// <summary>Clockwise or anticlockwise arc?</summary>
    private bool clockwiseArc;
    /// <summary>The list of the points in the arc</summary>
    private Vector3[] arcPoints;

    //Movement Parameters
    private float mSpeed;
    private float mVelocity;

    private void Update()
    {
        if (Core.mode == CoreMode.dwellStart) //Dwell Mode
        {
            Core.dwellTime -= Time.deltaTime;

            if (Core.dwellTime <= 0) //Timing Control
            {
                Core.dwellTime = 0; Core.mode = CoreMode.dwellEnd;
                ErrorHandler.Log("Done with dwell");
            }
        }
        else if (Core.mode == CoreMode.drawStart) //Draw mode
        {
            if (running == false) //The draw has not started (So set the coordinates (only once per draw))
            {
                switch (Core.group[1])
                {
                    case GMode.G00: //Rapid Move
                        SetCoords(); //Set the linear coords only
                        break;

                    case GMode.G01: //Linear Move
                        SetCoords(); //Set the linear coords only
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(linePrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.SetPosition(0, startCoord);
                        break;

                    case GMode.G02: //Clockwise Arc Draw
                        clockwiseArc = true;
                        SetCoords(true); //Set the linear and arc coords
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.positionCount = segmentCount; // Set the counts of the Line Renderer
                        currentLine.SetPosition(0, startCoord);
                        segmentStep++;
                        break;

                    case GMode.G03:
                        clockwiseArc = false;
                        SetCoords(true); //Set the linear and arc coords
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.positionCount = segmentCount; // Set the counts of the Line Renderer
                        currentLine.SetPosition(0, startCoord);
                        segmentStep++;
                        break;

                    default:
                        ErrorHandler.Error("Unhandled Draw Command!");
                        break;
                }
                running = true; //Set the simulator state
            }
            else //The draw has started running
            {
                if (Core.group[1] == GMode.G00) //Rapid Move Draw
                {
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    //Time Control (mVelocity is the default movement velocity)
                    t += (mVelocity / (60 * d)) * Time.deltaTime * mSpeed;
                }
                else if (Core.group[1] == GMode.G01) //Linear Move Draw
                {
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    currentLine.SetPosition(1, effector.transform.position);
                    t += (Core.feedRate / (60 * d)) * Time.deltaTime * mSpeed; //Time Control
                }
                else if (Core.group[1] == GMode.G02) //Clockwise Arc Movements
                {
                    effector.transform.position = Vector3.Lerp(arcPoints[segmentStep - 1], arcPoints[segmentStep], m);
                    currentLine.SetPosition(segmentStep, effector.transform.position);
                    m += (Core.feedRate / (60 * segmentLength)) * Time.deltaTime * mSpeed; //Time Control
                }
                else if (Core.group[1] == GMode.G03) //Anti-Clockwise Arc Movements
                {
                    effector.transform.position = Vector3.Lerp(arcPoints[segmentStep - 1], arcPoints[segmentStep], m);
                    currentLine.SetPosition(segmentStep, effector.transform.position);
                    m += (Core.feedRate / (60 * segmentLength)) * Time.deltaTime * mSpeed; //Time Control
                }
            }

            //Linear Timing Control
            if (t >= 1.0f)
            {
                //Approximate the line to the correct ending
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, 1);
                t = 0f; running = false;
                Core.mode = CoreMode.drawEnd; ErrorHandler.Log("Done with Linear Drawing!");
            }

            //Arc Draw Timing Control
            if (m >= 1.0f)
            {
                effector.transform.position = Vector3.Lerp(startCoord, arcPoints[segmentStep], 1);
                m = 0; //Done with one segment of drawing
                segmentStep++;
            }

            //Approximate the endings
            if (segmentStep >= segmentCount)
            {
                //Approximate the line to the correct ending
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, 1);
                currentLine.SetPosition(segmentStep-1, endCoord);
                t = 0f; m = 0f; running = false;
                Core.mode = CoreMode.drawEnd; ErrorHandler.Log("Done with Arc Drawing!");
            }
        }

        CheckIfDone();
    }

    /// <summary>Resets and then set the new coordinates</summary>
    public void SetCoords(bool drawArcs = false)
    {
        LoadMovementParameters(); //Load the parameters as specified in the settings
        ResetCoords(); //Clear the coordinates holder
        SetLinearCoords(); //Set the linear coordinates

        ErrorHandler.Log("Starting Point: " + startCoord + " End Point: " + endCoord);
        
        //Set the arc coordinates if it is specified
        if (drawArcs == true) SetArcCoords(); 
    }

    /// <summary>Sets the coordinates required to draw linear lines</summary>
    public void SetLinearCoords()
    {
        ErrorHandler.Log("Setting the Linear Coords");
        if (Core.positionMode == PositionMode.absolute)
        {
            foreach (var s in Core.coordList)
            {
                ErrorHandler.Log("Coordinate: " + s);
            }

            //The coordinates z and y are flipped becaue of the gcode coordinate system
            endCoord = new()
            {
                x = Core.coordList.Contains("x") ? Core.coord.x : startCoord.x,
                y = Core.coordList.Contains("z") ? Core.coord.z : startCoord.y,
                z = Core.coordList.Contains("y") ? Core.coord.y : startCoord.z
            };
        }
        else //PositionMode.incremental
        {
            endCoord = startCoord + new Vector3(Core.coord.x, Core.coord.z, Core.coord.y);
        }

        d = (endCoord - startCoord).magnitude; //Distance between the start and end points
    }

    /// <summary>Sets the coordinates required to draw an arc</summary>
    public void SetArcCoords()
    {
        ErrorHandler.Log("Setting the Arc Coords");

        Vector3 cp = new Vector3(Core.coord.i, Core.coord.k, Core.coord.j); //K and J are flipped because of GCode coordinate system
        centerPoint = (Core.arcMode == PositionMode.arcAbsolute) ? cp : startCoord + cp; //Set the centerpoint

        if (Core.coord.r == 0 && cp != Vector3.zero) //Calculate and set the radius (Assumes a uniform circular arc)
        {
            //Format 1 (IJK is the centerpoint of the arc and it is specified)
            radius = (startCoord - centerPoint).magnitude; //Calculate the radius
        }
        else //Calculate for the Centerpoint
        {
            //Format 2 (r is the radius of the arc , we then calculate for the centerpoint)
            radius = Core.coord.r;
            centerPoint = CalculateCentrePoint(startCoord, endCoord, radius);
        }
        ErrorHandler.Log("CenterPoint: " + centerPoint);
        startAngle = Mathf.Atan2(startCoord.y - endCoord.y, startCoord.x - endCoord.x);
        endAngle = Mathf.Atan2(endCoord.y - startCoord.y, endCoord.x - startCoord.x);

        if (startAngle < 0) startAngle = (float)((Mathf.PI * 2.0) + startAngle);
        if (endAngle < 0) endAngle = (float)((Mathf.PI * 2.0) + endAngle);

        sweep = endAngle - startAngle;
        if (clockwiseArc && sweep > 0) //Clockwise arc
        {
            startAngle += 2 * Mathf.PI;
        }
        else if (!clockwiseArc && sweep < 0) //Anti-Clockwise arc
        {
            endAngle += 2 * Mathf.PI;
        }

        ErrorHandler.Log("Start Angle: " + (startAngle * Mathf.Rad2Deg) + " End Angle: " + (endAngle * Mathf.Rad2Deg));

        sweep = endAngle - startAngle;
        arcLength = CalculateArcLength(sweep, radius);

        //segmentCount = (int)(arcLength / segmentLength); //Calculate the segment count
        segmentCount = 5; //Calculate the segment count
        angleStep = (endAngle - startAngle) / (segmentCount - 1); //Calculate angle step based on number of segments
        arcPoints = new Vector3[segmentCount]; //Create an array of Vector3 points for the arc

        for (int i = 0; i < segmentCount; i++) // Loop through segments and calculate positions
        {
            float angle = startAngle + (angleStep * i); //Angles are already in radians
            //float radians = Mathf.Deg2Rad * angle;
            //arcPoints[i] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            arcPoints[i] = centerPoint + (radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
        }
    }

    /// <summary>Checks to see if the simulator is done with a command and sets the appropriate mode</summary>
    public void CheckIfDone()
    {
        if (Core.mode == CoreMode.drawEnd || Core.mode == CoreMode.dwellEnd)
        {
            Core.mode = CoreMode.normal; running = false;
            ErrorHandler.Log("Done With The Command!");
        }
    }

    /// <summary>Resets all the coordinates to default</summary>
    public void ResetCoords()
    {
        d = 0; arcLength = 0; radius = 0;
        startAngle = 0; endAngle = 0; 
        segmentStep = 0; segmentCount = 1; arcPoints = null;
        endCoord = Vector3.zero; centerPoint = Vector3.zero;
        startCoord = end.transform.position; //The positon should be gotten from the tip
    }

    /// <summary>Calculate the centrepoint for the arc given the respective parameters</summary>
    public Vector3 CalculateCentrePoint(Vector3 start, Vector3 end, float rad)
    {
        ErrorHandler.Log("Calculating the centerpoint");
        float r = rad; //The radius
        float d = (end - start).magnitude; //The distance between the 2 points
        Vector3 center = Vector3.Lerp(start, end, 0.5f); //Midpoint of the start and end points

        Vector3 cp = new();

        if (r < 0) //First Circle (Negative r)
        {
            cp.x = center.x + Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * ((start.y - end.y) / d));
            cp.y = center.y + Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * ((end.x - start.x) / d));
        }
        else //Second Cirle (Positive r)
        {
            cp.x = center.x - Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * (start.y - end.y) / d);
            cp.y = center.y - Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * (end.x - start.x) / d);
        }

        //Vector3 centerPoint;
        return cp;
    }

    /// <summary>Returns the degree of the angle between these 2 vectors in degrees</summary>
    public float CalculateAngle(Vector3 start, Vector3 end, Vector3 cp)
    {
        Vector3 a = start - cp;
        Vector3 b = end - cp;

        return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
    }

    /// <summary>Returns the length of the arc based on the given params</summary>
    public float CalculateArcLength(float sweep, float rad)
    {
        // float circumference=PI*2.0*radius;
        // float len=sweep*circumference/(PI*2.0) //simply
        return Mathf.Abs(sweep) * rad;
    }

    public float atan3(Vector3 start, Vector3 end)
    {
        float dy = start.x - end.x;
        float dx = start.y - end.y;

        double a = Mathf.Atan2(dy, dx);

        if (a < 0)
        {
            a = (Mathf.PI * 2.0) + a;
        }

        return (float)a;
    }

    /// <summary>Load default values as defined from the settings!</summary>
    public void LoadMovementParameters()
    {
        mVelocity = PlayerPrefs.GetInt("velocity");
        mSpeed = PlayerPrefs.GetInt("speed");
    }
}