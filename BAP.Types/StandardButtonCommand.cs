using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAP.Types
{
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

    //[MessagePackObject]
    //public class StandardButtonCommand
    //{
    //    [Key(0)]
    //    public int MsgId { get; set; }
    //    [Key(1)]
    //    public long TurnOnInMillis { get; set; }
    //    [Key(2)]
    //    public bool WaitForCurrentTimerToComplete { get; set; }
    //    [Key(3)]
    //    public ButtonDisplay InitialDisplay { get; set; } = new ButtonDisplay();
    //    [Key(4)]
    //    public ButtonDisplay OnPressDisplay { get; set; } = new ButtonDisplay();
    //    [Key(5)]
    //    public ButtonDisplay OnTimeOutDisplay { get; set; } = new ButtonDisplay();
    //    public StandardButtonCommand()
    //    {

    //    }
    //    public StandardButtonCommand(ButtonDisplay initialDisplay, ButtonDisplay? onPressDisplay = null, ButtonDisplay? onTimeOutDisplahy = null, long turnOnInMillis = 0, bool waitForCurrentTimerToComplete = false)
    //    {
    //        InitialDisplay = initialDisplay;
    //        OnPressDisplay = onPressDisplay ?? new();
    //        OnTimeOutDisplay = onTimeOutDisplahy ?? new();
    //        TurnOnInMillis = turnOnInMillis;
    //        WaitForCurrentTimerToComplete = waitForCurrentTimerToComplete;

    //    }
    //    public override string ToString()
    //    {
    //        return $"StandardButtonCommand Initial display of {InitialDisplay} and onPressDisplay {OnPressDisplay} and OnTimeoutDisplay of {OnTimeOutDisplay}. Turn on in Millis is {TurnOnInMillis} and WaitForCurrentTimerToComplete is{WaitForCurrentTimerToComplete}";
    //    }
    //}


    //[MessagePackObject]
    //public class ButtonDisplay
    //{
    //    [Key(0)]
    //    public ushort Red { get; set; }
    //    [Key(1)]
    //    public ushort Green { get; set; }
    //    [Key(2)]
    //    public ushort Blue { get; set; }
    //    [Key(3)]
    //    public ushort Pattern { get; set; }
    //    [Key(4)]
    //    public ushort ImageId { get; set; }
    //    [Key(5)]
    //    public ushort Brightness { get; set; }
    //    [Key(6)]
    //    public long TurnOffAfterMillis { get; set; }
    //    //[Key(7)]
    //    //public Transition TransitionOn { get; set; }
    //    public ButtonDisplay()
    //    {

    //    }
    //    public ButtonDisplay(BapColor bapColor, Patterns pattern = Patterns.AllOneColor, int customImage = 0, long turnOffAfterMillis = 0, int brightness = 16) : this(bapColor.Red, bapColor.Green, bapColor.Blue, pattern, customImage, turnOffAfterMillis, brightness)
    //    {

    //    }
    //    public ButtonDisplay(int red = 0, int green = 0, int blue = 0, Patterns pattern = Patterns.AllOneColor, int customImage = 0, long turnOffAfterMillis = 0, int brightness = 16)
    //    {
    //        ushort realRed = red > 255 ? (ushort)255 : (ushort)red;
    //        ushort realGreen = green > 255 ? (ushort)255 : (ushort)green;
    //        ushort realBlue = blue > 255 ? (ushort)255 : (ushort)blue;
    //        ushort realCustomImage = customImage > 255 ? (ushort)255 : (ushort)customImage;
    //        Red = realRed;
    //        Green = realGreen;
    //        Blue = realBlue;
    //        Pattern = (ushort)pattern;
    //        ImageId = realCustomImage;
    //        Brightness = (ushort)brightness;
    //        TurnOffAfterMillis = turnOffAfterMillis;

    //    }
    //    public override string ToString()
    //    {
    //        List<string> allItems = new List<string>();
    //        allItems.Add(Pattern > 0 ? $"Pattern is {((Patterns)Pattern)} with Red = {Red}, Green = {Green} and Blue = {Blue}" : "");
    //        allItems.Add(ImageId > 0 ? $"Display the custom image with id {ImageId}" : "");
    //        allItems.Add($"with Brightness of {Brightness}");
    //        allItems.Remove("");
    //        return string.Join(" and ", allItems);
    //    }

    //}
}
