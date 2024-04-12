using AxesCore;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppSettings appSettings;

    [Header("UI")]
    [SerializeField] Text codeLine;
    [SerializeField] Text titleText;
    [SerializeField] Text machineVariables;
    [SerializeField] GameObject displayPanel;
    [SerializeField] GameObject grid;

    [Header("NC Code File Path")]
    public static List<string> fileBuffer;
    public CodeControl codeController;
    public static LoadMode loadMode = LoadMode.unloaded;

    void Start()
    {
        appSettings = new();
        appSettings.Load();
        CommandDefinitions.Init();
        Core.Init();
        ErrorHandler.Init();
        UnLoadFile();
        UpdateUI();
    }

    /// <summary>Brings up the context menu to select an NC file</summary>
    public void SelectFile()
    {
        //Reset the buffer to make sure there is no left over file when restarting
        UnLoadFile();

        // Create filter
        var extensions = new[] { new ExtensionFilter("NC File", "nc"), new ExtensionFilter("NC File", "txt") };

        //Open the context menu
        var path = StandaloneFileBrowser.OpenFilePanel("Select GCode File", "", extensions, false);

        if (path.Length > 0)
        {
            LoadFile(path);
        }
    }

    /// <summary>Attempts to load the selected file into a buffer</summary>
    void LoadFile(string[] path)
    {
        if (File.Exists(path[0]) != true) //No valid file path was picked
        {
            ErrorHandler.Error("File does not exist!");
            return;
        }
        else
        {
            try //Read the lines of the file
            {
                fileBuffer.AddRange(File.ReadAllLines(path[0]));
            }
            catch (Exception e) //An error occured
            {
                ErrorHandler.Error("Error Reading file: " + e);
                return;
            }

            string fileName = Path.GetFileName(path[0]); //Get the name of the file
            SetTitleText("Axes - " + fileName); //Set the tile of the window to the name of the file
            loadMode = LoadMode.loaded; //Set the file loaded mode
            ErrorHandler.Log(("[File]: " + fileName + " " + loadMode));
        }
    }

    void UnLoadFile()
    {
        SetTitleText("Axes"); //Change back the title of the window
        fileBuffer = new List<string>(); //Empty the file buffer
        loadMode = LoadMode.unloaded; //Set the file mode to unloaded
    }

    /// <summary>Sets the title for the window </summary>
    public void SetTitleText(string title) => titleText.text = title;

    /// <summary>Hides the display panel</summary>
    public void MinimizeDisplayPanel() => displayPanel.SetActive(!displayPanel.activeInHierarchy);

    /// <summary> Updates the UI for the Core Variables and Debug Handler </summary>
    public void UpdateUI()
    {
        codeLine.text = ErrorHandler.current;

        //Update the text for the Core variables
        machineVariables.text = "Spindle Speed: " + Core.spindleSpeed + "\n" +
                                "Feed Rate: " + Core.feedRate + "\n" +
                                "Dwell Time: " + Core.dwellTime + "\n" +
                                "Position Mode: " + Core.positionMode + "\n" +
                                "Fixture Offset: " + Core.fixtureOffset + "\n" +
                                "Arc Mode: " + Core.arcMode + "\n" +
                                "Exact Stop: " + Core.exactStop + "\n" +
                                "Plane Select: " + Core.planeMode + "\n" +
                                "Core Mode: " + Core.mode + "\n" +
                                "Group 1 Mode: " + Core.group[1] + "\n" +
                                "UPM: " + Core.upm;

        Invoke(nameof(UpdateUI), 1f); //Update UI every second not every frame (Sort of a delayed recursive loop)   
    }

    public void OpenLog()
    {
        Application.OpenURL(ErrorHandler.filePath); //Try to open the log file in notepad or something
    }

    public void ToggleGrid()
    {
        grid.SetActive(!grid.activeSelf);
    }

    public void Quit()
    {
        Application.Quit();
    }
}