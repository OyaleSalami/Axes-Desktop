using UnityEngine;
using AxesCore;

public class ArmControl: MonoBehaviour
{
    [SerializeField] bool running;

    [Header("Running Parameters")]
    Vector3 startCoord;
    Vector3 endCoord;
    [SerializeField] OperationType opType;

    float timer = 0;

    void Start()
    {
        running = false;
    }

    void Update()
    {
        switch (opType) 
        {
            case OperationType.RapidMove:
                transform.position = Vector3.Slerp(transform.position, startCoord, (timer + (Core.feedRate/60)) * Time.deltaTime);
                if (Vector3.Equals(transform.position, startCoord)) running = false;
                break;

            case OperationType.LinearFeedMove:
                transform.position = Vector3.Slerp(transform.position, endCoord, (timer + (Core.feedRate / 60)) * Time.deltaTime);
                if (Vector3.Equals(transform.position, endCoord)) running = false;
                break;


            default:
                break;
        }
    }

    public void RapidMove(Coord coord)
    {
        startCoord = new Vector3(coord.x, coord.z, coord.y);

        //TODO: Stop Drawing Lines Instead
        GetComponent<TrailRenderer>().enabled = false; 
        opType = OperationType.RapidMove;
    }

    public void LinearFeedMove(Coord start, Coord end)
    {
        startCoord = new Vector3(start.x, start.z, start.y);
        endCoord = new Vector3(end.x, end.z, end.y);

        GetComponent<TrailRenderer>().enabled = true; //TODO: Draw Lines Instead
        opType = OperationType.LinearFeedMove;
    }
}
