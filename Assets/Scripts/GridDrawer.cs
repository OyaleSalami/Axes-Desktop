using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public Color lineColor;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //Calculate Total Number of vertices neeed
        int vertexCount = (gridWidth + 1) * (gridHeight + 1);
        lineRenderer.positionCount = vertexCount;

        //Generate Grid Vertices
        int index = 0;

        for (int y = 0; y <= gridHeight; y++)
        {
            for (int x = 0; x <= gridWidth; x++)
            {
                float xPos = x * cellSize;
                float yPos = y * cellSize;
                lineRenderer.SetPosition(index, new Vector3(xPos, yPos, 0));
                index++;
            }
        }

        //Draw Horizontal Lines
        for (int y = 0; y <= gridHeight; y++)
        {
            int startIndex = y * (gridWidth + 1);
            int endIndex = startIndex + gridWidth;
            for (int i = startIndex; i < endIndex; i++)
            {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) + new Vector3(cellSize, 0, 0));
            }
        }

        //Draw Vertical Lines
        for (int x = 0; x <= gridHeight; x++)
        {
            int startIndex = x;
            int endIndex = startIndex + (gridHeight + 1) * gridWidth;
            for (int i = startIndex; i < endIndex; i += gridWidth + 1)
            {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i) + new Vector3(0, cellSize, 0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
