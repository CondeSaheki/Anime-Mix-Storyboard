using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Storyboarding;

namespace Saheki
{

    public class TextColorEffect : Text.IEffect
    {
        public Color4 Color = Color4.Red;

        public TextColorEffect(Color4 color)
        {
            Color = color;
        }

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float heigth, float width, char character, string text)
        {
            sprite.Scale(startTime, scale);
            sprite.Color(startTime, Color);
            sprite.Move(startTime, position);
            sprite.Fade(startTime, startTime, 0, 1);
            sprite.Fade(endTime - 1, endTime, 1, 0);
        }
    }
}