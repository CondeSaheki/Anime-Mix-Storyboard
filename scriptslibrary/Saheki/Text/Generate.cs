using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;

namespace Saheki
{
    public static partial class Text
    {
        /// <summary>
        /// Generates a storyboard object for a given text.
        /// </summary>
        /// <param name="script">The generator to use to create the storyboard object.</param>
        /// <param name="font">The font to use to generate the text.</param>
        /// <param name="text">The text to generate.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="startTime">The start time of the text.</param>
        /// <param name="endTime">The end time of the text.</param>
        /// <param name="scale">The size of the text.</param>
        /// <param name="origin">The origin point of the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="centralized">Whether or not to center the text.</param>
        /// <returns>The bounding box of the generated text.</returns>
        public static Box2 Generate(
            StoryboardObjectGenerator script,
            FontGenerator font,
            string text,
            Vector2 position,
            int startTime,
            int endTime,
            uint scale = 100,
            OsbOrigin origin = OsbOrigin.TopLeft,
            Orientation orientation = Orientation.Horizontal,
            bool centralized = true)
        {
            return Generate(script, font, text, position, startTime, endTime, scale, new TextDefaultEffect(), origin, orientation, centralized);
        }

        /// <summary>
        /// Generates a storyboard object for a given text.
        /// </summary>
        /// <param name="script">The generator to use to create the storyboard object.</param>
        /// <param name="font">The font to use to generate the text.</param>
        /// <param name="text">The text to generate.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="startTime">The start time of the text.</param>
        /// <param name="endTime">The end time of the text.</param>
        /// <param name="scale">The size of the text.</param>
        /// <param name="effect">The effect to apply to each character of the text.</param>
        /// <param name="origin">The origin point of the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="centralized">Whether or not to center the text.</param>
        /// <returns>The bounding box of the generated text.</returns>
        public static Box2 Generate(
            StoryboardObjectGenerator script,
            FontGenerator font,
            string text,
            Vector2 position,
            int startTime,
            int endTime,
            uint scale,
            IEffect effect,
            OsbOrigin origin = OsbOrigin.TopLeft,
            Orientation orientation = Orientation.Horizontal,
            bool centralized = true)
        {
            float scaleValue = Scale(scale);
            Vector2 size = Size(font, text, scale, orientation);
            Vector2 positionOffset = PositionOffset(size, origin) + position;

            Vector2 characterPosition = positionOffset;

            for (int i = 0; i < text.Length; ++i)
            {
                var character = text[i];
                var texture = font.GetTexture(character.ToString());

                if (!texture.IsEmpty)
                {
                    Vector2 characterPositionOffset = CharacterOffset(characterPosition, texture, scaleValue, size, orientation, centralized);

                    var sprite = script.GetLayer("").CreateSprite(texture.Path);
                    effect.Draw(
                        sprite,
                        characterPositionOffset,
                        startTime,
                        endTime,
                        scaleValue,
                        i,
                        text.Length,
                        size.Y,
                        size.X,
                        character,
                        text);
                }
                // Move to next character position based on orientation
                characterPosition += orientation == Orientation.Horizontal
                    ? new Vector2(texture.BaseWidth * scaleValue, 0)
                    : new Vector2(0, texture.BaseHeight * scaleValue);
            }

            return new Box2(positionOffset, positionOffset + size);
        }

        private static Vector2 CharacterOffset(Vector2 basePosition, FontTexture texture, float scale, Vector2 dimensions, Orientation orientation = Orientation.Horizontal, bool centralized = true)
        {
            Vector2 offset = basePosition + texture.OffsetFor(OsbOrigin.Centre) * scale;

            if (centralized)
            {
                offset.X += orientation == Orientation.Vertical ? (dimensions.X - texture.BaseWidth * scale) / 2 : 0;
                offset.Y += orientation == Orientation.Horizontal ? (dimensions.Y - texture.BaseHeight * scale) / 2 : 0;
            }

            return offset;
        }
    }
}