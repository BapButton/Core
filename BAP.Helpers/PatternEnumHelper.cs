using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;

namespace BAP.Helpers
{


    public static class PaternEnumHelper
    {
        public static int GetNumberFromEnum(Patterns pattern)
        {
            switch (pattern)
            {
                case Patterns.Number0: return 0;
                case Patterns.Number1: return 1;
                case Patterns.Number2: return 2;
                case Patterns.Number3: return 3;
                case Patterns.Number4: return 4;
                case Patterns.Number5: return 5;
                case Patterns.Number6: return 6;
                case Patterns.Number7: return 7;
                case Patterns.Number8: return 8;
                case Patterns.Number9: return 9;
                default:
                    throw new Exception($"Invalid pattern. It must be one of the number patterns but {pattern.ToString()} was passed in.");
            }
        }

        public static Patterns GetPatternFromNumber(int number)
        {
            if (number == 0)
            {
                return Patterns.Number0;
            }
            if (number < 99)
            {
                return (Patterns)number;
            }
            return Patterns.NoPattern;
        }
        public static Patterns GetEnumFromCharacter(char character)
        {
            //todo This needs all the characters represented by the keyboard
            switch (character)
            {
                case '0': return Patterns.Number0;
                case '1': return Patterns.Number1;
                case '2': return Patterns.Number2;
                case '3': return Patterns.Number3;
                case '4': return Patterns.Number4;
                case '5': return Patterns.Number5;
                case '6': return Patterns.Number6;
                case '7': return Patterns.Number7;
                case '8': return Patterns.Number8;
                case '9': return Patterns.Number9;
                case 'A': return Patterns.LetterA;
                case 'B': return Patterns.LetterB;
                case 'C': return Patterns.LetterC;
                case 'D': return Patterns.LetterD;
                case 'E': return Patterns.LetterE;
                case 'F': return Patterns.LetterF;
                case 'G': return Patterns.LetterG;
                case 'H': return Patterns.LetterH;
                case 'I': return Patterns.LetterI;
                case 'J': return Patterns.LetterJ;
                case 'K': return Patterns.LetterK;
                case 'L': return Patterns.LetterL;
                case 'M': return Patterns.LetterM;
                case 'N': return Patterns.LetterN;
                case 'O': return Patterns.LetterO;
                case 'P': return Patterns.LetterP;
                case 'Q': return Patterns.LetterQ;
                case 'R': return Patterns.LetterR;
                case 'S': return Patterns.LetterS;
                case 'T': return Patterns.LetterT;
                case 'U': return Patterns.LetterU;
                case 'V': return Patterns.LetterV;
                case 'W': return Patterns.LetterW;
                case 'X': return Patterns.LetterX;
                case 'Y': return Patterns.LetterY;
                case 'Z': return Patterns.LetterZ;
                case 'a': return Patterns.LowercaseLetterA;
                case 'b': return Patterns.LowercaseLetterB;
                case 'c': return Patterns.LowercaseLetterC;
                case 'd': return Patterns.LowercaseLetterD;
                case 'e': return Patterns.LowercaseLetterE;
                case 'f': return Patterns.LowercaseLetterF;
                case 'g': return Patterns.LowercaseLetterG;
                case 'h': return Patterns.LowercaseLetterH;
                case 'i': return Patterns.LowercaseLetterI;
                case 'j': return Patterns.LowercaseLetterJ;
                case 'k': return Patterns.LowercaseLetterK;
                case 'l': return Patterns.LowercaseLetterL;
                case 'm': return Patterns.LowercaseLetterM;
                case 'n': return Patterns.LowercaseLetterN;
                case 'o': return Patterns.LowercaseLetterO;
                case 'p': return Patterns.LowercaseLetterP;
                case 'q': return Patterns.LowercaseLetterQ;
                case 'r': return Patterns.LowercaseLetterR;
                case 's': return Patterns.LowercaseLetterS;
                case 't': return Patterns.LowercaseLetterT;
                case 'u': return Patterns.LowercaseLetterU;
                case 'v': return Patterns.LowercaseLetterV;
                case 'w': return Patterns.LowercaseLetterW;
                case 'x': return Patterns.LowercaseLetterX;
                case 'y': return Patterns.LowercaseLetterY;
                case 'z': return Patterns.LowercaseLetterZ;
                case '→': return Patterns.RightArrow;
                case '←': return Patterns.LeftArrow;
                case 'ñ': return Patterns.LowerCaseNWithTilde;
                case 'á': return Patterns.LowerCaseAWithAque;
                case 'é': return Patterns.LowerCaseEWithAque;
                case 'í': return Patterns.LowerCaseIWithAque;
                case 'ó': return Patterns.LowerCaseOWithAque;
                case 'ú': return Patterns.LowerCaseUwithAque;
                case 'ü': return Patterns.LowerCaseUwithDoubleDots;
                case '!': return Patterns.ExclamationPoint;

                default:
                    throw new Exception($"Invalid character of {character} was passed in. it must be somethign mapped to a pattern");
            }
        }
        public static string GetCharacterFromEnum(Patterns pattern)
        {
            switch (pattern)
            {
                case Patterns.LetterA: return "A";
                case Patterns.LetterB: return "B";
                case Patterns.LetterC: return "C";
                case Patterns.LetterD: return "D";
                case Patterns.LetterE: return "E";
                case Patterns.LetterF: return "F";
                case Patterns.LetterG: return "G";
                case Patterns.LetterH: return "H";
                case Patterns.LetterI: return "I";
                case Patterns.LetterJ: return "J";
                case Patterns.LetterK: return "K";
                case Patterns.LetterL: return "L";
                case Patterns.LetterM: return "M";
                case Patterns.LetterN: return "N";
                case Patterns.LetterO: return "O";
                case Patterns.LetterP: return "P";
                case Patterns.LetterQ: return "Q";
                case Patterns.LetterR: return "R";
                case Patterns.LetterS: return "S";
                case Patterns.LetterT: return "T";
                case Patterns.LetterU: return "U";
                case Patterns.LetterV: return "V";
                case Patterns.LetterW: return "W";
                case Patterns.LetterX: return "X";
                case Patterns.LetterY: return "Y";
                case Patterns.LetterZ: return "Z";
                default:
                    throw new Exception($"Invalid pattern of {pattern} was passed in. It must be between pattern representing a letter between A and Z");
            }
        }
    }
}
