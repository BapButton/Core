using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Helpers
{
    public static class BasicKeyboardLetters
    {
        public const string Numbers = "0123456789";
        public const string EnglishUpperCaseLetters = "ABCDEFGHIJKLMNOPRSTUVWXYZ";
        public const string EnglishLowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        public const string EnglishUpperCaseLettersAndNumbers = EnglishUpperCaseLetters + Numbers;
        public const string EnglishLowerLettersAndNumbers = EnglishLowerCaseLetters + Numbers;
        public const string SpanishUpperCase = "AÁBCDEÉFGHIJKLMNÑOÒPRSTUÚÜVWXYZ";
        public const string SpanishLowerCase = "aábcdeéfghiíjklmnñoópqrstuúüvwxyz";
        public const string SpanishLowerCaseWithNumbers = SpanishLowerCase + Numbers;
        public const string SpanishUpperCaseLettersAndNumbers = SpanishUpperCase + Numbers;
    }
}
