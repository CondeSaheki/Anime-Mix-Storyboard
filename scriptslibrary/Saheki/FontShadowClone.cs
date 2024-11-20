using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Subtitles;

namespace Saheki
{   
    /// <summary>
    /// Represents a font effect that applies a shadow to text elements.
    /// </summary>
    public class FontShadowClone : FontEffect
    {
        /// <summary>
        /// The offset of the shadow in the text.
        /// </summary>
        public Vector2 Distance = new Vector2(1, 1);

        /// <summary>
        /// The color of the shadow.
        /// </summary>
        public Color4 Color = new Color4(0, 0, 0, 255);

        /// <summary>
        /// Indicates whether the shadow effect is overlaid on top of the original text.
        /// </summary>
        public bool Overlay => false;

        public Vector2 Measure()
        {
            return Distance;
        }

        public void Draw(Bitmap bitmap, Graphics textGraphics, Font font, StringFormat stringFormat, string text, float x, float y)
        {
            if (Distance.X == 0 && Distance.Y == 0) return;

            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(Color.ToArgb())))
            {
                textGraphics.DrawString(text, font, brush, x + Distance.X, y + Distance.Y, stringFormat);
            }
        }
    }
}