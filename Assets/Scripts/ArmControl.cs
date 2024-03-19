using AxesCore;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmControl : MonoBehaviour
{
    [Header("Miscellenous")]
    [SerializeField] GameObject arcPrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject drawHolder;
    LineRenderer currentLine;

    [SerializeField] bool running = false;

    [Header("Effector Parameters")]
    [SerializeField] GameObject effector; //The effector object
    [SerializeField] GameObject end; //The tip of the effector

    [Header("Running Parameters")]
    /// <summary>The start coordinate for a draw command</summary>
    public Vector3 startCoord;

    /// <summary>The end coordinate for a draw command</summary>
    public Vector3 endCoord;

    /// <summary>The centerpoint of the arc</summary>
    public Vector3 centerPoint;

    /// <summary>The radius of the defined arc (Arc is uniform)</summary>
    public float radius;

    /// <summary>The starting radius of the defined arc</summary>
    public float startRadius;

    /// <summary>The end radius of the defined arc</summary>
    public float endRadius;

    /// <summary>Distance between the start and end points</summary>
    float d = 0f;

    /// <summary> How many degrees the draw has gone </summary>
    float degree = 0;

    /// <summary>The timer for Lerping Linear Draw Commands</summary>
    float t = 0f;

    /// <summary>The timer for Lerping tinier Arc Draw Commands</summary>
    float m = 0f;

    //Movement Parameters
    float scale = 1;
    float mSpeed;
    float mVelocity;

    //Arc Parameters
    float sweep;
    float startAngle;
    float endAngle;
    float segmentLength = 0.3f;
    int segmentCount;
    int segmentStep;
    bool clockwiseArc;
    Vector3 nextStep;

    void Update()
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
                    case GMode.G00:
                        SetCoords(); //Set the linear coords only
                        break;

                    case GMode.G01:
                        SetCoords(); //Set the linear coords only
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(linePrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.SetPosition(0, startCoord);
                        break;

                    case GMode.G02:
                        SetCoords(true); //Set the linear and arc coords
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.SetPosition(0, startCoord);
                        currentLine.positionCount = segmentCount;
                        clockwiseArc = true;
                        break;

                    case GMode.G03:
                        SetCoords(true); //Set the linear and arc coords
                        //Create a line renderer //Set its first position
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.SetPosition(0, startCoord);
                        currentLine.positionCount = segmentCount;
                        clockwiseArc = false;
                        break;

                    default:
                        SetCoords(true); //Set the linear and arc coords
                        ErrorHandler.Error("Unhadled Draw Command!");
                        break;
                }

                running = true; //Set the simulator state
            }
            else //The draw has started running
            {
                if (Core.group[1] == GMode.G00) //Rapid Move Draw
                {
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    t += (mVelocity / (60 * d)) * Time.deltaTime * mSpeed; //Time Control (mVelocity is the default movement velocity)
                }
                else if (Core.group[1] == GMode.G01) //Linear Move Draw
                {
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    currentLine.SetPosition(1, effector.transform.position);
                    t += (Core.feedRate / (60 * d)) * Time.deltaTime * mSpeed; //Time Control
                }
                else if (Core.group[1] == GMode.G02) //Clockwise Arc Movements
                {
                    effector.transform.position = Vector3.Lerp(startCoord, nextStep, m);
                    currentLine.SetPosition(segmentStep, effector.transform.position);
                    m += (Core.feedRate / (60 * segmentLength)) * Time.deltaTime * mSpeed; //Time Control
                } 
                else if (Core.group[1] == GMode.G03) //Anti-Clockwise Arc Movements
                {
                    effector.transform.position = Vector3.Lerp(startCoord, nextStep, m);
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
                effector.transform.position = Vector3.Lerp(startCoord, nextStep, 1);
                currentLine.SetPosition(segmentStep, effector.transform.position);
                m = 0f; //Done with one segment of drawing
                segmentStep++;
                
                //Get the next intermediate point around the arc
                scale = ((float)segmentStep) / ((float)segmentCount);
                degree = (sweep * scale) + startAngle;
                radius = ((endRadius - startRadius) * scale) + startRadius;

                nextStep = new Vector3(centerPoint.x + Mathf.Cos(degree) * radius, 0, centerPoint.y + Mathf.Sin(degree) * radius);
            }

            if (segmentStep > segmentCount)
            {
                //Approximate the line to the correct ending
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, 1);
                t = 0f; m = 0f; running = false;
                Core.mode = CoreMode.drawEnd; ErrorHandler.Log("Done with Arc Drawing!");
            }
        }

        CheckIfDone();
    }

    public void SetCoords(bool drawArcs = false)
    {
        LoadMovementParameters(); //Load the parameters as specified in the settings
        ResetCoords(); //Clear the coordinates holder
        SetLinearCoords(); //Set the linear coordinates

        if (drawArcs == true)
        {
            SetArcCoords(); //Set the arc coordinates if that setting is on
        }

        ErrorHandler.Log("Starting Point: " + startCoord);
        ErrorHandler.Log("End Point: " + endCoord);
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

        //Format 1 (IJK is the centerpoint of the arc and it is specified)
        //K and J are flipped because of GCode coordinate system
        Vector3 cp = new Vector3(Core.coord.i, Core.coord.k, Core.coord.j);

        if (Core.arcMode == PositionMode.arcAbsolute)
        {
            centerPoint = cp;
        }
        else //PositionMode.arcIncremental
        {
            centerPoint = startCoord + cp;
        }

        if (Core.coord.r == 0 && cp != Vector3.zero) //Calculate and set the radius
        {
            // Center Point here should be given in absolute terms
            startRadius = (startCoord - centerPoint).magnitude;
            endRadius = (endCoord - centerPoint).magnitude;

            startAngle = Mathf.Atan2(startCoord.y - centerPoint.y, startCoord.x - centerPoint.x);
            endAngle = Mathf.Atan2(endCoord.y - centerPoint.y, endCoord.x - centerPoint.x);

            if (startAngle < 0)
            {
                startAngle = (float)((Mathf.PI * 2.0) + startAngle);
            }
            if (endAngle < 0)
            {
                endAngle = (float)((Mathf.PI * 2.0) + endAngle);
            }

            ErrorHandler.Log("Start Radius: " + startRadius + " End Radius: " + endRadius);
            ErrorHandler.Log("Start Angle: " + startAngle + " End Angle: " + endAngle);
        }
        else //Calculate for the Centerpoint
        {
            radius = Core.coord.r;
            centerPoint = CalculateCentrePoint(startCoord, endCoord, radius);
        }


        sweep = endAngle - startAngle;
        float radiusDiff = endRadius - startRadius;

        if (clockwiseArc == true && sweep > 0)
        {
            startAngle += 2 * Mathf.PI;
        }
        else if (clockwiseArc == false && sweep < 0)
        {
            endAngle += 2 * Mathf.PI;
        }

        sweep = endAngle - startAngle;
        float len1 = Mathf.Abs(sweep) * startRadius;
        float len = Mathf.Sqrt((len1 * len1) + (radiusDiff * radiusDiff));

        //segmentCount = (int)Mathf.Max(Mathf.Ceil(len / 10), 1);

        segmentCount = (int)Mathf.Floor(len1 / segmentLength);
        Debug.Log("Segment Count: " + segmentCount);
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
        d = 0; degree = 0;
        startAngle = 0; endAngle = 0; radius = 0;
        segmentStep = 0; segmentCount = 1;
        nextStep = Vector3.zero; endCoord = Vector3.zero; centerPoint = Vector3.zero;
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
        // float len=sweep*circumference/(PI*2.0);
        // simplifies to
        float len = Mathf.Abs(sweep) * rad;
        return len;
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