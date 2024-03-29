﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Helpers
{
    public enum StandardTransitions
    {
        NoTransition,
        SlideThroughScreenFromLeft,
        SlideThroughScreenFromRight,
        SlideThroughScreenFromBottom,
        SlideThroughScreenFromTop,
        SlideOnScreenFromLeftSide,
        SlideOnScreenFromRightSide,
        SlideOnScreenFromTop,
        SlideOnScreenFromBottom,
        //Todo Activate these Transitions
        //SlideOffScreenToTheRight,
        //SlideOffScreenToTheLeft,
        //SlideOffScreenToTheTop,
        //SlideOffScreenToTheBottom
    }
    public enum Patterns : ushort
    {
        NoPattern = 0,
        Number1 = 1,
        Number2 = 2,
        Number3 = 3,
        Number4 = 4,
        Number5 = 5,
        Number6 = 6,
        Number7 = 7,
        Number8 = 8,
        Number9 = 9,
        Number10 = 10,
        Number11 = 11,
        Number12 = 12,
        Number13 = 13,
        Number14 = 14,
        Number15 = 15,
        Number16 = 16,
        Number17 = 17,
        Number18 = 18,
        Number19 = 19,
        Number20 = 20,
        Number21 = 21,
        Number22 = 22,
        Number23 = 23,
        Number24 = 24,
        Number25 = 25,
        Number26 = 26,
        Number27 = 27,
        Number28 = 28,
        Number29 = 29,
        Number30 = 30,
        Number31 = 31,
        Number32 = 32,
        Number33 = 33,
        Number34 = 34,
        Number35 = 35,
        Number36 = 36,
        Number37 = 37,
        Number38 = 38,
        Number39 = 39,
        Number40 = 40,
        Number41 = 41,
        Number42 = 42,
        Number43 = 43,
        Number44 = 44,
        Number45 = 45,
        Number46 = 46,
        Number47 = 47,
        Number48 = 48,
        Number49 = 49,
        Number50 = 50,
        Number51 = 51,
        Number52 = 52,
        Number53 = 53,
        Number54 = 54,
        Number55 = 55,
        Number56 = 56,
        Number57 = 57,
        Number58 = 58,
        Number59 = 59,
        Number60 = 60,
        Number61 = 61,
        Number62 = 62,
        Number63 = 63,
        Number64 = 64,
        Number65 = 65,
        Number66 = 66,
        Number67 = 67,
        Number68 = 68,
        Number69 = 69,
        Number70 = 70,
        Number71 = 71,
        Number72 = 72,
        Number73 = 73,
        Number74 = 74,
        Number75 = 75,
        Number76 = 76,
        Number77 = 77,
        Number78 = 78,
        Number79 = 79,
        Number80 = 80,
        Number81 = 81,
        Number82 = 82,
        Number83 = 83,
        Number84 = 84,
        Number85 = 85,
        Number86 = 86,
        Number87 = 87,
        Number88 = 88,
        Number89 = 89,
        Number90 = 90,
        Number91 = 91,
        Number92 = 92,
        Number93 = 93,
        Number94 = 94,
        Number95 = 95,
        Number96 = 96,
        Number97 = 97,
        Number98 = 98,
        Number99 = 99,
        Number0 = 100,
        LetterA = 101,
        LetterB = 102,
        LetterC = 103,
        LetterD = 104,
        LetterE = 105,
        LetterF = 106,
        LetterG = 107,
        LetterH = 108,
        LetterI = 109,
        LetterJ = 110,
        LetterK = 111,
        LetterL = 112,
        LetterM = 113,
        LetterN = 114,
        LetterO = 115,
        LetterP = 116,
        LetterQ = 117,
        LetterR = 118,
        LetterS = 119,
        LetterT = 120,
        LetterU = 121,
        LetterV = 122,
        LetterW = 123,
        LetterX = 124,
        LetterY = 125,
        LetterZ = 126,
        AllOneColor = 127,
        CheckMark = 128,
        XOut = 129,
        Border = 130,
        LowercaseLetterA = 131,
        LowercaseLetterB = 132,
        LowercaseLetterC = 133,
        LowercaseLetterD = 134,
        LowercaseLetterE = 135,
        LowercaseLetterF = 136,
        LowercaseLetterG = 137,
        LowercaseLetterH = 138,
        LowercaseLetterI = 139,
        LowercaseLetterJ = 140,
        LowercaseLetterK = 141,
        LowercaseLetterL = 142,
        LowercaseLetterM = 143,
        LowercaseLetterN = 144,
        LowercaseLetterO = 145,
        LowercaseLetterP = 146,
        LowercaseLetterQ = 147,
        LowercaseLetterR = 148,
        LowercaseLetterS = 149,
        LowercaseLetterT = 150,
        LowercaseLetterU = 151,
        LowercaseLetterV = 152,
        LowercaseLetterW = 153,
        LowercaseLetterX = 154,
        LowercaseLetterY = 155,
        LowercaseLetterZ = 156,
        WifiHigh = 157,
        WifiMedium = 158,
        WifiLow = 159,
        DownArrow = 160,
        RightArrow = 161,
        UpArrow = 162,
        LeftArrow = 163,
        PlainSmilyFace = 164,
        PlusSign = 165,
        MinusSign = 166,
        Asterix = 167,
        ForwardSlash = 168,
        Divide = 169,
        Equals = 170,
        Carrot = 171,
        LeftAngleBracket = 172,
        RightAngleBracket = 173,
        LeftParentheses = 174,
        RightParentheses = 175,
        LeftSquareBracket = 176,
        RightSquareBracket = 177,
        LeftCurlyBrace = 178,
        RightCurlyBrace = 179,
        Period = 180,
        Colon = 181,
        SemiColon = 182,
        Comma = 183,
        ExclamationPoint = 184,
        QuestionMark = 185,
        AtSign = 186,
        Amersand = 187,
        DollarSign = 188,
        PoundSign = 189,
        BackSlash = 190,
        LeftSingleQuoteMark = 191,
        RightSingleQuoteMark = 192,
        LowerCaseAWithAque = 193,
        LowerCaseEWithAque = 194,
        LowerCaseIWithAque = 195,
        LowerCaseOWithAque = 196,
        LowerCaseNWithTilde = 197,
        LowerCaseUwithAque = 198,
        LowerCaseUwithDoubleDots = 199
    };
    public class PatternHelper
    {


        public static List<Byte> GetBytesForPattern(Patterns pattern)
        {
            var result = pattern switch
            {
                Patterns.NoPattern => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 },
                Patterns.Number1 => new List<Byte> { 0b00000000, 0b00011000, 0b00011000, 0b00111000, 0b00011000, 0b00011000, 0b00011000, 0b01111110 },
                Patterns.Number2 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b00000110, 0b00001100, 0b00110000, 0b01100000, 0b01111110 },

                Patterns.Number3 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b00000110, 0b00011100, 0b00000110, 0b01100110, 0b00111100 },
                // //4
                Patterns.Number4 => new List<Byte> { 0b00000000, 0b00001100, 0b00011100, 0b00101100, 0b01001100, 0b01111110, 0b00001100, 0b00001100 },
                // //5
                Patterns.Number5 => new List<Byte> { 0b00000000, 0b01111110, 0b01100000, 0b01111100, 0b00000110, 0b00000110, 0b01100110, 0b00111100 },
                // //6
                Patterns.Number6 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b00111100 },
                // //7
                Patterns.Number7 => new List<Byte> { 0b00000000, 0b01111110, 0b01100110, 0b00001100, 0b00001100, 0b00011000, 0b00011000, 0b00011000 },
                // //8
                Patterns.Number8 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b00111100, 0b01100110, 0b01100110, 0b00111100 },
                // //9
                Patterns.Number9 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b01100110, 0b00111100 },
                // //10
                Patterns.Number10 => new List<Byte> { 0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001001, 0b01001001, 0b01001001, 0b01001111 },
                // //11
                Patterns.Number11 => new List<Byte> { 0b00000000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000 },
                // //12
                Patterns.Number12 => new List<Byte> { 0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01001111, 0b01001000, 0b01001000, 0b01001111 },
                // //13
                Patterns.Number13 => new List<Byte> { 0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01001111, 0b01000001, 0b01000001, 0b01001111 },
                // //14
                Patterns.Number14 => new List<Byte> { 0b00000000, 0b01001001, 0b01001001, 0b01001001, 0b01001111, 0b01000001, 0b01000001, 0b01000001 },
                // //15
                Patterns.Number15 => new List<Byte> { 0b00000000, 0b01001111, 0b01001000, 0b01001000, 0b01001111, 0b01000001, 0b01000001, 0b01001111 },
                // //16
                Patterns.Number16 => new List<Byte> { 0b00000000, 0b01001000, 0b01001000, 0b01001000, 0b01001111, 0b01001001, 0b01001001, 0b01001111 },
                // //17
                Patterns.Number17 => new List<Byte> { 0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01000001, 0b01000001, 0b01000001, 0b01000001 },
                // //18
                Patterns.Number18 => new List<Byte> { 0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001111, 0b01001001, 0b01001001, 0b01001111 },
                // //19
                Patterns.Number19 => new List<Byte> { 0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001111, 0b01000001, 0b01000001, 0b01000001 },
                // //20
                Patterns.Number20 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101001, 0b10001001, 0b10001001, 0b11101111 },
                // //21
                Patterns.Number21 => new List<Byte> { 0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b11100100, 0b10000100, 0b10000100, 0b11100100 },
                // //22
                Patterns.Number22 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11101111, 0b10001000, 0b10001000, 0b11101111 },
                // //23
                Patterns.Number23 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11101111, 0b10000001, 0b10000001, 0b11101111 },
                // //24
                Patterns.Number24 => new List<Byte> { 0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b11101111, 0b10000001, 0b10000001, 0b11100001 },
                // //25
                Patterns.Number25 => new List<Byte> { 0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b11101111, 0b10000001, 0b10000001, 0b11101111 },
                // //26
                Patterns.Number26 => new List<Byte> { 0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b11101111, 0b10001001, 0b10001001, 0b11101111 },
                // //27
                Patterns.Number27 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100001, 0b10000001, 0b10000001, 0b11100001 },
                // //28
                Patterns.Number28 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b10001001, 0b10001001, 0b11101111 },
                // //29
                Patterns.Number29 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b10000001, 0b10000001, 0b11100001 },
                // //30
                Patterns.Number30 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101001, 0b00101001, 0b00101001, 0b11101111 },
                // //31
                Patterns.Number31 => new List<Byte> { 0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b11100100, 0b00100100, 0b00100100, 0b11100100 },
                // //32
                Patterns.Number32 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100111, 0b00101000, 0b00101000, 0b11101111 },
                // //33
                Patterns.Number33 => new List<Byte> { 0b00000000, 0b11101110, 0b00100010, 0b00100010, 0b11101110, 0b00100010, 0b00100010, 0b11101110 },
                // //34
                Patterns.Number34 => new List<Byte> { 0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b11101111, 0b00100001, 0b00100001, 0b11100001 },
                // //35
                Patterns.Number35 => new List<Byte> { 0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b11101111, 0b00100001, 0b00100001, 0b11101111 },
                // //36
                Patterns.Number36 => new List<Byte> { 0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b11101111, 0b00101001, 0b00101001, 0b11101111 },
                // //37
                Patterns.Number37 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100001, 0b00100001, 0b00100001, 0b11100001 },
                // //38
                Patterns.Number38 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b00101001, 0b00101001, 0b11101111 },
                // //39
                Patterns.Number39 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b00100001, 0b00100001, 0b11100001 },
                // //40
                Patterns.Number40 => new List<Byte> { 0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101001, 0b00101001, 0b00101001, 0b00101111 },
                // //41
                Patterns.Number41 => new List<Byte> { 0b00000000, 0b10100100, 0b10100100, 0b10100100, 0b11100100, 0b00100100, 0b00100100, 0b00100100 },
                // //42
                Patterns.Number42 => new List<Byte> { 0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11101111, 0b00101000, 0b00101000, 0b00101111 },
                // //43
                Patterns.Number43 => new List<Byte> { 0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11101111, 0b00100001, 0b00100001, 0b00101111 },
                // //44
                Patterns.Number44 => new List<Byte> { 0b00000000, 0b10101001, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001 },
                // //45
                Patterns.Number45 => new List<Byte> { 0b00000000, 0b10101111, 0b10101000, 0b10101000, 0b11101111, 0b00100001, 0b00100001, 0b00101111 },
                // //46
                Patterns.Number46 => new List<Byte> { 0b00000000, 0b10101000, 0b10101000, 0b10101000, 0b11101111, 0b00101001, 0b00101001, 0b00101111 },
                // //47
                Patterns.Number47 => new List<Byte> { 0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11100001, 0b00100001, 0b00100001, 0b00100001 },
                // //48
                Patterns.Number48 => new List<Byte> { 0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101111, 0b00101001, 0b00101001, 0b00101111 },
                // //49
                Patterns.Number49 => new List<Byte> { 0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001 },

                // //50
                Patterns.Number50 => new List<Byte> { 0b00000000, 0b11100100, 0b10000100, 0b10000100, 0b11100100, 0b00100100, 0b00100100, 0b11100100 },
                // //51
                Patterns.Number51 => new List<Byte> { 0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11101111, 0b00101000, 0b00101000, 0b11101111 },
                // //52
                Patterns.Number52 => new List<Byte> { 0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11101111, 0b00100001, 0b00100001, 0b11101111 },
                // //53
                Patterns.Number53 => new List<Byte> { 0b00000000, 0b11101001, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001 },
                // //54
                Patterns.Number54 => new List<Byte> { 0b00000000, 0b11101000, 0b10001000, 0b10001000, 0b11101111, 0b00101001, 0b00101001, 0b11101111 },
                // //55
                Patterns.Number55 => new List<Byte> { 0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11100001, 0b00100001, 0b00100001, 0b11100001 },
                // //56
                Patterns.Number56 => new List<Byte> { 0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00101001, 0b00101001, 0b11101111 },
                // //57
                Patterns.Number57 => new List<Byte> { 0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001 },
                // //58
                Patterns.Number58 => new List<Byte> { 0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001 },
                // //59
                Patterns.Number59 => new List<Byte> { 0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101001, 0b10101001, 0b10101001, 0b11101111 },
                // //60
                Patterns.Number60 => new List<Byte> { 0b00000000, 0b10000100, 0b10000100, 0b10000100, 0b11100100, 0b10100100, 0b10100100, 0b11100100 },
                // //61
                Patterns.Number61 => new List<Byte> { 0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11101111, 0b10101000, 0b10101000, 0b11101111 },
                // //62
                Patterns.Number62 => new List<Byte> { 0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11101111, 0b10100001, 0b10100001, 0b11101111 },
                // //63
                Patterns.Number63 => new List<Byte> { 0b00000000, 0b10001001, 0b10001001, 0b10001001, 0b11101111, 0b10100001, 0b10100001, 0b11100001 },
                // //64
                Patterns.Number64 => new List<Byte> { 0b00000000, 0b10001111, 0b10001000, 0b10001000, 0b11101111, 0b10100001, 0b10100001, 0b11101111 },
                // //65
                Patterns.Number65 => new List<Byte> { 0b00000000, 0b10001000, 0b10001000, 0b10001000, 0b11101111, 0b10101001, 0b10101001, 0b11101111 },
                // //66
                Patterns.Number66 => new List<Byte> { 0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11100001, 0b10100001, 0b10100001, 0b11100001 },
                // //67
                Patterns.Number67 => new List<Byte> { 0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101111, 0b10101001, 0b10101001, 0b11101111 },
                // //68
                Patterns.Number68 => new List<Byte> { 0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101111, 0b10100001, 0b10100001, 0b11100001 },
                // //69
                Patterns.Number69 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101001, 0b00101001, 0b00101001, 0b00101111 },
                // //70
                Patterns.Number70 => new List<Byte> { 0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100 },
                // //71
                Patterns.Number71 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00101111, 0b00101000, 0b00101000, 0b00101111 },
                // //72
                Patterns.Number72 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00101111, 0b00100001, 0b00100001, 0b00101111 },
                // //73
                Patterns.Number73 => new List<Byte> { 0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001 },
                // //74
                Patterns.Number74 => new List<Byte> { 0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b00101111, 0b00100001, 0b00100001, 0b00101111 },
                // //75
                Patterns.Number75 => new List<Byte> { 0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b00101111, 0b00101001, 0b00101001, 0b00101111 },
                // //76
                Patterns.Number76 => new List<Byte> { 0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00100001, 0b00100001, 0b00100001, 0b00100001 },
                // //77
                Patterns.Number77 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00101001, 0b00101001, 0b00101111 },
                // //78
                Patterns.Number78 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001 },
                // //79
                Patterns.Number79 => new List<Byte> { 0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001 },
                // //80
                Patterns.Number80 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101001, 0b10101001, 0b10101001, 0b11101111 },
                // //81
                Patterns.Number81 => new List<Byte> { 0b00000000, 0b11100100, 0b10100100, 0b10100100, 0b11100100, 0b10100100, 0b10100100, 0b11100100 },
                // //82
                Patterns.Number82 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b10101000, 0b10101000, 0b11101111 },
                // //83
                Patterns.Number83 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b10100001, 0b10100001, 0b11101111 },
                // //84
                Patterns.Number84 => new List<Byte> { 0b00000000, 0b11101001, 0b10101001, 0b10101001, 0b11101111, 0b10100001, 0b10100001, 0b11100001 },
                // //85
                Patterns.Number85 => new List<Byte> { 0b00000000, 0b11101111, 0b10101000, 0b10101000, 0b11101111, 0b10100001, 0b10100001, 0b11101111 },
                // //86
                Patterns.Number86 => new List<Byte> { 0b00000000, 0b11101000, 0b10101000, 0b10101000, 0b11101111, 0b10101001, 0b10101001, 0b11101111 },
                // //87
                Patterns.Number87 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11100001, 0b10100001, 0b10100001, 0b11100001 },
                // //88
                Patterns.Number88 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b10101001, 0b10101001, 0b11101111 },
                // //89
                Patterns.Number89 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b10100001, 0b10100001, 0b11100001 },
                // //90
                Patterns.Number90 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101001, 0b00101001, 0b00101001, 0b00101111 },
                // //91
                Patterns.Number91 => new List<Byte> { 0b00000000, 0b11100100, 0b10100100, 0b10100100, 0b11100100, 0b00100100, 0b00100100, 0b00100100 },
                // //92
                Patterns.Number92 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b00101000, 0b00101000, 0b00101111 },
                // //93
                Patterns.Number93 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b00100001, 0b00100001, 0b00101111 },
                // //94
                Patterns.Number94 => new List<Byte> { 0b00000000, 0b11101001, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001 },
                // //95
                Patterns.Number95 => new List<Byte> { 0b00000000, 0b11101111, 0b10101000, 0b10101000, 0b11101111, 0b00100001, 0b00100001, 0b00101111 },
                // //96
                Patterns.Number96 => new List<Byte> { 0b00000000, 0b11101000, 0b10101000, 0b10101000, 0b11101111, 0b00101001, 0b00101001, 0b00101111 },
                // //97
                Patterns.Number97 => new List<Byte> { 0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11100001, 0b00100001, 0b00100001, 0b00100001 },
                // //98
                Patterns.Number98 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b00101001, 0b00101001, 0b00101111 },
                // //99
                Patterns.Number99 => new List<Byte> { 0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001 },
                // //0 - 100
                Patterns.Number0 => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100 },
                // //A - 101
                Patterns.LetterA => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01111110, 0b01100110, 0b01100110, 0b01100110 },
                // //B - 102
                Patterns.LetterB => new List<Byte> { 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01100110, 0b01100110, 0b01111100 },
                // //C - 103
                Patterns.LetterC => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100000, 0b01100000, 0b01100110, 0b00111100 },
                // //D - 104
                Patterns.LetterD => new List<Byte> { 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01111100 },
                // //E - 105
                Patterns.LetterE => new List<Byte> { 0b00000000, 0b01111110, 0b01100000, 0b01100000, 0b01111100, 0b01100000, 0b01100000, 0b01111110 },
                // //F - 106
                Patterns.LetterF => new List<Byte> { 0b00000000, 0b01111110, 0b01100000, 0b01100000, 0b01111100, 0b01100000, 0b01100000, 0b01100000 },
                // //G - 107
                Patterns.LetterG => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100000, 0b01101110, 0b01100110, 0b00111100 },
                // //H - 108
                Patterns.LetterH => new List<Byte> { 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01111110, 0b01100110, 0b01100110, 0b01100110 },
                // //I - 109
                Patterns.LetterI => new List<Byte> { 0b00000000, 0b00111100, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100 },
                // //J - 110
                Patterns.LetterJ => new List<Byte> { 0b00000000, 0b00011110, 0b00001100, 0b00001100, 0b00001100, 0b01101100, 0b01101100, 0b00111000 },
                // //K - 111
                Patterns.LetterK => new List<Byte> { 0b00000000, 0b01100110, 0b01101100, 0b01111000, 0b01110000, 0b01111000, 0b01101100, 0b01100110 },
                // //L - 112
                Patterns.LetterL => new List<Byte> { 0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01111110 },
                // //M - 113
                Patterns.LetterM => new List<Byte> { 0b00000000, 0b01100011, 0b01110111, 0b01111111, 0b01101011, 0b01100011, 0b01100011, 0b01100011 },
                // //N - 114
                Patterns.LetterN => new List<Byte> { 0b00000000, 0b01100011, 0b01110011, 0b01111011, 0b01101111, 0b01100111, 0b01100011, 0b01100011 },
                // //O - 115
                Patterns.LetterO => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100 },
                // //P - 116
                Patterns.LetterP => new List<Byte> { 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000 },
                // //Q - 117
                Patterns.LetterQ => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b01101110, 0b00111100, 0b00000110 },
                // //R - 118
                Patterns.LetterR => new List<Byte> { 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01111000, 0b01101100, 0b01100110 },
                // //S - 119
                Patterns.LetterS => new List<Byte> { 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b00111100, 0b00000110, 0b01100110, 0b00111100 },
                // //T - 120
                Patterns.LetterT => new List<Byte> { 0b00000000, 0b01111110, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000 },
                // //U - 121
                Patterns.LetterU => new List<Byte> { 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111110 },
                // //V - 122
                Patterns.LetterV => new List<Byte> { 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000 },
                // //W - 123
                Patterns.LetterW => new List<Byte> { 0b00000000, 0b01100011, 0b01100011, 0b01100011, 0b01101011, 0b01111111, 0b01110111, 0b01100011 },
                // //X - 124
                Patterns.LetterX => new List<Byte> { 0b00000000, 0b01100011, 0b01100011, 0b00110110, 0b00011100, 0b00110110, 0b01100011, 0b01100011 },
                // //Y - 125
                Patterns.LetterY => new List<Byte> { 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000, 0b00011000, 0b00011000 },
                // //z - 126
                Patterns.LetterZ => new List<Byte> { 0b00000000, 0b01111110, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b01111110 },
                // //All One Color - 127
                Patterns.AllOneColor => new List<Byte> { 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111 },
                // //CheckMark - 128
                Patterns.CheckMark => new List<Byte> { 0b00000011, 0b00000011, 0b00000111, 0b00000110, 0b11001100, 0b11111100, 0b01111000, 0b00110000 },
                // //Xout - 129
                Patterns.XOut => new List<Byte> { 0b10000001, 0b01000010, 0b00100100, 0b00011000, 0b00011000, 0b00100100, 0b01000010, 0b10000001 },
                // //Border - 130
                Patterns.Border => new List<Byte> { 0b11111111, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b11111111 },
                // //a -131
                Patterns.LowercaseLetterA => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00000110, 0b00111110, 0b01100110, 0b00111110 },
                // //b -132
                Patterns.LowercaseLetterB => new List<Byte> { 0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b01111100 },
                // //c -133
                Patterns.LowercaseLetterC => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100110, 0b00111100 },
                // //d -134
                Patterns.LowercaseLetterD => new List<Byte> { 0b00000000, 0b00000110, 0b00000110, 0b00000110, 0b00111110, 0b01100110, 0b01100110, 0b00111110 },
                // //e -135
                Patterns.LowercaseLetterE => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01111110, 0b01100000, 0b00111100 },
                // //f -136
                Patterns.LowercaseLetterF => new List<Byte> { 0b00000000, 0b00011100, 0b00110110, 0b00110000, 0b00110000, 0b01111100, 0b00110000, 0b00110000 },
                // //g -137
                Patterns.LowercaseLetterG => new List<Byte> { 0b00000000, 0b00000000, 0b00111110, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b00111100 },
                // //h -138
                Patterns.LowercaseLetterH => new List<Byte> { 0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b01100110 },
                // //i -139
                Patterns.LowercaseLetterI => new List<Byte> { 0b00000000, 0b00000000, 0b00011000, 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00111100 },
                // //j -140
                Patterns.LowercaseLetterJ => new List<Byte> { 0b00000000, 0b00001100, 0b00000000, 0b00001100, 0b00001100, 0b01101100, 0b01101100, 0b00111000 },
                // //k -141
                Patterns.LowercaseLetterK => new List<Byte> { 0b00000000, 0b01100000, 0b01100000, 0b01100110, 0b01101100, 0b01111000, 0b01101100, 0b01100110 },
                // //l -142
                Patterns.LowercaseLetterL => new List<Byte> { 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000 },
                // //m -143
                Patterns.LowercaseLetterM => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01100011, 0b01110111, 0b01111111, 0b01101011, 0b01101011 },
                // //n -144
                Patterns.LowercaseLetterN => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b01111110, 0b01100110, 0b01100110, 0b01100110 },
                // //o -145
                Patterns.LowercaseLetterO => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b00111100 },
                // //p -146
                Patterns.LowercaseLetterP => new List<Byte> { 0b00000000, 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000 },
                // //q -147
                Patterns.LowercaseLetterQ => new List<Byte> { 0b00000000, 0b00000000, 0b00111100, 0b01101100, 0b01101100, 0b00111100, 0b00001101, 0b00001111 },
                // //r -148
                Patterns.LowercaseLetterR => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100000, 0b01100000 },
                // //s -149
                Patterns.LowercaseLetterS => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111110, 0b01000000, 0b00111100, 0b00000010, 0b01111100 },
                // //t -150
                Patterns.LowercaseLetterT => new List<Byte> { 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b01111110, 0b00011000, 0b00011000, 0b00011000 },
                // //u -151
                Patterns.LowercaseLetterU => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111110 },
                // //v -152
                Patterns.LowercaseLetterV => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b00111100, 0b00011000 },
                // //w -153
                Patterns.LowercaseLetterW => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01100011, 0b01101011, 0b01101011, 0b01101011, 0b00111110 },
                // //x -155
                Patterns.LowercaseLetterX => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b00111100, 0b00011000, 0b00111100, 0b01100110 },
                // //y -155
                Patterns.LowercaseLetterY => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b00111100 },
                // //z -156
                Patterns.LowercaseLetterZ => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00001100, 0b00011000, 0b00110000, 0b00111100 },
                // //WifiHigh - 157
                Patterns.WifiHigh => new List<Byte> { 0b00000000, 0b00000011, 0b00000011, 0b00011011, 0b00011011, 0b11011011, 0b11011011, 0b11011011 },
                // //WifiMedium- 158
                Patterns.WifiMedium => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b11011000, 0b11011000, 0b11011000 },
                // //WifiLow- 159
                Patterns.WifiLow => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000000, 0b11000000, 0b11000000 },
                // //Down Arrow - 160
                Patterns.DownArrow => new List<Byte> { 0b00111000, 0b00111000, 0b00111000, 0b00111000, 0b11111110, 0b01111100, 0b00111000, 0b00010000 },
                // //Right Arrow - 161
                Patterns.RightArrow => new List<Byte> { 0b00001000, 0b00001100, 0b11111110, 0b11111111, 0b11111110, 0b00001100, 0b00001000, 0b00000000 },
                // //Up Arrow 162
                Patterns.UpArrow => new List<Byte> { 0b00010000, 0b00111000, 0b01111100, 0b11111110, 0b00111000, 0b00111000, 0b00111000, 0b00111000 },
                // //Left Arrow 163
                Patterns.LeftArrow => new List<Byte> { 0b00010000, 0b00110000, 0b01111111, 0b11111111, 0b01111111, 0b00110000, 0b00010000, 0b00000000 },
                // //SmileyFace - 164
                Patterns.PlainSmilyFace => new List<Byte> { 0b00111100, 0b01111110, 0b10111101, 0b10111101, 0b11111111, 0b10111101, 0b01000010, 0b00111100 },
                // //Plus Sign - 165
                Patterns.PlusSign => new List<Byte> { 0b00000000, 0b00110000, 0b00110000, 0b11111100, 0b00110000, 0b00110000, 0b00000000, 0b00000000 },
                // //Minus Sign - 166
                Patterns.MinusSign => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000, 0b00000000, 0b00000000 },
                // //Asterix - 167
                Patterns.Asterix => new List<Byte> { 0b00000000, 0b01100110, 0b00111100, 0b11111111, 0b00111100, 0b01100110, 0b00000000, 0b00000000 },
                // //Forward Slash - 168
                Patterns.ForwardSlash => new List<Byte> { 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b10000000, 0b00000000 },
                // //Divide - 169
                Patterns.Divide => new List<Byte> { 0b00000000, 0b11000110, 0b11001100, 0b00011000, 0b00110000, 0b01100110, 0b11000110, 0b00000000 },
                // //Equals - 170
                Patterns.Equals => new List<Byte> { 0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000 },
                // //Carrot - 171
                Patterns.Carrot => new List<Byte> { 0b00010000, 0b00111000, 0b01101100, 0b11000110, 0b00000000, 0b00000000, 0b00000000, 0b00000000 },
                // //Left Angle bracket - 172
                Patterns.LeftAngleBracket => new List<Byte> { 0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00000000 },
                // //Right Angle bracket - 173
                Patterns.RightAngleBracket => new List<Byte> { 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b00000000 },
                // //Left parentheses- 174
                Patterns.LeftParentheses => new List<Byte> { 0b00011000, 0b00110000, 0b01100000, 0b01100000, 0b01100000, 0b00110000, 0b00011000, 0b00000000 },
                // //Right parentheses - 175
                Patterns.RightParentheses => new List<Byte> { 0b01100000, 0b00110000, 0b00011000, 0b00011000, 0b00011000, 0b00110000, 0b01100000, 0b00000000 },
                // //Left Square bracket -176
                Patterns.LeftSquareBracket => new List<Byte> { 0b01111000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01111000, 0b00000000 },
                // //Right Square bracket -177
                Patterns.RightSquareBracket => new List<Byte> { 0b01111000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b01111000, 0b00000000 },
                // //Left Curly 0brace -178
                Patterns.LeftCurlyBrace => new List<Byte> { 0b00011100, 0b00110000, 0b00110000, 0b11100000, 0b00110000, 0b00110000, 0b00011100, 0b00000000 },
                // //Right Curly 0brace -179
                Patterns.RightCurlyBrace => new List<Byte> { 0b11100000, 0b00110000, 0b00110000, 0b00011100, 0b00110000, 0b00110000, 0b11100000, 0b00000000 },
                // //Period -180
                Patterns.Period => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b00000000 },
                // //Colon -181
                Patterns.Colon => new List<Byte> { 0b00000000, 0b00110000, 0b00110000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b00000000 },
                // //SemiColon -182
                Patterns.SemiColon => new List<Byte> { 0b00000000, 0b00110000, 0b00110000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b01100000 },
                // //Comma -183
                Patterns.Comma => new List<Byte> { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b01100000 },
                // //Exclamation Point -184
                Patterns.ExclamationPoint => new List<Byte> { 0b00011000, 0b00111100, 0b00111100, 0b00011000, 0b00011000, 0b00000000, 0b00011000, 0b00000000 },
                // //QuestionMark -185
                Patterns.QuestionMark => new List<Byte> { 0b01111000, 0b11001100, 0b00001100, 0b00011000, 0b00110000, 0b00000000, 0b00110000, 0b00000000 },
                // //At Sign -186
                Patterns.AtSign => new List<Byte> { 0b01111100, 0b11000110, 0b11011110, 0b11011110, 0b11011110, 0b11000000, 0b01111000, 0b00000000 },
                // //Ampersand -187
                Patterns.Amersand => new List<Byte> { 0b00111000, 0b01101100, 0b00111000, 0b01110110, 0b11011100, 0b11001100, 0b01110110, 0b00000000 },
                // //Dollar Sign -188
                Patterns.DollarSign => new List<Byte> { 0b00110000, 0b01111100, 0b11000000, 0b01111000, 0b00001100, 0b11111000, 0b00110000, 0b00000000 },
                // //Pound Sign - 189
                Patterns.PoundSign => new List<Byte> { 0b01101100, 0b01101100, 0b11111110, 0b01101100, 0b11111110, 0b01101100, 0b01101100, 0b00000000 },
                // //BackSlash -190
                Patterns.BackSlash => new List<Byte> { 0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110, 0b00000010, 0b00000000 },
                // //Left Single Quote mark - 191
                Patterns.LeftSingleQuoteMark => new List<Byte> { 0b00110000, 0b00110000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 },
                // //Right Single Quote mark - 192
                Patterns.RightSingleQuoteMark => new List<Byte> { 0b01100000, 0b01100000, 0b11000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 },
                // //a with aque mark - 193
                Patterns.LowerCaseAWithAque => new List<Byte> { 0b00001000, 0b00010000, 0b00000000, 0b01111100, 0b00001100, 0b01111100, 0b11001100, 0b01111110 },
                // //e with aque mark - 194
                Patterns.LowerCaseEWithAque => new List<Byte> { 0b00001000, 0b00010000, 0b00000000, 0b01111000, 0b11001100, 0b11111100, 0b11000000, 0b01111000 },
                // //i with aque mark - 195
                Patterns.LowerCaseIWithAque => new List<Byte> { 0b00000010, 0b00000100, 0b00110000, 0b00000000, 0b00110000, 0b00110000, 0b00110000, 0b00110000 },
                // //o with aque mark- 196
                Patterns.LowerCaseOWithAque => new List<Byte> { 0b00000000, 0b00001000, 0b00010000, 0b00000000, 0b01111000, 0b11001100, 0b11001100, 0b01111000 },
                // //n with tilde mark- 197
                Patterns.LowerCaseNWithTilde => new List<Byte> { 0b00000000, 0b01010000, 0b10101000, 0b00000000, 0b11111000, 0b11001100, 0b11001100, 0b11001100 },
                // //u with aque mark- 198
                Patterns.LowerCaseUwithAque => new List<Byte> { 0b00000100, 0b00001000, 0b00010000, 0b00000000, 0b11001100, 0b11001100, 0b01001100, 0b01111110 },
                // //u with double dots- 199
                Patterns.LowerCaseUwithDoubleDots => new List<Byte> { 0b00000000, 0b00000000, 0b11001100, 0b00000000, 0b11001100, 0b11001100, 0b11111100, 0b01111110 }
            };
            return result;
        }
        private List<byte> currentPattern { get; set; }
        //    public static List<List<Byte>> BuiltinPatterns = new List<List<Byte>>  {
        //    //Zero means no pattern
        //   new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000},
        //    //1
        //   new List<Byte> {0b00000000, 0b00011000, 0b00011000, 0b00111000, 0b00011000, 0b00011000, 0b00011000, 0b01111110},
        //    //2
        //   new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b00000110, 0b00001100, 0b00110000, 0b01100000, 0b01111110},
        //    //3
        //   new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b00000110, 0b00011100, 0b00000110, 0b01100110, 0b00111100},
        //    //4
        //   new List<Byte>  {0b00000000, 0b00001100, 0b00011100, 0b00101100, 0b01001100, 0b01111110, 0b00001100, 0b00001100},
        //    //5
        //   new List<Byte>  {0b00000000, 0b01111110, 0b01100000, 0b01111100, 0b00000110, 0b00000110, 0b01100110, 0b00111100},
        //    //6
        //   new List<Byte>  {0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b00111100},
        //    //7
        //   new List<Byte>  {0b00000000, 0b01111110, 0b01100110, 0b00001100, 0b00001100, 0b00011000, 0b00011000, 0b00011000},
        //    //8
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b00111100, 0b01100110, 0b01100110, 0b00111100},
        //    //9
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b01100110, 0b00111100},
        //    //10
        //    new List<Byte> {0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001001, 0b01001001, 0b01001001, 0b01001111},
        //    //11
        //    new List<Byte> {0b00000000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000, 0b01001000},
        //    //12
        //    new List<Byte> {0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01001111, 0b01001000, 0b01001000, 0b01001111},
        //    //13
        //    new List<Byte> {0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01001111, 0b01000001, 0b01000001, 0b01001111},
        //    //14
        //    new List<Byte> {0b00000000, 0b01001001, 0b01001001, 0b01001001, 0b01001111, 0b01000001, 0b01000001, 0b01000001},
        //    //15
        //    new List<Byte> {0b00000000, 0b01001111, 0b01001000, 0b01001000, 0b01001111, 0b01000001, 0b01000001, 0b01001111},
        //    //16
        //    new List<Byte> {0b00000000, 0b01001000, 0b01001000, 0b01001000, 0b01001111, 0b01001001, 0b01001001, 0b01001111},
        //    //17
        //    new List<Byte> {0b00000000, 0b01001111, 0b01000001, 0b01000001, 0b01000001, 0b01000001, 0b01000001, 0b01000001},
        //    //18
        //    new List<Byte> {0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001111, 0b01001001, 0b01001001, 0b01001111},
        //    //19
        //    new List<Byte> {0b00000000, 0b01001111, 0b01001001, 0b01001001, 0b01001111, 0b01000001, 0b01000001, 0b01000001},
        //    //20
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101001, 0b10001001, 0b10001001, 0b11101111},
        //    //21
        //    new List<Byte> {0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b11100100, 0b10000100, 0b10000100, 0b11100100},
        //    //22
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11101111, 0b10001000, 0b10001000, 0b11101111},
        //    //23
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11101111, 0b10000001, 0b10000001, 0b11101111},
        //    //24
        //    new List<Byte> {0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b11101111, 0b10000001, 0b10000001, 0b11100001},
        //    //25
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b11101111, 0b10000001, 0b10000001, 0b11101111},
        //    //26
        //    new List<Byte> {0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b11101111, 0b10001001, 0b10001001, 0b11101111},
        //    //27
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100001, 0b10000001, 0b10000001, 0b11100001},
        //    //28
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b10001001, 0b10001001, 0b11101111},
        //    //29
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b10000001, 0b10000001, 0b11100001},
        //    //30
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101001, 0b00101001, 0b00101001, 0b11101111},
        //    //31
        //    new List<Byte> {0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b11100100, 0b00100100, 0b00100100, 0b11100100},
        //    //32
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100111, 0b00101000, 0b00101000, 0b11101111},
        //    //33
        //    new List<Byte> {0b00000000, 0b11101110, 0b00100010, 0b00100010, 0b11101110, 0b00100010, 0b00100010, 0b11101110},
        //    //34
        //    new List<Byte> {0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b11101111, 0b00100001, 0b00100001, 0b11100001},
        //    //35
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b11101111, 0b00100001, 0b00100001, 0b11101111},
        //    //36
        //    new List<Byte> {0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b11101111, 0b00101001, 0b00101001, 0b11101111},
        //    //37
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b11100001, 0b00100001, 0b00100001, 0b11100001},
        //    //38
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b00101001, 0b00101001, 0b11101111},
        //    //39
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b11101111, 0b00100001, 0b00100001, 0b11100001},
        //    //40
        //    new List<Byte> {0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101001, 0b00101001, 0b00101001, 0b00101111},
        //    //41
        //    new List<Byte> {0b00000000, 0b10100100, 0b10100100, 0b10100100, 0b11100100, 0b00100100, 0b00100100, 0b00100100},
        //    //42
        //    new List<Byte> {0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11101111, 0b00101000, 0b00101000, 0b00101111},
        //    //43
        //    new List<Byte> {0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11101111, 0b00100001, 0b00100001, 0b00101111},
        //    //44
        //    new List<Byte> {0b00000000, 0b10101001, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001},
        //    //45
        //    new List<Byte> {0b00000000, 0b10101111, 0b10101000, 0b10101000, 0b11101111, 0b00100001, 0b00100001, 0b00101111},
        //    //46
        //    new List<Byte> {0b00000000, 0b10101000, 0b10101000, 0b10101000, 0b11101111, 0b00101001, 0b00101001, 0b00101111},
        //    //47
        //    new List<Byte> {0b00000000, 0b10101111, 0b10100001, 0b10100001, 0b11100001, 0b00100001, 0b00100001, 0b00100001},
        //    //48
        //    new List<Byte> {0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101111, 0b00101001, 0b00101001, 0b00101111},
        //    //49
        //    new List<Byte> {0b00000000, 0b10101111, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001},

        //    //50
        //    new List<Byte> {0b00000000, 0b11100100, 0b10000100, 0b10000100, 0b11100100, 0b00100100, 0b00100100, 0b11100100},
        //    //51
        //    new List<Byte> {0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11101111, 0b00101000, 0b00101000, 0b11101111},
        //    //52
        //    new List<Byte> {0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11101111, 0b00100001, 0b00100001, 0b11101111},
        //    //53
        //    new List<Byte> {0b00000000, 0b11101001, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001},
        //    //54
        //    new List<Byte> {0b00000000, 0b11101000, 0b10001000, 0b10001000, 0b11101111, 0b00101001, 0b00101001, 0b11101111},
        //    //55
        //    new List<Byte> {0b00000000, 0b11101111, 0b10000001, 0b10000001, 0b11100001, 0b00100001, 0b00100001, 0b11100001},
        //    //56
        //    new List<Byte> {0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00101001, 0b00101001, 0b11101111},
        //    //57
        //    new List<Byte> {0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001},
        //    //58
        //    new List<Byte> {0b00000000, 0b11101111, 0b10001001, 0b10001001, 0b11101111, 0b00100001, 0b00100001, 0b11100001},
        //    //59
        //    new List<Byte> {0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101001, 0b10101001, 0b10101001, 0b11101111},
        //    //60
        //    new List<Byte> {0b00000000, 0b10000100, 0b10000100, 0b10000100, 0b11100100, 0b10100100, 0b10100100, 0b11100100},
        //    //61
        //    new List<Byte> {0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11101111, 0b10101000, 0b10101000, 0b11101111},
        //    //62
        //    new List<Byte> {0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11101111, 0b10100001, 0b10100001, 0b11101111},
        //    //63
        //    new List<Byte> {0b00000000, 0b10001001, 0b10001001, 0b10001001, 0b11101111, 0b10100001, 0b10100001, 0b11100001},
        //    //64
        //    new List<Byte> {0b00000000, 0b10001111, 0b10001000, 0b10001000, 0b11101111, 0b10100001, 0b10100001, 0b11101111},
        //    //65
        //    new List<Byte> {0b00000000, 0b10001000, 0b10001000, 0b10001000, 0b11101111, 0b10101001, 0b10101001, 0b11101111},
        //    //66
        //    new List<Byte> {0b00000000, 0b10001111, 0b10000001, 0b10000001, 0b11100001, 0b10100001, 0b10100001, 0b11100001},
        //    //67
        //    new List<Byte> {0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101111, 0b10101001, 0b10101001, 0b11101111},
        //    //68
        //    new List<Byte> {0b00000000, 0b10001111, 0b10001001, 0b10001001, 0b11101111, 0b10100001, 0b10100001, 0b11100001},
        //    //69
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101001, 0b00101001, 0b00101001, 0b00101111},
        //    //70
        //    new List<Byte> {0b00000000, 0b11100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100, 0b00100100},
        //    //71
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00101111, 0b00101000, 0b00101000, 0b00101111},
        //    //72
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00101111, 0b00100001, 0b00100001, 0b00101111},
        //    //73
        //    new List<Byte> {0b00000000, 0b11101001, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001},
        //    //74
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101000, 0b00101000, 0b00101111, 0b00100001, 0b00100001, 0b00101111},
        //    //75
        //    new List<Byte> {0b00000000, 0b11101000, 0b00101000, 0b00101000, 0b00101111, 0b00101001, 0b00101001, 0b00101111},
        //    //76
        //    new List<Byte> {0b00000000, 0b11101111, 0b00100001, 0b00100001, 0b00100001, 0b00100001, 0b00100001, 0b00100001},
        //    //77
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00101001, 0b00101001, 0b00101111},
        //    //78
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001},
        //    //79
        //    new List<Byte> {0b00000000, 0b11101111, 0b00101001, 0b00101001, 0b00101111, 0b00100001, 0b00100001, 0b00100001},
        //    //80
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101001, 0b10101001, 0b10101001, 0b11101111},
        //    //81
        //    new List<Byte> {0b00000000, 0b11100100, 0b10100100, 0b10100100, 0b11100100, 0b10100100, 0b10100100, 0b11100100},
        //    //82
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b10101000, 0b10101000, 0b11101111},
        //    //83
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b10100001, 0b10100001, 0b11101111},
        //    //84
        //    new List<Byte> {0b00000000, 0b11101001, 0b10101001, 0b10101001, 0b11101111, 0b10100001, 0b10100001, 0b11100001},
        //    //85
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101000, 0b10101000, 0b11101111, 0b10100001, 0b10100001, 0b11101111},
        //    //86
        //    new List<Byte> {0b00000000, 0b11101000, 0b10101000, 0b10101000, 0b11101111, 0b10101001, 0b10101001, 0b11101111},
        //    //87
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11100001, 0b10100001, 0b10100001, 0b11100001},
        //    //88
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b10101001, 0b10101001, 0b11101111},
        //    //89
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b10100001, 0b10100001, 0b11100001},
        //    //90
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101001, 0b00101001, 0b00101001, 0b00101111},
        //    //91
        //    new List<Byte> {0b00000000, 0b11100100, 0b10100100, 0b10100100, 0b11100100, 0b00100100, 0b00100100, 0b00100100},
        //    //92
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b00101000, 0b00101000, 0b00101111},
        //    //93
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11101111, 0b00100001, 0b00100001, 0b00101111},
        //    //94
        //    new List<Byte> {0b00000000, 0b11101001, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001},
        //    //95
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101000, 0b10101000, 0b11101111, 0b00100001, 0b00100001, 0b00101111},
        //    //96
        //    new List<Byte> {0b00000000, 0b11101000, 0b10101000, 0b10101000, 0b11101111, 0b00101001, 0b00101001, 0b00101111},
        //    //97
        //    new List<Byte> {0b00000000, 0b11101111, 0b10100001, 0b10100001, 0b11100001, 0b00100001, 0b00100001, 0b00100001},
        //    //98
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b00101001, 0b00101001, 0b00101111},
        //    //99
        //    new List<Byte> {0b00000000, 0b11101111, 0b10101001, 0b10101001, 0b11101111, 0b00100001, 0b00100001, 0b00100001},
        //    //0 - 100
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110,  0b01100110,  0b01100110,  0b00111100},
        //    //A - 101
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01111110, 0b01100110, 0b01100110, 0b01100110},
        //    //B - 102
        //    new List<Byte> {0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01100110, 0b01100110, 0b01111100},
        //    //C - 103
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100000, 0b01100000, 0b01100110, 0b00111100},
        //    //D - 104
        //    new List<Byte> {0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01111100},
        //    //E - 105
        //    new List<Byte> {0b00000000, 0b01111110, 0b01100000, 0b01100000, 0b01111100, 0b01100000, 0b01100000, 0b01111110},
        //    //F - 106
        //    new List<Byte> {0b00000000, 0b01111110, 0b01100000, 0b01100000, 0b01111100, 0b01100000, 0b01100000, 0b01100000},
        //    //G - 107
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100000, 0b01101110, 0b01100110, 0b00111100},
        //    //H - 108
        //    new List<Byte> {0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01111110, 0b01100110, 0b01100110, 0b01100110},
        //    //I - 109
        //    new List<Byte> {0b00000000, 0b00111100, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00111100},
        //    //J - 110
        //    new List<Byte> {0b00000000, 0b00011110, 0b00001100, 0b00001100, 0b00001100, 0b01101100, 0b01101100, 0b00111000},
        //    //K - 111
        //    new List<Byte> {0b00000000, 0b01100110, 0b01101100, 0b01111000, 0b01110000, 0b01111000, 0b01101100, 0b01100110},
        //    //L - 112
        //    new List<Byte> {0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01111110},
        //    //M - 113
        //    new List<Byte> {0b00000000, 0b01100011, 0b01110111, 0b01111111, 0b01101011, 0b01100011, 0b01100011, 0b01100011},
        //    //N - 114
        //    new List<Byte> {0b00000000, 0b01100011, 0b01110011, 0b01111011, 0b01101111, 0b01100111, 0b01100011, 0b01100011},
        //    //O - 115
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100},
        //    //P - 116
        //    new List<Byte> {0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000},
        //    //Q - 117
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b01101110, 0b00111100, 0b00000110},
        //    //R - 118
        //    new List<Byte> {0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01111000, 0b01101100, 0b01100110},
        //    //S - 119
        //    new List<Byte> {0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b00111100, 0b00000110, 0b01100110, 0b00111100},
        //    //T - 120
        //    new List<Byte> {0b00000000, 0b01111110, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000},
        //    //U - 121
        //    new List<Byte> {0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111110},
        //    //V - 122
        //    new List<Byte> {0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000},
        //    //W - 123
        //    new List<Byte> {0b00000000, 0b01100011, 0b01100011, 0b01100011, 0b01101011, 0b01111111, 0b01110111, 0b01100011},
        //    //X - 124
        //    new List<Byte> {0b00000000, 0b01100011, 0b01100011, 0b00110110, 0b00011100, 0b00110110, 0b01100011, 0b01100011},
        //    //Y - 125
        //    new List<Byte> {0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b00111100, 0b00011000, 0b00011000, 0b00011000},
        //    //x - 126
        //    new List<Byte> {0b00000000, 0b01111110, 0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b01111110},
        //    //All One Color - 127
        //    new List<Byte> {0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111},
        //    //CheckMark - 128
        //    new List<Byte> {0b00000011, 0b00000011, 0b00000111, 0b00000110, 0b11001100, 0b11111100, 0b01111000, 0b00110000},
        //    //Xout - 129
        //    new List<Byte> {0b10000001, 0b01000010, 0b00100100, 0b00011000, 0b00011000, 0b00100100, 0b01000010, 0b10000001},
        //    //Border - 130
        //    new List<Byte> {0b11111111, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b10000001, 0b11111111},
        //    //a -131
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00000110, 0b00111110, 0b01100110, 0b00111110},
        //    //b -132
        //    new List<Byte> {0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b01111100},
        //    //c -133
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01100000, 0b01100110, 0b00111100},
        //    //d -134
        //    new List<Byte> {0b00000000, 0b00000110, 0b00000110, 0b00000110, 0b00111110, 0b01100110, 0b01100110, 0b00111110},
        //    //e -135
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01111110, 0b01100000, 0b00111100},
        //    //f -136
        //    new List<Byte> {0b00000000, 0b00011100, 0b00110110, 0b00110000, 0b00110000, 0b01111100, 0b00110000, 0b00110000},
        //    //g -137
        //    new List<Byte> {0b00000000, 0b00000000, 0b00111110, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b00111100},
        //    //h -138
        //    new List<Byte> {0b00000000, 0b01100000, 0b01100000, 0b01100000, 0b01111100, 0b01100110, 0b01100110, 0b01100110},
        //    //i -139
        //    new List<Byte> {0b00000000, 0b00000000, 0b00011000, 0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00111100},
        //    //j -140
        //    new List<Byte> {0b00000000, 0b00001100, 0b00000000, 0b00001100, 0b00001100, 0b01101100, 0b01101100, 0b00111000},
        //    //k -141
        //    new List<Byte> {0b00000000, 0b01100000, 0b01100000, 0b01100110, 0b01101100, 0b01111000, 0b01101100, 0b01100110},
        //    //l -142
        //    new List<Byte> {0b00000000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000},
        //    //m -143
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01100011, 0b01110111, 0b01111111, 0b01101011, 0b01101011},
        //    //n -144
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b01111110, 0b01100110, 0b01100110, 0b01100110},
        //    //o -145
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b01100110, 0b01100110, 0b01100110, 0b00111100},
        //    //p -146
        //    new List<Byte> {0b00000000, 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01111100, 0b01100000, 0b01100000},
        //    //q -147
        //    new List<Byte> {0b00000000, 0b00000000, 0b00111100, 0b01101100, 0b01101100, 0b00111100, 0b00001101, 0b00001111},
        //    //r -148
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01111100, 0b01100110, 0b01100110, 0b01100000, 0b01100000},
        //    //s -149
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111110, 0b01000000, 0b00111100, 0b00000010, 0b01111100},
        //    //t -150
        //    new List<Byte> {0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b01111110, 0b00011000, 0b00011000, 0b00011000},
        //    //u -151
        //   new List<Byte>  {0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b01100110, 0b01100110, 0b00111110},
        //    //v -152
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b00111100, 0b00011000},
        //    //w -153
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01100011, 0b01101011, 0b01101011, 0b01101011, 0b00111110},
        //    //x -155
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b00111100, 0b00011000, 0b00111100, 0b01100110},
        //    //y -155
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b01100110, 0b01100110, 0b00111110, 0b00000110, 0b00111100},
        //    //z -156
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00111100, 0b00001100, 0b00011000, 0b00110000, 0b00111100},
        //    //WifiHigh - 157
        //    new List<Byte> {0b00000000, 0b00000011, 0b00000011, 0b00011011, 0b00011011, 0b11011011, 0b11011011, 0b11011011},
        //    //WifiMedium- 158
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00011000, 0b00011000, 0b11011000, 0b11011000, 0b11011000},
        //    //WifiLow- 159
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b11000000, 0b11000000, 0b11000000},
        //    //Down Arrow - 160
        //    new List<Byte> {0b00111000, 0b00111000, 0b00111000, 0b00111000, 0b11111110, 0b01111100, 0b00111000, 0b00010000},
        //    //Right Arrow - 161
        //    new List<Byte> {0b00001000, 0b00001100, 0b11111110, 0b11111111, 0b11111110, 0b00001100, 0b00001000, 0b00000000},
        //    //Up Arrow 162
        //    new List<Byte> {0b00010000, 0b00111000, 0b01111100, 0b11111110, 0b00111000, 0b00111000, 0b00111000, 0b00111000},
        //    //Left Arrow 163
        //    new List<Byte> {0b00010000, 0b00110000, 0b01111111, 0b11111111, 0b01111111, 0b00110000, 0b00010000, 0b00000000},
        //    //SmileyFace - 164
        //    new List<Byte> {0b00111100, 0b01111110, 0b10111101, 0b10111101, 0b11111111, 0b10111101, 0b01000010, 0b00111100},
        //    //Plus Sign - 165
        //    new List<Byte> {0b00000000, 0b00110000, 0b00110000, 0b11111100, 0b00110000, 0b00110000, 0b00000000, 0b00000000},
        //    //Minus Sign - 166
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000, 0b00000000, 0b00000000},
        //    //Asterix - 167
        //    new List<Byte> {0b00000000, 0b01100110, 0b00111100, 0b11111111, 0b00111100, 0b01100110, 0b00000000, 0b00000000},
        //    //Forward Slash - 168
        //    new List<Byte> {0b00000110, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b10000000, 0b00000000},
        //    //Divide - 169
        //    new List<Byte> {0b00000000, 0b11000110, 0b11001100, 0b00011000, 0b00110000, 0b01100110, 0b11000110, 0b00000000},
        //    //Equals - 170
        //    new List<Byte> {0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000, 0b11111100, 0b00000000, 0b00000000},
        //    //Carrot - 171
        //    new List<Byte> {0b00010000, 0b00111000, 0b01101100, 0b11000110, 0b00000000, 0b00000000, 0b00000000, 0b00000000},
        //    //Left Angle 0bracket - 172
        //    new List<Byte> {0b00011000, 0b00110000, 0b01100000, 0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00000000},
        //    //Right Angle 0bracket - 173
        //    new List<Byte> {0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00011000, 0b00110000, 0b01100000, 0b00000000},
        //    //Left parentheses- 174
        //    new List<Byte> {0b00011000, 0b00110000, 0b01100000, 0b01100000, 0b01100000, 0b00110000, 0b00011000, 0b00000000},
        //    //Right parentheses - 175
        //    new List<Byte> {0b01100000, 0b00110000, 0b00011000, 0b00011000, 0b00011000, 0b00110000, 0b01100000, 0b00000000},
        //    //Left Square 0bracket -176
        //    new List<Byte> {0b01111000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01100000, 0b01111000, 0b00000000},
        //    //Right Square 0bracket -177
        //    new List<Byte> {0b01111000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b00011000, 0b01111000, 0b00000000},
        //    //Left Curly 0brace -178
        //    new List<Byte> {0b00011100, 0b00110000, 0b00110000, 0b11100000, 0b00110000, 0b00110000, 0b00011100, 0b00000000},
        //    //Right Curly 0brace -179
        //    new List<Byte> {0b11100000, 0b00110000, 0b00110000, 0b00011100, 0b00110000, 0b00110000, 0b11100000, 0b00000000},
        //    //Period -180
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b00000000},
        //    //Colon -181
        //    new List<Byte> {0b00000000, 0b00110000, 0b00110000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b00000000},
        //    //SemiColon -182
        //    new List<Byte> {0b00000000, 0b00110000, 0b00110000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b01100000},
        //    //Comma -183
        //    new List<Byte> {0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00110000, 0b00110000, 0b01100000},
        //    //Exclamation Point -184
        //    new List<Byte> {0b00011000, 0b00111100, 0b00111100, 0b00011000, 0b00011000, 0b00000000, 0b00011000, 0b00000000},
        //    //QuestionMark -185
        //    new List<Byte> {0b01111000, 0b11001100, 0b00001100, 0b00011000, 0b00110000, 0b00000000, 0b00110000, 0b00000000},
        //    //At Sign -186
        //    new List<Byte> {0b01111100, 0b11000110, 0b11011110, 0b11011110, 0b11011110, 0b11000000, 0b01111000, 0b00000000},
        //    //Ampersand -187
        //    new List<Byte> {0b00111000, 0b01101100, 0b00111000, 0b01110110, 0b11011100, 0b11001100, 0b01110110, 0b00000000},
        //    //Dollar Sign -188
        //    new List<Byte> {0b00110000, 0b01111100, 0b11000000, 0b01111000, 0b00001100, 0b11111000, 0b00110000, 0b00000000},
        //    //Pound Sign - 189
        //    new List<Byte> {0b01101100, 0b01101100, 0b11111110, 0b01101100, 0b11111110, 0b01101100, 0b01101100, 0b00000000},
        //    //BackSlash -190
        //    new List<Byte> {0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110, 0b00000010, 0b00000000},
        //    //Left Single Quote mark - 191
        //    new List<Byte> {0b00110000, 0b00110000, 0b00011000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000},
        //    //Right Single Quote mark - 192
        //    new List<Byte> {0b01100000, 0b01100000, 0b11000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000},
        //    //a with aque mark - 193
        //    new List<Byte> {  0b00001000,  0b00010000,  0b00000000,  0b01111100,  0b00001100,  0b01111100,  0b11001100,  0b01111110},
        //    //e with aque mark - 194
        //    new List<Byte> {  0b00001000,  0b00010000,  0b00000000,  0b01111000,  0b11001100,  0b11111100,  0b11000000,  0b01111000},
        //    //i with aque mark - 195
        //    new List<Byte> {  0b00000010, 0b00000100,  0b00110000,  0b00000000,  0b00110000,  0b00110000,  0b00110000,  0b00110000},
        //    //o with aque mark- 196
        //    new List<Byte> {  0b00000000,  0b00001000,  0b00010000,  0b00000000,  0b01111000,  0b11001100,  0b11001100,  0b01111000},
        //    //n with tilde mark- 197
        //    new List<Byte> {  0b00000000,  0b01010000,  0b10101000,  0b00000000,  0b11111000,  0b11001100,  0b11001100,  0b11001100},
        //    //u with aque mark- 198
        //    new List<Byte> {  0b00000100,  0b00001000,  0b00010000,  0b00000000,  0b11001100,  0b11001100,  0b01001100,  0b01111110},
        //    //u with double dots- 199
        //    new List<Byte> {  0b00000000,  0b00000000,  0b11001100,  0b00000000,  0b11001100,  0b11001100,  0b11111100,  0b01111110}

        //};


        public bool ShouldItBeOn(int lightId)
        {
            int byteNumber = lightId / 8;
            int realBitNumber = lightId % 8;
            int adjustedBitNumber = 7 - realBitNumber;
            return ((currentPattern[byteNumber] >> adjustedBitNumber) & 1) != 0;
        }

        public PatternHelper(Patterns pattern)
        {
            currentPattern = GetBytesForPattern(pattern);
        }
    }

    public static class AnimationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadBoth(this string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="images">Images to place in a row</param>
        /// <param name="buttonCount">Button count to calculate the amount of space to put at the end</param>
        /// <param name="frameSpacing">Number of blank spaces between the images</param>
        /// <param name="scrollAllTextOffScreen">Should a big black chunk get added to the end to scroll everything off the end. </param>
        /// <returns></returns>
        public static ulong[,] BuildBigMatrix(List<ulong[,]> images, int buttonCount, int frameSpacing, bool scrollAllTextOffScreen)
        {
            int screenWidth = buttonCount * 8;
            //First a fullboard of blankSpaceAtTheStart.
            int bigMatrixColumnWidth = screenWidth;
            //Then the widths of all of the matrices
            bigMatrixColumnWidth += images.Sum(t => t.GetLength(1));
            //Then the widths of all of the spaces - remove 1 because we don't have a space at the start;
            bigMatrixColumnWidth += (images.Count * frameSpacing) - frameSpacing;
            if (scrollAllTextOffScreen)
            {
                bigMatrixColumnWidth += screenWidth;
            }


            var bigMatrix = new ulong[8, bigMatrixColumnWidth];
            int nextStartingSpot = screenWidth;
            //Build it out with the blank space and the Frame Spacing
            for (int i = 0; i < images.Count; i++)
            {
                bigMatrix.MergeMatrices(images[i], 0, nextStartingSpot, false);
                nextStartingSpot = nextStartingSpot + 8 + frameSpacing;

            }
            return bigMatrix;
        }

        public static List<Frame> ExtractFramesFromMatrix(ulong[,] matrix, int numberOfImagesToExtract, int pixelNumberFromLeftToStart, int currentFrameId = 0)
        {
            List<Frame> frames = new();
            for (int i = 0; i < numberOfImagesToExtract; i++)
            {
                frames.Add(new Frame(matrix.ExtractMatrix(0, pixelNumberFromLeftToStart + (i * 8)), currentFrameId));
            }
            return frames;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseMatrix">Matrix to overwrite</param>
        /// <param name="overrideMatrix">This will overwrite the base matrix as long as it is not a 0</param>
        public static void MergeMatrices(this ulong[,] baseMatrix, ulong[,] overrideMatrix, int startingRow = 0, int startingColumn = 0, bool dontWriteBlankSpacesFromOverride = true)
        {
            int overrideMatrixHeight = overrideMatrix.GetLength(0);
            int overrideMatrixWidth = overrideMatrix.GetLength(1);
            int baseMatrixHeight = baseMatrix.GetLength(0);
            int baseMatrixWidth = baseMatrix.GetLength(1);

            for (int row = 0; row < overrideMatrixHeight; row++)
            {
                for (int column = 0; column < overrideMatrixWidth; column++)
                {
                    int baseMatrixRow = startingRow + row;
                    int baseMatrixColumn = startingColumn + column;
                    if (baseMatrixRow > -1 && baseMatrixColumn > -1 && baseMatrixRow < baseMatrixHeight && baseMatrixColumn < baseMatrixWidth)
                        if (overrideMatrix[row, column] > 0 || dontWriteBlankSpacesFromOverride == false)
                        {
                            baseMatrix[baseMatrixRow, baseMatrixColumn] = overrideMatrix[row, column];
                        }
                }
            }
        }
        public static List<Frame> GetFrames(ulong[,] baseFrame, StandardTransitions standardTransitions, int framesPerStep = 1)
        {
            List<Frame> frames = new List<Frame>();
            int frameNumber = 0;
            switch (standardTransitions)
            {
                case StandardTransitions.NoTransition:
                    frames.Add(new Frame(baseFrame, frameNumber));
                    break;
                case StandardTransitions.SlideThroughScreenFromLeft:
                    for (int x = 7; x >= -7; x--)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, 0, x), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideThroughScreenFromRight:
                    for (int x = -7; x <= 7; x++)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, 0, x), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideThroughScreenFromTop:
                    for (int y = -7; y <= 7; y++)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, y, 0), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideThroughScreenFromBottom:
                    for (int y = 7; y >= -7; y--)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, y, 0), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideOnScreenFromLeftSide:
                    for (int x = 7; x >= 0; x--)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, 0, x), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideOnScreenFromRightSide:
                    for (int x = -7; x <= 0; x++)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, 0, x), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideOnScreenFromTop:
                    for (int y = 7; y >= 0; y--)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, y, 0), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                case StandardTransitions.SlideOnScreenFromBottom:
                    for (int y = -7; y <= 0; y++)
                    {
                        frames.Add(new Frame(AnimationHelper.ShiftMatrix(baseFrame, y, 0), frameNumber));
                        frameNumber += framesPerStep;
                    }
                    break;
                default:
                    frames.Add(new Frame(baseFrame, frameNumber));
                    break;
            }
            return frames;
        }

        public static ulong[,] GetMatrix(Patterns pattern, BapColor bapColor)
        {
            var cp = new PatternHelper(pattern);
            var result = new ulong[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    result[row, column] = cp.ShouldItBeOn(row * 8 + column) ? bapColor.LongColor : 0;
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixToShift"> Matrix that you need to shift</param>
        /// <param name="shiftLeft">How far to shift left. This can be a negative number to shift things to the right</param>
        /// <param name="shiftDown">How many rows to shift down. This can be a negative number to shift up</param>
        /// <returns></returns>
        public static ulong[,] ShiftMatrix(this ulong[,] matrixToShift, int shiftLeft, int shiftDown)
        {
            int matrixWidth = matrixToShift.GetLength(0);
            int matrixHeight = matrixToShift.GetLength(1);
            var newMatrix = new ulong[matrixWidth, matrixHeight];

            for (int i = 0; i < matrixWidth; i++)
            {
                int currentRowInImage = shiftLeft + i;
                for (int j = 0; j < matrixHeight; j++)
                {
                    int currentColumnInImage = shiftDown + j;
                    if (currentRowInImage >= 0 && currentRowInImage < matrixWidth && currentColumnInImage >= 0 && currentColumnInImage < matrixHeight)
                    {
                        newMatrix[i, j] = matrixToShift[currentRowInImage, currentColumnInImage];
                    }
                }

            }

            return newMatrix;
        }

        public static ulong[,] ExtractMatrix(this ulong[,] matrix, int upperLeftCornerRowId, int upperLeftCornerColumnId, int rowsToExtract = 8, int columnsToExtract = 8)
        {
            ulong[,] newMatrix = new ulong[rowsToExtract, columnsToExtract];
            int possibleRows = matrix.GetLength(0);
            int possibleColumns = matrix.GetLength(1);
            for (int lr = 0; lr < rowsToExtract; lr++)
            {
                for (int lc = 0; lc < columnsToExtract; lc++)
                {
                    int rowToExtractFrom = lr + upperLeftCornerRowId;
                    int columnToExtractFrom = lc + upperLeftCornerColumnId;
                    if (rowToExtractFrom > possibleRows || columnToExtractFrom > possibleColumns)
                    {
                        newMatrix[lr, lc] = 0;
                    }
                    else
                    {
                        newMatrix[lr, lc] = matrix[rowToExtractFrom, columnToExtractFrom];
                    }

                }
            }

            return newMatrix;
        }

        public static ulong[,] ConcatOnRow(this ulong[,] leftMatrix, ulong[,] rightMatrix)
        {
            int leftMatrixHeight = leftMatrix.GetLength(0);
            int leftMatrixWidth = leftMatrix.GetLength(1);
            int rightMatrixwidth = rightMatrix.GetLength(1);
            var newMatrix = new ulong[leftMatrixHeight, leftMatrixWidth + rightMatrixwidth];
            for (int lr = 0; lr < leftMatrixHeight; lr++)
            {
                for (int lc = 0; lc < leftMatrixWidth; lc++)
                {
                    newMatrix[lr, lc] = leftMatrix[lr, lc];
                }
            }
            for (int lr = 0; lr < leftMatrixHeight; lr++)
            {
                for (int lc = 0; lc < rightMatrixwidth; lc++)
                {
                    newMatrix[lr, lc + leftMatrixWidth] = rightMatrix[lr, lc];
                }
            }

            return newMatrix;
        }
        public static ulong[,] ConcatOnColumn(this ulong[,] topMatrix, ulong[,] bottomMatrix)
        {
            int topMatrixHeight = topMatrix.GetLength(0);
            int topMatrixWidth = topMatrix.GetLength(1);
            int bottomMatrixHeight = bottomMatrix.GetLength(0);
            var newMatrix = new ulong[topMatrixHeight + bottomMatrixHeight, topMatrixWidth];
            for (int lr = 0; lr < topMatrixHeight; lr++)
            {
                for (int lc = 0; lc < topMatrixWidth; lc++)
                {
                    newMatrix[lr, lc] = topMatrix[lr, lc];
                }
            }
            for (int lr = 0; lr < bottomMatrixHeight; lr++)
            {
                for (int lc = 0; lc < topMatrixWidth; lc++)
                {
                    newMatrix[lr + topMatrixHeight, lc] = bottomMatrix[lr, lc];
                }
            }

            return newMatrix;
        }

    }
}