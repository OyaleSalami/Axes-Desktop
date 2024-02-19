using System;
using System.Collections.Generic;

namespace AxesCore
{
    public class Parser
    {
        /// <summary> Interprets the block and sets the correct parameters on the Core/Core Engine</summary>
        public static void InterpretTokens(List<string> tokens)
        {
            //Exit if the token list is empty
            if(tokens.Count < 1) { ErrorHandler.Log("Empty Token list"); return; }

            //Prepare Core to take new coordinates
            CoreEngine.ResetCoord();

            foreach (string token in tokens)
            {
                ErrorHandler.Log("Interpreting token : " + token);
                switch (token.ToLower()[0])
                {
                    case 'g': //Preparatory Instructions
                        try
                        {
                            //Call the appropriate function to handle the instruction
                            CommandDefinitions.opHandlers[CommandDefinitions.gModes[token]]();
                        }
                        catch (Exception e) 
                        {
                            ErrorHandler.Error("Unhandled gcode: " + e.ToString());
                        }
                        break;

                    case 'm': //Set any extra mode
                        //CoreEngine.SetMMode(CommandDefinitions.mModes[token]);
                        ErrorHandler.Log("MCode: " + token);
                        break;

                    case 't': //Set the current tool
                        //CoreEngine.SetTool(CommandDefinitions.tools[token]);
                        ErrorHandler.Log("MCode: " + token);
                        break;

                    case 'h': //Set the h coordinate
                        CoreEngine.SetToolHeight(ReadValue(token));
                        break;

                    case 'd': //Set the d coordinate
                        Core.coord.d = ReadValue(token);
                        break;

                    case 'q': //Set the q coordinate
                        Core.coord.q = ReadValue(token);
                        break;

                    case 'p': //Set the P coordinate
                        Core.coord.p = ReadValue(token);
                        break;

                    case 'f': //Set the F coordinate
                        Core.coord.f = ReadValue(token);
                        break;
                    
                    case 's': //Set Spindle Speed
                        Core.coord.s = ReadValue(token);
                        break;

                    case 'x': //Set the x coordinate
                        Core.coord.x = ReadValue(token);
                        break;

                    case 'y': //Set the y coordinate
                        Core.coord.y = ReadValue(token);
                        break;

                    case 'z': //Set the z coordinate
                        Core.coord.z = ReadValue(token);
                        break;

                    case 'a': //Set the a coordinate
                        Core.coord.a = ReadValue(token);
                        break;

                    case 'b': //Set the b coordinate
                        Core.coord.b = ReadValue(token);
                        break;

                    case 'c': //Set the c coordinate
                        Core.coord.c = ReadValue(token);
                        break;

                    case 'r': //Set the r coordinate
                        Core.coord.r = ReadValue(token);
                        break;

                    case 'i': //Set the i coordinate
                        Core.coord.i = ReadValue(token);
                        break;

                    case 'j': //Set the j coordinate
                        Core.coord.j = ReadValue(token);
                        break;

                    case 'k': //Set the k coordinate
                        Core.coord.k = ReadValue(token);
                        break;

                    case '%': //End of the program
                        Core.mode = CoreMode.EOF;
                        break;

                    default:
                        ErrorHandler.Error("Undefined token: " + token);
                        break;
                }
            }

            //Handle the coordinates set by the parameters above
            CoreEngine.HandleCoordinates();
        }
    
        private static float ReadValue(string token)
        {
            float f = 0;
            string temp = token.Remove(0, 1); //Remove the first letter

            if (float.TryParse(temp, out f) != true)
            {
                ErrorHandler.Error("Unable to convert float: " + token);
                throw new Exception("Unable to convert to float");
            }

            return f;
        }
    }

    /// <summary>Represents A Line Of GCode</summary>
    public class Block
    {
        /// <summary>The text of that GCode line</summary>
        public string line;

        /// <summary>Describes the block as a comment or not</summary>
        public bool isAComment = false;

        public Block(string _line) 
        { 
            line = _line; 

            //NOTE: Temporary line here
            if(line.StartsWith('(') == true || line.Length == 0 || line.StartsWith('\n'))
            {
                isAComment = true;
            }
        }

        ///<summary>Returns the list of tokens(words) from a block</summary>\
        public List<string> Tokenize()
        {
            List<string> tokens = new();

            //Split the block using space as a delimitter
            tokens.AddRange(line.Split(' ')); 

            tokens.RemoveAll(IsASpace);

            return tokens;
        }

        /// <returns>True if the string is a space</returns>
        private static bool IsASpace(String s)
        {
            return s == " ";
        }

        /// <returns>True if the string is a comment</returns>
        private static bool IsAComment(String s)
        {
            //TODO: Discard all lines that are comments
            return s.StartsWith('(');
        }
    }
}