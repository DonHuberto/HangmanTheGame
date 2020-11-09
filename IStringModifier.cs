using System;
using System.Text.RegularExpressions;

namespace HangmanGameProject
{
    public interface IStringModifier
    {
        /// <summary>
        /// Changing casing of given @text to Title Case:
        /// first letter of each word it's upper case, rest of the word is lower case 
        /// </summary>
        /// <param name="text">String to change to Title Case</param>
        /// <returns></returns>
        public static string ToTitleCase( string text )
        {
            // changing each letter at the beginning of a word to it's upper version
            return Regex.Replace( text.ToLower(), @"\b([a-z])", p => p.Value.ToUpper() );
        }

        /// <summary>
        /// Highlight one line of text in console.
        /// </summary>
        /// <param name="text">Line of text to be highlighted</param>
        public static void WriteHighlightText( string text )
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}