using System;
using System.Collections.Generic;

namespace AxesCore
{
    public class Parser
    {
        public static void InterpretTokens(List<string> tokens)
        {
            if (tokens.Count < 0) return;
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i] = tokens[i].ToLower();

                if (tokens[i].ToLower().StartsWith('g')) //Preparatory Instruction
                {
                    if (CommandDefinitions.gModes.ContainsKey(tokens[i]))
                    {
                        CoreEngine.SetGMode(CommandDefinitions.gModes[tokens[i]]); //Set the mode in the core engine
                    }
                }
                else if (tokens[i].ToLower().StartsWith('s'))
                {
                    if (CommandDefinitions.mModes.ContainsKey(tokens[i]))
                    {
                        CoreEngine.SetMMode(CommandDefinitions.mModes[tokens[i]]); //Set the mode in the core engine
                    }
                }
                else if (tokens[i].ToLower().StartsWith('p'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float p = 0;
                    if (float.TryParse(temp, out p) != true)
                    {
                        throw new Exception("Unable to convert to float");
                    }
                    CoreEngine.SetDwellTime(p);
                }
                else if (tokens[i].ToLower().StartsWith('f'))
                {
                    string temp = tokens[i];
                    temp.Remove(0);
                    float f = 0;
                    if (float.TryParse(temp, out f) != true)
                    {
                        throw new Exception("Unable to convert to float");
                    }
                    CoreEngine.SetFeedRate(f);
                }
                else if (tokens[i].ToLower().StartsWith('s'))
                {
                    string temp = tokens[i];
                    temp.Remove(0);
                    float s = 0;
                    if (float.TryParse(temp, out s) != true)
                    {
                        throw new Exception("Unable to convert to float");
                    }
                    CoreEngine.SetSpindleSpeed(s);
                }
                else if (tokens[i].StartsWith('x'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float x = 0;
                    if (float.TryParse(temp, out x) != true) throw new Exception("Unable to convert to float");
                    Core.coords["x"] = x;
                    CoreEngine.SetCoord();
                }
                else if (tokens[i].StartsWith('y'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float y = 0;
                    if (float.TryParse(temp, out y) != true) throw new Exception("Unable to convert to float");
                    Core.coords["y"] = y;
                    CoreEngine.SetCoord();
                }
                else if (tokens[i].StartsWith('z'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float z = 0;
                    if (float.TryParse(temp, out z) != true) throw new Exception("Unable to convert to float");
                    Core.coords["z"] = z;
                    CoreEngine.SetCoord();
                }
                else if (tokens[i].StartsWith('a'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float a = 0;
                    if (float.TryParse(temp, out a) != true) throw new Exception("Unable to convert to float");
                    Core.coords["a"] = a;
                    CoreEngine.SetCoord();
                }
                else if (tokens[i].StartsWith('b'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float b = 0;
                    if (float.TryParse(temp, out b) != true) throw new Exception("Unable to convert to float");
                    Core.coords["b"] = b;
                    CoreEngine.SetCoord();
                }
                else if (tokens[i].StartsWith('c'))
                {
                    string temp = tokens[i]; temp.Remove(0);
                    float c = 0;
                    if (float.TryParse(temp, out c) != true) throw new Exception("Unable to convert to float");
                    Core.coords["a"] = c;
                    CoreEngine.SetCoord();
                }
                else
                {
                    throw new Exception("Unknown NC Code");
                }
                Core.mode = CoreMode.waiting;
            }
        }
    }

    /// <summary>Represents A Line Of GCode</summary>
    public class Block
    {
        public string line;
        public BlockType type { get; }

        public Block(string _line)
        {
            if (line == null || line == "") type = BlockType.EOB;

            line = _line;
        }

        /// <summary>Returns a list of tokens from the line</summary>
        /// <returns></returns>
        public List<string> Tokenize()
        {
            List<string> tokens = new();
            //Split the block according to the tokens
            tokens.AddRange(line.Split(' '));

            tokens.RemoveAll(IsASpace);
            tokens.RemoveAll(IsAComment);

            //tokens.RemoveAll((string s) => { return s == " "; } );

            return tokens;
        }

        private static bool IsASpace(String s)
        {
            return s == " ";
        }

        private static bool IsAComment(String s)
        {
            return s.StartsWith('(');
        }
    }

    public enum BlockType
    {
        None,
        EOB,
    }
}

