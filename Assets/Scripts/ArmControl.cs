﻿using AxesCore;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject arcPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject drawHolder;
    private LineRenderer currentLine;

    [Header("Effector Parameters")]
    [SerializeField] private GameObject effector; ///The effector object
    [SerializeField] private GameObject end; //The tip of the effector
    [SerializeField] private bool running = false;

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

    //Arc Parameters
    /// <summary>The angle of the arc to be drawn (degree)</summary>
    private float sweep;
    /// <summary>The starting angle for the arc (degree)</summary>
    private float startAngle;
    /// <summary>The ending angle for the arc (degree)</summary>
    private float endAngle;
    /// <summary>Clockwise or anticlockwise arc?</summary>
    private bool clockwiseArc;


    /// <summary>The length of each line segment of the arc</summary>
    private float segmentLength = 0.75f;
    /// <summary>The number of segments in an arc</summary>
    private int segmentCount;
    /// <summary>The list of the points in the arc</summary>
    private List<Vector3> arcPoints;

    //Timers
    /// <summary>The timer for Lerping Linear Draw Commands</summary>
    private float t = 0f;
    /// <summary>The timer for Lerping Tinier Arc Draw Commands</summary>
    private float m = 0f;
    /// <summary>The index of the current segment</summary>
    private int segmentStep;
    /// <summary>The stepping angle for the segments in an arc</summary>
    private float angleStep;

    //Movement Parameters
    /// <summary>The speed of the simulator loaded from the settings</summary>
    private float mSpeed;
    /// <summary>The default velocity of the simulator (useful for G00 commands)</summary>
    private float mVelocity;

    bool drawArc;

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
                        SetCoords(); //Set the linear coords only (no draw line)
                        break;

                    case GMode.G01: //Linear Move
                        SetCoords(); //Set linear coords only //Create a line renderer //Set its first position
                        currentLine = Instantiate(linePrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        currentLine.SetPosition(0, startCoord);
                        break;

                    case GMode.G02: //Clockwise Arc Draw
                        clockwiseArc = true; SetCoords(true); //Set the linear and arc coords 
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>(); //Create a line renderer
                        currentLine.SetPosition(segmentStep, startCoord); segmentStep++; //Set the first position of the arc and add the index
                        currentLine.positionCount = segmentStep + 1;
                        break;

                    case GMode.G03: //Anti-Clockwise Arc Draw
                        clockwiseArc = false; SetCoords(true); //Set the linear and arc coords  
                        currentLine = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>(); //Create a line renderer
                        currentLine.SetPosition(0, startCoord); segmentStep++; //Set its first position of the arc and add the index
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
                    t += (mVelocity / (60 * d)) * Time.deltaTime * mSpeed;//Time Control
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
                    currentLine.SetPosition(segmentStep, effector.transform.position); //Set the position of the arc point
                    m += (Core.feedRate / (60 * segmentLength)) * Time.deltaTime * mSpeed; //Time Control
                }
                else if (Core.group[1] == GMode.G03) //Anti-Clockwise Arc Movements
                {
                    effector.transform.position = Vector3.Lerp(arcPoints[segmentStep - 1], arcPoints[segmentStep], m);
                    currentLine.SetPosition(segmentStep, effector.transform.position); //Set the position of the arc point
                    m += (Core.feedRate / (60 * segmentLength)) * Time.deltaTime * mSpeed; //Time Control
                }
            }

            //Linear Timing Control
            if (t >= 1.0f)
            {
                //Approximate the line to the correct ending
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, 1);
                t = 0f; running = false;
                Core.mode = CoreMode.drawEnd; ErrorHandler.Log("Done With Linear Drawing!");
            }

            //Arc Draw Timing Control
            if (m >= 1.0f)
            {
                //Approximate the line segment to the correct ending
                effector.transform.position = Vector3.Lerp(arcPoints[segmentStep - 1], arcPoints[segmentStep], 1);
                m = 0; //Done with one segment of drawing
                segmentStep++; currentLine.positionCount = segmentStep + 1;
            }

            //Approximate the endings (Done with the drawing of all segments)
            if (segmentStep >= segmentCount && drawArc == true)
            {
                //Approximate the last line segment to the correct ending
                effector.transform.position = Vector3.Lerp(arcPoints[segmentStep - 2], arcPoints[segmentStep - 1], 1);
                currentLine.SetPosition(currentLine.positionCount - 1, effector.transform.position);
                t = 0f; m = 0f; running = false;
                Core.mode = CoreMode.drawEnd; ErrorHandler.Log("Done With The Arc Drawing!");
            }
        }
        CheckIfDone();
    }

    /// <summary>Resets and then set the new coordinates</summary>
    public void SetCoords(bool drawArcs = false)
    {
        ResetCoords(); //Clear the coordinates holder
        LoadMovementParameters(); //Load the parameters as specified in the settings
        SetLinearCoords(); //Set the linear coordinates

        ErrorHandler.Log("Starting Point: " + startCoord + " End Point: " + endCoord);

        //Set the arc coordinates if it is specified
        if (drawArcs == true)
        {
            SetArcCoords(); //Set the arc coordinates
            ErrorHandler.Log("Radius: " + radius + " Arc Angle: " + sweep + " Arc Length : " + arcLength + " Center Point: " + centerPoint);
            ErrorHandler.Log("Start Angle: " + startAngle + " End Angle: " + endAngle);
        }
    }

    /// <summary>Sets the coordinates required to draw linear lines</summary>
    public void SetLinearCoords()
    {
        if (Core.positionMode == PositionMode.absolute)
        {
            endCoord = new() //The coordinates z and y are flipped becaue of the gcode coordinate system
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
        //Setting up the centerpoint
        if (!Core.coordList.Contains("i") && !Core.coordList.Contains("j") && !Core.coordList.Contains("k"))
        {
            //Centerpoint was not specified (Calculate it!)
            radius = Core.coord.r;
            centerPoint = CalculateCentrePoint(startCoord, endCoord, radius);
        }
        else
        {
            Vector3 cp = new Vector3(Core.coord.i, Core.coord.k, Core.coord.j); //K and J are flipped because of GCode coordinate system
            centerPoint = (Core.arcMode == PositionMode.arcAbsolute) ? cp : startCoord + cp; //Set the centerpoint in the appropriate mode
        }
        //Setting up the radius
        radius = Core.coordList.Contains("r") ? Core.coord.r : CalculateRadius(startCoord, centerPoint);

        //Setting up the Sweep, Start and End angles (Angles specified in degrees)
        startAngle = Core.coordList.Contains("c") ? Core.coord.c : CalculateStartAngle();
        endAngle = Core.coordList.Contains("e") ? Core.coord.e : CalculateEndAngle();
        if (startAngle < 0 || endAngle < 0) { ErrorHandler.Error("Negative Angles Are Not Allowed!"); }

        bool flipDirection;
        sweep = endAngle - startAngle; //The angle of the arc //negative = Clockwise //positive = Anticlockwise
        if (sweep > 90) { startAngle += 360; }
        if (sweep < -90) { startAngle -= 360; }

        sweep = endAngle - startAngle; //Recalculate the sweep angle
        arcLength = CalculateArcLength(sweep, radius); //Calculate the length of the arc
        flipDirection = (sweep < 0);

        //Calculate the segment count
        segmentCount = (int)(arcLength / segmentLength); //Calculate the segment count
        angleStep = sweep / (segmentCount - 1); //Calculate the stepping angle based on number of segments
        arcPoints = new List<Vector3>(segmentCount); //Create an array for the points of the arc

        // Determine the points on the arc
        for (int i = 0; i < segmentCount; i++)
        {
            //Based on if it is a clockwise arc or not
            float angle = (startAngle) + (angleStep * i);

            float radians = Mathf.Deg2Rad * angle; //Convert angles from degrees to radian
            arcPoints.Add(centerPoint + new Vector3(Mathf.Cos(radians) * radius, 0f, Mathf.Sin(radians) * radius));
        }

        if(clockwiseArc == true)
        {
            if(flipDirection != true)
            {
                arcPoints.Reverse();
            }
        }
        drawArc = true;
    }

    /// <summary>Checks to see if the simulator is done with a command and sets the appropriate mode</summary>
    public void CheckIfDone()
    {
        if (Core.mode == CoreMode.drawEnd || Core.mode == CoreMode.dwellEnd)
        {
            running = false; Core.mode = CoreMode.normal;
            ErrorHandler.Log("Done With The Command!");
        }
    }

    /// <summary>Resets all the coordinates to default</summary>
    public void ResetCoords()
    {
        //Linear Details
        d = 0; endCoord = Vector3.zero;
        startCoord = end.transform.position; //The positon should be gotten from the tip

        //Arc Details
        startAngle = 0; endAngle = 0; arcLength = 0; radius = 0; drawArc = false; arcPoints = new List<Vector3>();
        segmentStep = 0; segmentCount = 1; arcPoints = null; centerPoint = Vector3.zero;
    }

    /// <summary>Calculate the centrepoint for the arc by calculating the intersection of 2 circles</summary>
    public Vector3 CalculateCentrePoint(Vector3 start, Vector3 end, float radius)
    {
        float r = radius; //The radius
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
        ErrorHandler.Log("Calculated centrepoint: " + cp.ToString());
        return cp; //Vector3 centerPoint;
    }

    /// <summary>Returns the length of the arc based on the given params (Angle is in degrees)</summary>
    public float CalculateArcLength(float arcAngle, float radius)
    {
        //Arc length = θ/360 of 2πr = θ/360 × 2πr
        return Mathf.Abs(arcAngle / 360) * (2 * Mathf.PI * radius);
    }

    /// <summary>Returns the radius of the arc given the specified params</summary>
    public float CalculateRadius(Vector3 start, Vector3 center)
    {
        //NOTE: Calculate and set the radius (Assumes a uniform circular arc)
        return (start - center).magnitude;
    }

    /// <summary>Calculate the end angle of an arc</summary>
    public float CalculateEndAngle(Vector3 end = new(), Vector3 center = new())
    {
        return Mathf.Rad2Deg * Mathf.Atan2(startCoord.y - endCoord.y, startCoord.x - endCoord.x);
    }

    /// <summary>Calculate the start angle of an arc</summary>
    public float CalculateStartAngle(Vector3 end = new(), Vector3 center = new())
    {
        return Mathf.Rad2Deg * Mathf.Atan2(endCoord.y - startCoord.y, endCoord.x - startCoord.x);
    }

    /// <summary>Load default values as defined from the settings!</summary>
    public void LoadMovementParameters()
    {
        mVelocity = PlayerPrefs.GetInt("velocity", 100);
        mSpeed = PlayerPrefs.GetInt("speed", 1);
    }
}