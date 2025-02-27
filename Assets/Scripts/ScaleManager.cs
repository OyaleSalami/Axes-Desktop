using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleManager : MonoBehaviour
{
    [Tooltip("The number of grid elements to calculate")]
    [SerializeField] int gridCount = 100;
    [SerializeField] GameObject numPrefab; //Prefab of the grid number object

    [SerializeField] GameObject origin; //Marker at the origin
    [SerializeField] GameObject xaxis; //Marker on the xaxis
    [SerializeField] GameObject yaxis; //Marker on the yaxis

    [SerializeField] GameObject x0;
    [SerializeField] GameObject y0;
    [SerializeField] GameObject xHolder;
    [SerializeField] GameObject yHolder;

    [SerializeField] Camera oCam; //orthogonalCam

    private void Start()
    {
        GenerateScaleNumbers();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.G))
        {
            GenerateScaleNumbers();
        }
    }

    public void GenerateScaleNumbers()
    {
        // Clear the previous numbers
        for(int i = 0; i < xHolder.transform.childCount; i++)
        {
            if(xHolder.transform.GetChild(i).gameObject != x0)
            {
                Destroy(xHolder.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < yHolder.transform.childCount; i++)
        {
            if (yHolder.transform.GetChild(i).gameObject != y0)
            {
                Destroy(yHolder.transform.GetChild(i).gameObject);
            }
        }

        // Cast the points to the screen
        var o = oCam.WorldToScreenPoint(origin.transform.position);
        var x = oCam.WorldToScreenPoint(xaxis.transform.position);
        var y = oCam.WorldToScreenPoint(yaxis.transform.position);

        // Calculate the distance between grid numbers
        float distanceX = Vector3.Distance(o, x);
        float distanceY = Vector3.Distance(o, y);
        float xStart = x0.GetComponent<RectTransform>().position.x;
        float yStart = y0.GetComponent<RectTransform>().position.y;

        //Fill in new numbers (Positive)
        for (int i = 0; i < gridCount/2; i++)
        {
            //Generate positive x axis
            var temp = Instantiate(numPrefab, xHolder.transform);
            temp.GetComponent<RectTransform>().position = new Vector3(xStart + (distanceX * (i + 1)), x0.GetComponent<RectTransform>().position.y, 0);
            temp.GetComponent<Text>().text = (5 * (i + 1)).ToString();

            //Generate negative x axis
            temp = Instantiate(numPrefab, xHolder.transform);
            temp.GetComponent<RectTransform>().position = new Vector3(xStart - (distanceX * (i + 1)), x0.GetComponent<RectTransform>().position.y, 0);
            temp.GetComponent<Text>().text = (-5 * (i + 1)).ToString();

            //Generate positive y axis
            temp = Instantiate(numPrefab, yHolder.transform);
            temp.GetComponent<RectTransform>().position = new Vector3(y0.GetComponent<RectTransform>().position.x, yStart + (distanceY * (i + 1)), 0);
            temp.GetComponent<Text>().text = (5 * (i + 1)).ToString();

            //Generate negative y axis
            temp = Instantiate(numPrefab, yHolder.transform);
            temp.GetComponent<RectTransform>().position = new Vector3(y0.GetComponent<RectTransform>().position.x, yStart - (distanceY * (i + 1)), 0);
            temp.GetComponent<Text>().text = (-5 * (i + 1)).ToString();
        }
    }
}
