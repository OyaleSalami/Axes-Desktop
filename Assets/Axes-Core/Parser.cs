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
            if(tokens.Count < 1) { ErrorHandler.Error("Empty Token List"); return; }

            //Prepare Core to take new coordinates
            CoreEngine.ResetCoord();

            //Cycle through all the tokens and set the parameters and call the appropriate functions
            foreach (string token in tokens)
            {
                ErrorHandler.Log("Interpreting token : " + token);
                switch (token.ToLower()[0]) //Match the first letter of the token
                {
                    case 'g': //Preparatory Instructions
                        try //Call the appropriate function to handle the instruction
                        {
                            CommandDefinitions.opHandlers[CommandDefinitions.gModes[token]]();
                        }
                        catch (Exception e) 
                        { 
                            ErrorHandler.Error("Unhandled gcode: " + e.ToString()); 
                        }
                        break;

                    case 'm': //Set up any extra mode
                        //CoreEngine.SetMMode(CommandDefinitions.mModes[token]);
                        break;

                    case 't': //Set the current tool
                        //CoreEngine.SetTool(CommandDefinitions.tools[token]);
                        break;

                    case 'h': //Set the h coordinate
                        Core.coord.h = ReadValue(token);
                        break;

                    case 'l': //Set the l coordinate
                        Core.coord.l = ReadValue(token);
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
    
        /// <summary>Takes a token and reads the value specified after it</summary>
        private static float ReadValue(string token)
        {
            string temp = token.Remove(0, 1); //Remove the first letter

            if (float.TryParse(temp, out float f) != true)
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

        /// <summary>Inavlid Block(Comment, Empty Block, ...)</summary>
        public bool isNotValid = false;

        public Block(string _line) 
        { 
            line = _line; 

            if(line.StartsWith('(') == true || line.Length == 0 || line.StartsWith('\n'))
            {
                isNotValid = true; //Set the line as invalid
            }
        }

        ///<summary>Returns the list of tokens(words) from a block</summary>\
        public List<string> Tokenize()
        {
            List<string> tokens = new();

            tokens.AddRange(line.Split(' '));  //Split the block using space as a delimitter
            tokens.RemoveAll(IsASpace); //Remove all the extra spaces from the code
            return tokens;
        }

        /// <returns>True if the string is just a space</returns>
        private static bool IsASpace(String s)
        {
            return s == " ";
        }
    }

}