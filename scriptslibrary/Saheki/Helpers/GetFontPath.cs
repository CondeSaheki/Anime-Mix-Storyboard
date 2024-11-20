using System;
using System.IO;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// Returns the path to the given font name. If the font does not exist, it throws an exception.
        /// </summary>
        /// <param name="assetPath">The path to the asset folder.</param>
        /// <param name="fontName">The name of the font to retrieve.</param>
        /// <returns>The path to the given font name.</returns>
        public static string GetFontPath(string assetPath, string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return "Verdana";

            var fontPath = Path.Combine(assetPath, "Fonts", fontName);
            if (!File.Exists(fontPath)) throw new Exception($"Font not found: {fontPath}");
            return fontPath;
        }
    }
}