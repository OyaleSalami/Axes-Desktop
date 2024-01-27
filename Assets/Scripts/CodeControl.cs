using System.Collections.Generic;
using UnityEngine;
using AxesCore;
using UnityEngine.UI;

public class CodeControl : MonoBehaviour
{
    [SerializeField] ArmControl arm;
    [SerializeField] int lineIndex = 0;

    public void Update()
    {
        if(Core.mode == CoreMode.pre)
        {
            ExecuteCode(AppManager.fileLines[lineIndex]);
        }

        if(Core.mode == CoreMode.waiting)
        {

        }

        if(Core.mode == CoreMode.done)
        {
            lineIndex++; //Perform bounds checking
            ExecuteCode(AppManager.fileLines[lineIndex]);
        }
    }

    public void ExecuteCode(string line)
    {
        Block block = new Block(line);
        Parser.InterpretTokens(block.Tokenize()); //Interprets the blocks and sets the correct parameters
    }
}
