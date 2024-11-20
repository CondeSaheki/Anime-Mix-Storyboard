using OpenTK;
using StorybrewCommon.Storyboarding;

namespace Saheki
{
    public static partial class Text
    {
        /// <summary>
        /// Defines an interface for applying effects to text elements.
        /// </summary>
        public interface IEffect
        {
            /// <summary>
            /// Draws the specified text element with the effect.
            /// </summary>
            /// <param name="o">The text element to draw with the effect.</param>
            void Draw(OsbSprite sprite,
                Vector2 position,
                int startTime,
                int endTime,
                float scale,
                int index,
                int count,
                float heigth,
                float width,
                char character,
                string text);
        }
    }
}