using System;
using System.Collections.Generic;

namespace AxesCore
{
    public class Parser
    {
        public static void InterpretTokens(List<string> tokens)
        {
            if(tokens.Count < 1)
            {
                ErrorHandler.Log("Empty Token list");
                return;
            }

            CoreEngine.ResetCoord();

            foreach (string token in tokens)
            {
                switch (token.ToLower()[0])
                {
                    case 'g': //Preparatory Instruction
                        //Call the appropriate function to handle the instruction
                        CommandDefinitions.opHandlers[CommandDefinitions.gModes[token]]();
                        break;

                    case 'p': //Set Dwell Time
                        CoreEngine.SetDwellTime(ReadValue(token));
                        break;

                    case 'f': //Set feed rate
                        CoreEngine.SetFeedRate(ReadValue(token));
                        break;
                    
                    case 's': //Set Spindle Speed
                        CoreEngine.SetSpindleSpeed(ReadValue(token));
                        break;

                    case 'm': //Set any extra mode
                        CoreEngine.SetMMode(CommandDefinitions.mModes[token]);
                        break;

                    case 'x': //Set the x coordinate
                        Core.mode = CoreMode.drawStart;
                        Core.coord.c[0] = ReadValue(token);
                        break;

                    case 'y': //Set the y coordinate
                        Core.mode = CoreMode.drawStart;
                        Core.coord.c[1] = ReadValue(token);
                        break;

                    case 'z': //Set the z coordinate
                        Core.mode = CoreMode.drawStart;
                        Core.coord.c[2] = ReadValue(token);
                        break;

                    case 'a': //Set the a coordinate
                        Core.coord.c[3] = ReadValue(token);
                        break;

                    case 'b': //Set the b coordinate
                        Core.coord.c[4] = ReadValue(token);
                        break;

                    case 'c': //Set the c coordinate
                        Core.coord.c[5] = ReadValue(token);
                        break;

                    case 'r': //Set the r coordinate
                        Core.coord.c[6] = ReadValue(token);
                        break;

                    case 'i': //Set the i coordinate
                        Core.coord.c[7] = ReadValue(token);
                        break;

                    case 'j': //Set the j coordinate
                        Core.coord.c[8] = ReadValue(token);
                        break;

                    case 'k': //Set the k coordinate
                        Core.coord.c[9] = ReadValue(token);
                        break;

                    default:
                        ErrorHandler.Error("Undefined token: " + token);
                        break;
                }
            }

        }
    
        private static float ReadValue(string token)
        {
            float f = 0;
            string temp = token.Remove(0, 1); //Remove the first letter

            if (float.TryParse(temp, out f) != true)
            {
                ErrorHandler.Error("Unbale to convert float: " + temp);
                throw new Exception("Unable to convert to float");
            }

            return f;
        }
    }

    /// <summary>Represents A Line Of GCode</summary>
    public class Block
    {
        public string line;
        public Block(string _line) =>  line = _line;

        /// <summary>Returns the list of tokens from a block</summary>\
        public List<string> Tokenize()
        {
            List<string> tokens = new();
            tokens.AddRange(line.Split(' ')); //Split the block according to the tokens

            tokens.RemoveAll(IsASpace);
            tokens.RemoveAll(IsAComment);

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

}