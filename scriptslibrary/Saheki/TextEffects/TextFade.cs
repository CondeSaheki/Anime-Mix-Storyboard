using OpenTK;
using StorybrewCommon.Storyboarding;

namespace Saheki
{
    public class TextFade : Text.IEffect
    {
        public int Delay = 500;
        public float Opacity = 1f;
        public bool FadeIn = true;
        public bool FadeOut = true;

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float height, float width, char character, string text)
        {
            sprite.Scale(startTime, scale);
            sprite.Move(startTime, position);
            if (FadeIn) sprite.Fade(startTime, startTime + Delay, 0, Opacity);
            else sprite.Fade(startTime, Opacity);

            if (FadeOut) sprite.Fade(endTime - Delay, endTime, Opacity, 0);
            else sprite.Fade(endTime - 1, endTime, Opacity, 0);
        }
    }
}