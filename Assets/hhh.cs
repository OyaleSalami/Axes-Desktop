using System.Collections;
using SFB;
using System.Collections.Generic;
using UnityEngine;

public class hhh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectFile()
    {
        SFB.StandaloneFileBrowser.OpenFilePanel("Select", null, ".nc", false);
    }
}
