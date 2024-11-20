using System;
using OpenTK;
using StorybrewCommon.Subtitles;

namespace Saheki
{
    public static partial class Text
    {
        /// <summary>
        /// The scale of the text.
        /// </summary>
        private const float TextScale = 0.5f;

        /// <summary>
        /// Calculates the scale for the given size percentage.
        /// </summary>
        /// <param name="size">The size to scale.</param>
        /// <returns>The scale of the given size.</returns>
        /// <exception cref="Exception">Thrown if <paramref name="size"/> is greater than 100.</exception>
        public static float Scale(uint size)
        {
            if (size > 100) throw new Exception("Scale can not be greater than 100");
            return TextScale * size / 100f;
        }

        /// <summary>
        /// Calculates the text scale required to fill the given constraints.
        /// </summary>
        /// <param name="font">The font to use when measuring the text.</param>
        /// <param name="text">The text to measure.</param>
        /// <param name="maxScale">The maximum scale of the text.</param>
        /// <param name="constraints">The constraints to fit the text within.</param>
        /// <param name="orientation">The orientation of the text, either horizontal or vertical.</param>
        /// <returns>The scale required to fit the text within the given constraints, but limited to the given maximum size.</returns>
        public static uint ScaleFill(FontGenerator font, string text, Vector2 constraints, uint maxScale = 100,  Orientation orientation = Orientation.Horizontal)
        {
            var size = Size(font, text, maxScale, orientation);

            var scaleWidth = constraints.X / size.X;
            var scaleHeight = constraints.Y / size.Y;

            var scale = Math.Min(scaleWidth, scaleHeight) * 100;

            return (uint)Math.Min(scale, maxScale);
        }
    }
}
