using UnityEngine;
using AxesCore;
using System.Linq;

public class ArmControl: MonoBehaviour
{
    [SerializeField] bool running;

    [Header("Effector Parameters")]
    [SerializeField] GameObject effector;
    [SerializeField] TrailRenderer trail;

    [Header("Running Parameters")]
    public Vector3 startCoord;
    public Vector3 endCoord;
    public Vector3 centerPoint;
    public float radius;
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
        if(Core.mode == CoreMode.drawStart)
        {
            //Linear movements
            if (Core.group[1] == GMode.G00 || Core.group[1] == GMode.G01)
            {
                effector.transform.position = Vector3.Lerp(startCoord, endCoord,t);
                t += (Core.feedRate/(60*d)) * Time.deltaTime;
            }

            //Clockwise Arc Movements
            if (Core.group[1] == GMode.G02)
            {
                Vector3 temp = new();
                radius = CalculateRadius();
                temp.x = centerPoint.x + (radius * Mathf.Cos(degree));
                temp.z = centerPoint.z + (radius * Mathf.Sin(degree));

                effector.transform.position = temp;
                degree += 1 * Time.deltaTime; 
            }

            //Anti-Clockwise Arc Movements
            if (Core.group[1] == GMode.G03)
            {

            }
        }

        if(t >= 1.0f)
        {
            t = 0f;
            Core.mode = CoreMode.drawEnd;
        }

        if(degree >= 30f) //The distance between this 2 vecors
        {
            degree = 0f;
            Core.mode = CoreMode.drawEnd;
        }
    }

    public void Draw()
    {
        switch (Core.group[1])
        {
            case GMode.G00:
                draw = false;
                SetCoords();
                break;

            case GMode.G01:
                draw = true;
                SetCoords();
                break;

            case GMode.G02:
                draw = true;
                SetCoords();
                break;

            case GMode.G03:
                draw = true;
                SetCoords();
                break;

            case GMode.G04:
                draw = true;
                break;

            default:
                break;
        }
    }

    public void SetCoords()
    {
        ResetCoords();

        if (Core.positionMode == PositionMode.absolute)
        {
            endCoord = new Vector3(Core.coord.c[0], Core.coord.c[2], Core.coord.c[1]);
        }
        else //PositionMode.incremental
        {
            endCoord = startCoord + new Vector3(Core.coord.c[0], Core.coord.c[2], Core.coord.c[1]);
        }

        if(Core.arcMode == PositionMode.arcAbsolute)
        {
            //X Y Z
            //I K J
            centerPoint = new Vector3(Core.coord.c[7], Core.coord.c[9], Core.coord.c[8]);
        }
        else
        {
            centerPoint = startCoord + new Vector3(Core.coord.c[7], Core.coord.c[9], Core.coord.c[8]);
        }

        d = (endCoord - startCoord).magnitude;

        trail.emitting = draw;
    }

    public void ResetCoords()
    {
        d = 0; degree = 0;
        endCoord = new();
        centerPoint = new();
        startCoord = effector.transform.position;
    }

    public float CalculateRadius()
    {
        Vector3 temp = (startCoord - centerPoint)/2; //Midpoint

        return temp.magnitude;
    }
}
