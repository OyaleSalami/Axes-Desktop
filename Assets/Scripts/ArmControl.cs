using AxesCore;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
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

    /// <summary>Distance between the start and end points</summary>
    float d = 0f;

    bool draw = false;
    float timer = 0;
    float degree = 0;

    void Start()
    {
        running = false;
    }

    static float t = 0f; //For Timing LERP operations

    void Update()
    {
        if (Core.mode == CoreMode.dwellStart)
        {
            Core.dwellTime -= Time.deltaTime;

            if(Core.dwellTime <= 0)
            {
                Core.dwellTime = 0;
                Core.mode = CoreMode.dwellEnd;
            }
        }
        else if (Core.mode == CoreMode.drawStart)
        {
            if (Core.group[1] == GMode.G00) //Rapid Move
            {
                SetCoords(); draw = false;
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                t += Time.deltaTime / d; //Time Control
            }
            else if (Core.group[1] == GMode.G01) //Linear Move
            {
                SetCoords(); draw = true;
                effector.transform.position = Vector3.Lerp(startCoord, endCoord, t);
                t += (Core.feedRate / (60 * d)) * Time.deltaTime; //Time Control
            }
            else if (Core.group[1] == GMode.G02) //Clockwise Arc Movements
            {
                SetCoords(); draw = true;
                Vector3 temp = new()
                {
                    x = centerPoint.x + (radius * Mathf.Cos(degree * Mathf.Deg2Rad)),
                    z = centerPoint.z + (radius * Mathf.Sin(degree * Mathf.Deg2Rad))
                };
                

                effector.transform.position = temp;
                degree += 1 * Time.deltaTime; //Time Control
            }
            else if (Core.group[1] == GMode.G03) //Anti-Clockwise Arc Movements
            {

            }


            //Timing Control
            if (t >= 1.0f)
            {
                t = 0f;
                running = false;
                Core.mode = CoreMode.drawEnd;
            }

            if (degree >= 90f) //Should be the angle between this 2 vectors
            {
                degree = 0f;
                running = false;
                Core.mode = CoreMode.drawEnd;
            }
        }

        CheckIfDone();
    }

    public void SetCoords()
    {
        if (running == true) return;
        ResetCoords();

        if (Core.positionMode == PositionMode.absolute)
        {
            endCoord = new Vector3(Core.coord.x, Core.coord.z, Core.coord.y);
        }
        else //PositionMode.incremental
        {
            endCoord = startCoord + new Vector3(Core.coord.x, Core.coord.z, Core.coord.y);
        }

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
            ErrorHandler.Log("Drawing an Arc!");

            float r1 = (startCoord- centerPoint).magnitude;
            float r2 = (endCoord - centerPoint).magnitude;

            if(Mathf.Approximately(r1, r2)) //Check to make sure the points are equidistance
            {
                ErrorHandler.Error("R1: " + r1 + " R2: " + r2);
                throw new Exception("R1 is not equal to R2");
            }

            radius = r1;
        }
        else //Calculate for the centerpoint
        {
            radius = Core.coord.r;
        }

        d = (endCoord - startCoord).magnitude;
        trail.emitting = draw;
        running = true;
    }

    public void CheckIfDone()
    {
        if(Core.mode == CoreMode.drawEnd || Core.mode == CoreMode.dwellEnd)
        {
            Core.mode = CoreMode.done;
        }
        running = false;
    }


    public void ResetCoords()
    {
        d = 0; degree = 0;
        endCoord = new();
        centerPoint = new();
        startCoord = effector.transform.position;
    }

    public Vector3 CalculateCentrePoint(Vector3 start, Vector3 end, float radius)
    {
        Vector3 center = Vector3.Lerp(start, end, 0.5f); //Midpoint


        //Vector3 centerPoint;
        return centerPoint.normalized * radius;
    }
}
