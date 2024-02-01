using UnityEngine;
using AxesCore;
using System.Linq;

public class ArmControl: MonoBehaviour
{
    [SerializeField] bool running;

    [Header("Effector Parameters")]
    [SerializeField] GameObject effector;

    [Header("Running Parameters")]
    public Vector3 startCoord;
    public Vector3 endCoord;
    bool draw = false;

    float timer = 0;

    void Start()
    {
        running = false;
    }

    static float t = 0f; //For Timing LERP operations

    void Update()
    {
        if(Core.mode == CoreMode.drawStart)
        {
            if(draw == true)
            {
                //Handle actual draw functions
            }
            else
            {
                Debug.Log("Drawing");
                effector.transform.position = Vector3.Lerp(startCoord, endCoord,t);
            }
            t += 0.5f * Time.deltaTime;
        }

        if(t >= 1.0f)
        {
            t = 0f;
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
                break;

            case GMode.G02:
                draw = true;
                break;

            case GMode.G03:
                draw = true;
                break;

            case GMode.G04:
                draw = true;
                break;
            case GMode.G05:
                break;
            case GMode.G06:
                break;
            case GMode.G07:
                break;
            case GMode.G08:
                break;
            case GMode.G09:
                break;
            case GMode.G10:
                break;

            default:
                break;
        }
    }

    public void SetCoords()
    {
        if (Core.positionMode == PositionMode.absolute)
        {
            startCoord = effector.transform.position;
            endCoord = new Vector3(Core.coord.c[0], Core.coord.c[2], Core.coord.c[1]);
        }
        else if (Core.positionMode == PositionMode.incremental)
        {
            startCoord = effector.transform.position;
            endCoord = startCoord + new Vector3(Core.coord.c[0], Core.coord.c[2], Core.coord.c[1]);
        }
        else
        {
            ErrorHandler.Error("Inavlid Position Mode: " + Core.positionMode);
        }
    }
}
