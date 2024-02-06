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
                ErrorHandler.Log("Empty Token list: " + tokens);
                return;
            }

            CoreEngine.ResetCoord();

            foreach (string token in tokens)
            {
                switch (token.ToLower()[0])
                {
                    case 'g': //Preparatory Instructions
                        try
                        {
                            //Call the appropriate function to handle the instruction
                            CommandDefinitions.opHandlers[CommandDefinitions.gModes[token]]();

                            switch (CommandDefinitions.gModes[token])
                            {
                                case GMode.G59:
                                    CoreEngine.SetCoordMode(CoordMode.fixtureOffset);
                                    break;

                                case GMode.G541:
                                    CoreEngine.SetCoordMode(CoordMode.addFixtureOffset);
                                    break;

                                default:
                                    CoreEngine.SetCoordMode(CoordMode.draw);
                                    break;
                            }
                        }
                        catch (Exception e) 
                        {
                            ErrorHandler.Error("Unhandled gcode: " + e.ToString());
                        }
                        break;

                    case 'm': //Set any extra mode
                        CoreEngine.SetMMode(CommandDefinitions.mModes[token]);
                        break;

                    case 't': //Set the current tool
                        CoreEngine.SetTool(CommandDefinitions.tools[token]);
                        break;

                    case 'd': //Set tool diameter
                        CoreEngine.SetToolDiameter(ReadValue(token));
                        break;

                    case 'h': //Set tool diameter
                        CoreEngine.SetToolHeight(ReadValue(token));
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

                    default:
                        ErrorHandler.Error("Undefined token: " + token);
                        break;
                }
            }

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
        public string line;

        public Block(string _line) 
        { 
            line = _line; 
        }

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