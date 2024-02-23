using AxesCore;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
    [Header("Miscellenous")]
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject drawHolder;
    LineRenderer currentLine;

    [SerializeField] bool running;

    [Header("Effector Parameters")]
    [SerializeField] GameObject effector;
    [SerializeField] TrailRenderer trail;

    [Header("Running Parameters")]
    /// <summary>The start coordinate for a draw command</summary>
    public Vector3 startCoord;

    /// <summary>The end coordinate for a draw command</summary>
    public Vector3 endCoord;

    /// <summary>The centerpoint of the arc</summary>
    public Vector3 centerPoint;

    /// <summary>The radius of the defined arc</summary>
    public float radius;

    /// <summary>The angle between the end and start points in degrees</summary>
    float angle = 0f;

    /// <summary>Distance between the start and end points</summary>
    float d = 0f;

    bool draw = false;
    float timer = 0;
    float degree = 0;
    float t = 0f; //For Timing LERP operations

    void Start()
    {
        running = false;
    }


    void Update()
    {
        if (Core.mode == CoreMode.dwellStart)
        {
            Core.dwellTime -= Time.deltaTime;

            if (Core.dwellTime <= 0) //Timing Control
            {
                Core.dwellTime = 0;
                Core.mode = CoreMode.dwellEnd;
                ErrorHandler.Log("Done with dwell");
            }
        }
        else if (Core.mode == CoreMode.drawStart) //Draw mode
        {
            if (running == false) //The draw has not started
            {
                switch (Core.group[1])
                {
                    case GMode.G00:
                        SetCoords(); //Set the linear coords only
                        break;
                    case GMode.G01:
                        SetCoords(); //Set the linear coords only
                        //Create line renderer
                        //currentLine = Instantiate(linePrefab, drawHolder.transform).GetComponent<LineRenderer>();
                        //currentLine.SetPosition(0, startCoord); ///Set its first position
                        break;
                    default:
                        SetCoords(true); //Set the linear and arc coords
                        break;
                }
                running = true; //Set the simulator state
            }
            else //The draw has started running
            {
                if (Core.group[1] == GMode.G00) //Rapid Move Draw
                {
                    draw = false; //No trail
                    trail.emitting = draw; //Determines whether a trail would be drawn or not
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    t += Time.deltaTime / d; //Time Control
                }
                else if (Core.group[1] == GMode.G01) //Linear Move Draw
                {
                    draw = true; //Draw a trail
                    trail.emitting = draw; //Determines whether a trail would be drawn or not
                    effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                    //currentLine.SetPosition(1, effector.transform.position);
                    t += (Core.feedRate / (60 * d)) * Time.deltaTime; //Time Control
                }
            }

            /* Arc Drawing Code
            if (Core.group[1] == GMode.G02) //Clockwise Arc Movements
            {
                draw = true; SetCoords(true);
                float drawAngle = Mathf.LerpAngle(0, angle, degree/angle);
                Vector3 pointOnCircle = centerPoint + radius * new Vector3(Mathf.Cos(drawAngle), 0, Mathf.Sin(drawAngle));

                effector.transform.position = pointOnCircle;
                degree += 1 * Time.deltaTime; //Time Control
            }
            else if (Core.group[1] == GMode.G03) //Anti-Clockwise Arc Movements
            {
                draw = true; SetCoords(true); 
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                t += (Core.feedRate / (60 * d)) * Time.deltaTime; //Time Control
            }
            */
            
            //Timing Control
            if (t >= 1.0f)
            {
                t = 0f; running = false;
                Core.mode = CoreMode.drawEnd;
                ErrorHandler.Log("Done with Linear Drawing!");
            }

            //Arc Draw Timing Control
            if (degree > angle) //Should be the angle between these 2 vectors
            {
                degree = 0f; //running = false;
                //Core.mode = CoreMode.drawEnd;
                ErrorHandler.Log("Done with Arc Drawing!");
            }
        }

        CheckIfDone();
    }

    public void SetCoords(bool drawArcs = false)
    {
        ResetCoords(); //Clear the coordinates holder

        SetLinearCoords(); //Set the linear coordinates
        if (drawArcs == true)
        {
            SetArcCoords(); //Set the arc coordinates if that setting is on
        }
        ErrorHandler.Log("Set Coordinates");
    }

    /// <summary>Sets the coordinates required to draw linear lines</summary>
    public void SetLinearCoords()
    {
        ErrorHandler.Log("Setting the Linear Coords");
        if (Core.positionMode == PositionMode.absolute)
        {
            endCoord = new Vector3(Core.coord.x, Core.coord.z, Core.coord.y);
        }
        else //PositionMode.incremental
        {
            endCoord = startCoord + new Vector3(Core.coord.x, Core.coord.z, Core.coord.y);
        }

        d = (endCoord - startCoord).magnitude; //Distance between the start and end points
    }

    public void SetArcCoords()
    {
        ErrorHandler.Log("Setting the Arc Coords");
        
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
            //Center Point here should be given in absolute terms
            float r1 = (startCoord - centerPoint).magnitude;
            float r2 = (endCoord - centerPoint).magnitude;

            if (r1 != r2) //Check to make sure the points are equidistance
            {
                ErrorHandler.Error("R1: " + r1 + " R2: " + r2);
            }

            radius = r2;
        }
        else //Calculate for the Centerpoint
        {
            radius = Core.coord.r;
            centerPoint = CalculateCentrePoint(startCoord, endCoord, radius);
        }

        angle = CalculateAngle(startCoord, endCoord, centerPoint);
    }
    
    public void CheckIfDone()
    {
        if (Core.mode == CoreMode.drawEnd || Core.mode == CoreMode.dwellEnd)
        {
            Core.mode = CoreMode.normal;
            running = false;
            ErrorHandler.Log("Done With The Command!");
        }
    }

    public void ResetCoords()
    {
        d = 0; degree = 0;
        angle = 0; radius = 0;
        endCoord = new();
        centerPoint = new();
        startCoord = effector.transform.position;
    }

    public Vector3 CalculateCentrePoint(Vector3 start, Vector3 end, float radius)
    {
        ErrorHandler.Log("Calculating the centerpoint");
        float r = radius;
        float d = (end - start).magnitude;
        Vector3 center = Vector3.Lerp(start, end, 0.5f); //Midpoint
        //Vector3 center = (start + end) / 2;

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
}
