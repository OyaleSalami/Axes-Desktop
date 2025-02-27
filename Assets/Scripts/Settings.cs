using AxesCore;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] InputField speedInput;
    [SerializeField] InputField feedrateInput;
    [SerializeField] InputField maxVelocityInput;

    [Header("Color Settings")]
    [SerializeField] Material lineMat;
    [SerializeField] Material imageMat;
    [SerializeField] Image displayImage;
    [SerializeField] Slider red;
    [SerializeField] Slider green;
    [SerializeField] Slider blue;

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        float speed = AppManager.appSettings.speed;
        float feedrate = AppManager.appSettings.defaultFeedrate;
        float velocity = AppManager.appSettings.maxFeedrate;

        speedInput.text = speed.ToString();
        feedrateInput.text = feedrate.ToString();
        maxVelocityInput.text = velocity.ToString();
    }

    public void SetSpeed()
    {
        if (float.Parse(speedInput.text) > 1000)
        {
            speedInput.text = "1000";
        }
        if (float.Parse(speedInput.text) <= 0)
        {
            speedInput.text = "1";
        }

        float speed = float.Parse(speedInput.text);
        AppManager.appSettings.SetSpeed(speed); UpdateUI();
        ErrorHandler.Log("Set Simulator Speed: " + speed);
    }

    public void SetVelocity()
    {
        if (float.Parse(maxVelocityInput.text) >= 20000)
        {
            maxVelocityInput.text = "20000";
        }
        if (float.Parse(maxVelocityInput.text) <= 0)
        {
            maxVelocityInput.text = "2000";
        }

        float velocity = float.Parse(maxVelocityInput.text);
        AppManager.appSettings.SetMaxFeedrate(velocity); UpdateUI();
        ErrorHandler.Log("Set Default Velocity: " + velocity);
    }

    public void SetFeedrate()
    {
        if (float.Parse(feedrateInput.text) >= 10000)
        {
            feedrateInput.text = "10000";
        }
        if (float.Parse(feedrateInput.text) <= 0)
        {
            feedrateInput.text = "50";
        }

        float feedrate = float.Parse(feedrateInput.text); 
        AppManager.appSettings.SetDefaultFeedrate(feedrate); Core.feedRate = feedrate;
        UpdateUI(); ErrorHandler.Log("Set Feedrate: " + feedrate);
    }

    //Color Related Functions

    public void SetNewColor()
    {
        float r = red.value;
        float g = green.value;
        float b = blue.value;

        imageMat.color = new Color(r, g, b);
        lineMat.color = new Color(r, g, b);
    }
}
