using UnityEngine;

public class temp : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] float radius;
    public int segments = 20;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Draw();
        }
    }

    public void Draw()
    {
        lineRenderer.positionCount = segments + 1;

        //Arc points using circle formula
        float angleStep = Mathf.PI * 2 / segments;
        Vector3 center = Vector3.Lerp(start, end, 0.5f); //Centerpoint
        Vector3 direction = (end - start).normalized; //Initial direction

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 pointOnCircle = center + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            lineRenderer.SetPosition(i, pointOnCircle);
        }
    }
}
