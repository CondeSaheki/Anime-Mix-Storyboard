using OpenTK;
using StorybrewCommon.Storyboarding;

namespace Saheki
{
    public class TextDefaultEffect : Text.IEffect
    {
        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float height, float width, char character, string text)
        {
            sprite.Scale(startTime, scale);
            sprite.Move(startTime, position);
            sprite.Fade(startTime, startTime, 0, 1);
            sprite.Fade(endTime - 1, endTime, 1, 0);
        }
    }  
}