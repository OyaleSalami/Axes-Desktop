using System.Collections.Generic;
using UnityEngine;
using AxesCore;
using UnityEngine.UI;

public class CodeControl : MonoBehaviour
{
    [SerializeField] ArmControl arm;
    [SerializeField] int lineIndex = 0;
    [SerializeField] InputField codeLine;

    public void Update()
    {

    }

    public void ExecuteCode(string line)
    {
        line = codeLine.text;
        Block block = new Block(line);
        Parser.InterpretTokens(block.Tokenize()); //Interprets the blocks and sets the correct parameters
        arm.Draw();
    }
}
