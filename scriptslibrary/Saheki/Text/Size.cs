using OpenTK;
using StorybrewCommon.Subtitles;
using System;

namespace Saheki
{
    public static partial class Text
    {
        /// <summary>
        /// Calculates the total size of the given text based on the given font and scale.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The font to use when measuring the text.</param>
        /// <param name="scale">The scale of the font.</param>
        /// <param name="orientation">The orientation of the text, either horizontal or vertical.</param>
        /// <returns>The total size of the given text.</returns>
        public static Vector2 Size(FontGenerator font, string text, uint scale = 100, Orientation orientation = Orientation.Horizontal)
        {
            var scaleValue = Scale(scale);
            float width = 0f, height = 0f;

            foreach (var character in text)
            {
                var texture = font.GetTexture(character.ToString());
                // if (texture.IsEmpty) continue;

                if (orientation == Orientation.Horizontal)
                {
                    width += texture.BaseWidth * scaleValue;
                    height = Math.Max(height, texture.BaseHeight * scaleValue);
                }
                else
                {
                    width = Math.Max(width, texture.BaseWidth * scaleValue);
                    height += texture.BaseHeight * scaleValue;
                }
            }
            return new Vector2(width, height);
        }
    }
}