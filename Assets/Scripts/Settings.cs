using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] public static float speed;
    [SerializeField] public static float feedrate;
    [SerializeField] public static float maxVelocity;

    [Header("UI Elements")]
    [SerializeField] InputField speedInput;
    [SerializeField] InputField feedrateInput;
    [SerializeField] InputField maxVelocityInput;

    // Start is called before the first frame update
    void Start()
    {
        Load(); UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Load()
    {
        speed = PlayerPrefs.GetInt("speed_settings", 1);
        feedrate = PlayerPrefs.GetFloat("feedrate_settings", 15);
        maxVelocity = PlayerPrefs.GetFloat("maxvelocity_settings", 1000);
    }

    void UpdateUI()
    {
        speedInput.text = speed.ToString();
        feedrateInput.text = feedrate.ToString();
    }

    public void SetValue(string key, int value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}
