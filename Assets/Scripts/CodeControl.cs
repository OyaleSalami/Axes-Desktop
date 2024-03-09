using AxesCore;
using UnityEngine;
using UnityEngine.UI;

public class CodeControl : MonoBehaviour
{
    [SerializeField] ArmControl arm;
    [SerializeField] int lineIndex = 0;

    public void Update()
    {
        if (Core.mode == CoreMode.normal && AppManager.loadMode == LoadMode.loaded)
        {
            if (lineIndex >= AppManager.fileBuffer.Count - 1)
            {
                Core.mode = CoreMode.EOF; //End Of File
                ErrorHandler.Log("End Of File");
                return; //Don't do anything
            }
            else //Move to the next line of the file
            {
                ErrorHandler.Log("Line: " + (lineIndex + 1));
                ExecuteCode(AppManager.fileBuffer[lineIndex]);
                lineIndex++;
            }
        }
    }

    /// <summary>Executes a single line of NC code </summary>
    public void ExecuteCode(string line)
    {
        Block block = new Block(line);
        if (block.isNotValid == true)
        {
            Core.mode = CoreMode.normal;
        }
        else
        {
            //Interprets the block and sets the correct parameters for the Core
            Parser.InterpretTokens(block.Tokenize());
        }
    }
}
