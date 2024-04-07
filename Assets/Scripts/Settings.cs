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

    void Awake()
    {
        LoadDefaults(); UpdateUI();
    }

    /// <summary>Set the default values of the settings if they do not exists</summary>
    void LoadDefaults()
    {
        if (PlayerPrefs.HasKey("speed") == false)
        {
            PlayerPrefs.SetInt("speed", 10);
        }
        if(PlayerPrefs.HasKey("velocity") == false)
        {
            PlayerPrefs.SetInt("velocity", 1000);
        }
        if(PlayerPrefs.HasKey("feedrate") == false)
        {
            PlayerPrefs.SetInt("feedrate", 15);
        }
    }

    void UpdateUI()
    {
        int speed = PlayerPrefs.GetInt("speed");
        int feedrate = PlayerPrefs.GetInt("feedrate");
        int velocity = PlayerPrefs.GetInt("velocity");

        speedInput.text = PlayerPrefs.GetInt("speed", speed).ToString();
        feedrateInput.text = PlayerPrefs.GetFloat("feedrate", feedrate).ToString();
        maxVelocityInput.text = PlayerPrefs.GetFloat("velocity", velocity).ToString();
    }

    public void SetSpeed()
    {
        if (Int32.Parse(speedInput.text) > 200)
        {
            speedInput.text = "200";
        }

        if (Int32.Parse(speedInput.text) <= 0)
        {
            speedInput.text = "1";
        }

        int speed = Int32.Parse(speedInput.text);
        PlayerPrefs.SetInt("speed", speed);
        ErrorHandler.Log("Set Simulator Speed: " + speed);
    }

    public void SetVelocity()
    {
        if (int.Parse(maxVelocityInput.text) >= 20000)
        {
            maxVelocityInput.text = "20000";
        }

        if (int.Parse(maxVelocityInput.text) <= 0)
        {
            maxVelocityInput.text = "1000"; //Default velocity
        }

        int velocity = Int32.Parse(maxVelocityInput.text);
        PlayerPrefs.SetInt("velocity", velocity);
        ErrorHandler.Log("Set Velocity: " + velocity);
    }

    public void SetFeedrate()
    {
        if (int.Parse(feedrateInput.text) >= 10000)
        {
            feedrateInput.text = "10000";
        }

        if (int.Parse(feedrateInput.text) <= 0)
        {
            feedrateInput.text = "15";
        }

        int feedrate = int.Parse(feedrateInput.text);
        Core.feedRate = feedrate;
        PlayerPrefs.SetInt("feedrate", feedrate);
        ErrorHandler.Log("Set Feedrate: " + feedrate);
    }
}
