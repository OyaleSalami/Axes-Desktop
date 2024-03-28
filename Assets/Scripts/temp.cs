using UnityEngine;

public class temp : MonoBehaviour
{
    [SerializeField] GameObject drawHolder;
    [SerializeField] GameObject arcPrefab;
    [SerializeField] LineRenderer line;

    [SerializeField] TrailRenderer trail;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] Vector3 centerPoint;
    [SerializeField] float radius = 3;
    [SerializeField] float startA = 0;
    [SerializeField] float endA = 90;
    [SerializeField] float sweep;
    [SerializeField] int segments = 20;
    [SerializeField] bool clockwise = true;

    void Start()
    {
        //float startAngle = Mathf.Atan2(startCoord.y - endCoord.y, startCoord.x - endCoord.x) * Mathf.Rad2Deg;
        //float endAngle = Mathf.Atan2(endCoord.y - startCoord.y, endCoord.x - startCoord.x) * Mathf.Rad2Deg;

        //Vector3 pointOnCircle = centerPoint + startCoord + radius * new Vector3(Mathf.Cos(degree), 0, Mathf.Sin(degree));

        //effector.transform.position = Vector3.Lerp(effector.transform.position, pointOnCircle, m);
        //m += Time.deltaTime;
        //line.positionCount = 20; //20 segments
        line = Instantiate(arcPrefab, drawHolder.transform).GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Draw();
        }
    }

    public void Draw()
    {
        Debug.Log("Drawn!");
        float endAngle = endA; float startAngle = startA;
        sweep = endAngle - startAngle; //If negative = Clockwise //If positive = Anticlockwise

        if(sweep > 90)
        {
            startAngle += 360;
        }
        sweep = endAngle - startAngle;

        // Calculate angle step based on number of segments
        float angleStep = sweep / (segments - 1);

        // Create an array of Vector3 points for the arc
        Vector3[] points = new Vector3[segments];

        // Loop through segments and calculate positions
        for (int i = 0; i < segments; i++)
        {
            float angle = startAngle + (angleStep * i);
            float radians = Mathf.Deg2Rad * angle;
            points[i] = centerPoint + new Vector3(Mathf.Cos(radians) * radius, 0f, Mathf.Sin(radians) * radius);
        }

        // Set the positions of the Line Renderer
        line.positionCount = segments;
        line.SetPositions(points);

        /*
        Vector3 center = Vector3.zero;

        Vector3 pointOnCircle;

        while (angle <= 360)
        {
            pointOnCircle = center + (radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
            line.SetPosition(i, pointOnCircle);

            line.positionCount++; i++;
            angle += 5;                                                                                                         
        }

        
        for (int j = 0; j <= segments; j++)
        {
            Vector3 pointOnCircle = center + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Vector3 nextStep = new Vector3(centerPoint.x + Mathf.Cos(degree) * radius, 0, centerPoint.y + Mathf.Sin(degree) * radius);
            line.SetPosition(i, pointOnCircle);
        }
        */
        //float angle = i * angleStep;
    }

    public void CalculateCenterPoint()
    {
        float r = radius;
        float d = (end - start).magnitude;
        Vector3 center = (start + end) / 2;

        //First Circle (Negative r)
        Vector3 f;
        f.x = center.x + Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * ((start.y - end.y) / d));
        f.y = center.y + Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * ((end.x - start.x) / d));

        //Second Cirle (Positive r)
        Vector3 s;
        s.x = center.x - Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * (start.y - end.y) / d);
        s.y = center.y - Mathf.Sqrt(Mathf.Pow(r, 2) - Mathf.Pow(d / 2, 2) * (end.x - start.x) / d);
    }

    /// <summary>Returns the degree of the angle between these 2 vectors in degrees</summary>
    public float CalculateAngle(Vector3 start, Vector3 end, Vector3 cp)
    {
        Vector3 a = start - cp;
        Vector3 b = end - cp;

        return Mathf.Acos(Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
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

}

///Equation of a circle
/*
 * (x−h)**2 + (y−k)**2 = r**2
 * 
 * eg (3,2) 5
 * (x-3)**2 + (y-2)**2 = 25
 * 
 * centerpoint (h, k)
 * radius r
 */

//lineRenderer.positionCount = segments + 1;

////Arc points using circle formula
//float angleStep = Mathf.PI * 2 / segments;
//Vector3 center = Vector3.Lerp(start, end, 0.5f); //Centerpoint
//Vector3 direction = (end - start).normalized; //Initial direction

//for (int i = 0; i <= segments; i++)
//{
//    float angle = i * angleStep;
//    Vector3 pointOnCircle = center + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
//    lineRenderer.SetPosition(i, pointOnCircle);
//}

/*
 * Center Point here should be given in absolute terms
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
*/