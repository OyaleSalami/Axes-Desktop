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
    [SerializeField] Text machineVariables;
    [SerializeField] GameObject displayPanel;

    [Header("NC File Code Path")]
    int fileIndex;
    public static List<string> fileLines;

    void Start()
    {
        Core.Init();
        CommandDefinitions.Init();
        fileLines = new List<string>();
    }

    void Update()
    {
        UpdateUI();
    }

    public void SelectFile()
    {
        string ncType = NativeFilePicker.ConvertExtensionToFileType("nc");
        NativeFilePicker.PickFile(LoadFile, ncType);
    }

    void LoadFile(string path)
    {
        if (File.Exists(path) != true)
        {
            Debug.Log("File does not exist!"); return;
        }
        else
        {
            string filename = Path.GetFileName(path);
            SetTitleText(filename);

            //Read Lines
            try
            {
                fileLines.AddRange(File.ReadAllLines(path));
                fileIndex = 0;
                SetCodeLine(fileLines[0]);
            }
            catch (Exception e)
            {
                Debug.Log("Error Reading file: " + e);
            }
        }
    }

    public void SetCodeLine(string line) => codeLine.text = line;

    public void SetTitleText(string title) => titleText.text = title;

    public void MinimizeDisplayPanel() => displayPanel.SetActive(!displayPanel.activeInHierarchy);

    public void NextLine()
    {
        fileIndex++;
        if (fileIndex >= fileLines.Count-1)
        {
            fileIndex = fileLines.Count - 1;
        }
        SetCodeLine(fileLines[fileIndex]);
    }

    public void PrevLine()
    {
        fileIndex--;
        if (fileIndex <= 0)
        {
            fileIndex = 0;
        }
        SetCodeLine(fileLines[fileIndex]);
    }

    public void UpdateUI()
    {
        machineVariables.text = "Spindle Speed: " + Core.spindleSpeed + "\n" +
                                "Feed Rate: " + Core.feedRate + "\n" +
                                "Dwell Time: " + Core.dwellTime + "\n";
    }
}
