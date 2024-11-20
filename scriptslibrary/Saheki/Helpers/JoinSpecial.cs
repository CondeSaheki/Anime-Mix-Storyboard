using System.Collections.Generic;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// Joins a list of strings into a single string with a specified separator, 
        /// inserting a newline character when the maximum line length is exceeded.
        /// </summary>
        /// <param name="texts">The list of strings to join.</param>
        /// <param name="separator">The separator to use between strings.</param>
        /// <param name="maxLenght">The maximum length of a line before inserting a newline.</param>
        /// <returns>A single string with the texts joined by the separator and newlines.</returns>
        public static string JoinSpecial(IEnumerable<string> texts, string separator, uint maxLenght)
        {
            var result = "";
            var currentLine = "";

            foreach (var text in texts)
            {
                if (currentLine.Length + (currentLine.Length > 0 ? separator.Length : 0) + text.Length > maxLenght)
                {
                    result += (result.Length > 0 ? "\n" : "") + currentLine;
                    currentLine = text;
                }
                else currentLine += (currentLine.Length > 0 ? separator : "") + text;    
            }

            if (currentLine.Length > 0) result += (result.Length > 0 ? "\n" : "") + currentLine;
            
            return result;
        }
    }
}

 