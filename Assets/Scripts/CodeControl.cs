using AxesCore;
using UnityEngine;
using UnityEngine.UI;

public class CodeControl : MonoBehaviour
{
    [SerializeField] ArmControl arm;
    [SerializeField] int lineIndex = 0;

    public void Update()
    {
        if (Core.mode == CoreMode.done && AppManager.loadMode == LoadMode.loaded)
        {
            if (lineIndex >= AppManager.fileBuffer.Count - 1)
            {
                Core.mode = CoreMode.EOF; //End Of File
                return; //Don't do anything
            }
            else //Move to the next line of the file
            {
                lineIndex++;
                ErrorHandler.Log("Line: " + (lineIndex + 1));
                ExecuteFile();
            }
        }
    }

    public void ExecuteFile()
    {
        Core.mode = CoreMode.start;
        ExecuteCode(AppManager.fileBuffer[lineIndex]);
    }

    /// <summary>Executes a single line of NC code </summary>
    public void ExecuteCode(string line)
    {
        Block block = new Block(line);
        if (block.isAComment == true)
        {
            //Skip the comment line
            Core.mode = CoreMode.done;
        }
        else
        {
            //Interprets the block and sets the correct parameters
            Parser.InterpretTokens(block.Tokenize());
        }
    }
}
