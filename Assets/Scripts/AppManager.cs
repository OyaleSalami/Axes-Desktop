using AxesCore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text codeLine;
    [SerializeField] Text titleText;
    [SerializeField] Text dataLog;
    [SerializeField] Text machineVariables;
    [SerializeField] GameObject displayPanel;

    [Header("NC Code File Path")]
    int fileIndex;
    public static List<string> fileBuffer;
    public CodeControl codeController;

    public static LoadMode loadMode;

    void Start()
    {
        Core.Init();
        ErrorHandler.Init();
        CommandDefinitions.Init();
        fileBuffer = new List<string>();
        UpdateUI();
    }

    /// <summary>Brings up the context menu to select an NC file</summary>
    public void SelectFile()
    {
        string ncType = NativeFilePicker.ConvertExtensionToFileType("nc"); //Define the extension
        NativeFilePicker.PickFile(LoadFile, ncType); //Bring up the context menu
    }

    /// <summary>Attempts to load the selected file into a buffer</summary>
    void LoadFile(string path)
    {
        if (File.Exists(path) != true) //No file was picked
        {
            ErrorHandler.Error("File does not exist!");
            return;
        }
        else
        {
            try //Read the lines of the file
            {
                fileIndex = 0;
                fileBuffer.AddRange(File.ReadAllLines(path));
            }
            catch (Exception e) //An error occured
            {
                ErrorHandler.Error("Error Reading file: " + e);
            }

            loadMode = LoadMode.loaded;
            string filename = Path.GetFileName(path); //Get the name of the file
            SetTitleText(filename); //Set the tile of the window to the name of the file
            codeController.ExecuteFile(); //Start executing the file
        }
    }

    void UnLoadFile()
    {
        SetTitleText("Axes");
        fileBuffer = new List<string>();
        fileIndex = 0;
        loadMode = LoadMode.unloaded;
    }

    /// <summary>Sets the title for the window </summary>
    public void SetTitleText(string title) => titleText.text = title;

    /// <summary>Hides the display panel</summary>
    public void MinimizeDisplayPanel() => displayPanel.SetActive(!displayPanel.activeInHierarchy);

    /// <summary>
    /// Updates the UI for the 
    /// * Machine Variables
    /// * Debug Handler
    /// </summary>
    public void UpdateUI()
    {
        dataLog.text = "";
        foreach (var item in ErrorHandler.logs)
        {
            dataLog.text += "\n" + item;
        }

        foreach (var item in ErrorHandler.errors)
        {
            dataLog.text += "\n" + "<color=red>" + item + "</color>";
        }

        machineVariables.text = "Spindle Speed: " + Core.spindleSpeed + "\n" +
                                "Feed Rate: " + Core.feedRate + "\n" +
                                "Dwell Time: " + Core.dwellTime + "\n" +
                                "Position Mode: " + Core.positionMode + "\n" +
                                "Fixture Offset: " + Core.fixtureOffset + "\n" +
                                "Arc Mode: " + Core.arcMode + "\n" +
                                "Exact Stop: " + Core.exactStop + "\n" +
                                "Plane Select: " + Core.planeMode + "\n" +
                                "Core Mode: " + Core.mode + "\n" +
                                "UPM: " + Core.upm;

        Invoke(nameof(UpdateUI), 1f); //Update UI every second not every frame   
    }
}