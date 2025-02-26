using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    [SerializeField] GameObject origin; //Marker at the origin
    [SerializeField] GameObject xaxis; //Marker on the xaxis
    [SerializeField] GameObject yaxis; //Marker on the yaxis

    [SerializeField] GameObject x0;
    [SerializeField] GameObject y0;

    [SerializeField] Camera oCam; //orthogonalCam

    void Update()
    {
        if(Input.GetKey(KeyCode.G))
        {
            GenerateScaleNumbers();
        }
    }

    void GenerateScaleNumbers()
    {
        var o = oCam.WorldToScreenPoint(origin.transform.position);
        var x = oCam.WorldToScreenPoint(xaxis.transform.position);
        print(Vector3.Distance(o, x));
    }
}
