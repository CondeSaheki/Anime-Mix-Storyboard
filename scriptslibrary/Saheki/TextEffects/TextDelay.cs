using OpenTK;
using StorybrewCommon.Storyboarding;
using System;

namespace Saheki
{
    public class TextDelay : Text.IEffect
    {
        public int interval;

        public TextDelay(double beatInterval)
        {
            interval = (int)Math.Round(beatInterval / 8);
        }

        public TextDelay(int interval = 500)
        {
            this.interval = interval;
        }

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float height, float width, char character, string text)
        {
            int delay = index * interval / count;

            if (startTime + delay > endTime) throw new Exception($"TextDelay, {text}, {startTime}: Duration is too short to apply this effect");

            sprite.Scale(startTime + delay, scale);
            sprite.Move(startTime + delay, position);
            sprite.Fade(startTime + delay, startTime + delay, 0, 1);
            sprite.Fade(endTime - 1, endTime, 1, 0);
        }
    }
}