using OpenTK;
using StorybrewCommon.Storyboarding;
using System;

namespace Saheki
{
    public class TextAngle : Text.IEffect
    {
        public float EntranceAngle = MathHelper.DegreesToRadians(180 - 60);
        public float EntranceDistance = 100f;
        public int Interval = 200;

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index,
            int count, float heigth, float width, char character, string text)
        {
            int delay = Interval / (count + 1);
            var currentDelay = delay * (index + 1);
            var a = delay * (index + 1);
            var distance = new Vector2((float)Math.Cos(EntranceAngle), (float)Math.Sin(EntranceAngle)) * EntranceDistance;

            sprite.Scale(startTime + currentDelay, scale);
            sprite.Move(OsbEasing.InOutSine, startTime + currentDelay, startTime + currentDelay + delay, position + distance, position);
            sprite.Fade(startTime + currentDelay, 1);
            sprite.Fade(endTime - 1, endTime, 1, 0);
        }
    }
}