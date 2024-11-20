using System;
using System.Linq;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// Removes all illegal characters from a string that would cause issues with osu!
        /// </summary>
        /// <param name="input">The string to remove illegal characters from.</param>
        /// <returns>A string with all illegal characters removed.</returns>
        public static string LegalizeString(string input)
        {
            // ':', '?', '\"', '<', '>', '|', '*', '½'
            var allowedChars = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-+[](){}".ToCharArray();
            return new string(input.Where(c => allowedChars.Contains(c)).ToArray());
        }

    }
}