using UnityEngine;

public class temp : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] TrailRenderer trail;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] Vector3 centerPoint;
    [SerializeField] float radius;
    public int segments = 20;

    void Start()
    {
        //float startAngle = Mathf.Atan2(startCoord.y - endCoord.y, startCoord.x - endCoord.x) * Mathf.Rad2Deg;
        //float endAngle = Mathf.Atan2(endCoord.y - startCoord.y, endCoord.x - startCoord.x) * Mathf.Rad2Deg;

        //Vector3 pointOnCircle = centerPoint + startCoord + radius * new Vector3(Mathf.Cos(degree), 0, Mathf.Sin(degree));

        //effector.transform.position = Vector3.Lerp(effector.transform.position, pointOnCircle, m);
        //m += Time.deltaTime;
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