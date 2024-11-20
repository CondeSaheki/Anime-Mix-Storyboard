using OpenTK;
using StorybrewCommon.Storyboarding;
using System;

namespace Saheki
{
    public class TextDelayAngleEffect : Text.IEffect
    {
        public float Angle = MathHelper.DegreesToRadians(0);
        public int Delay = 500;
        public float Distance = 50;

        public TextDelayAngleEffect(float angle, int delay, float distance)
        {
            Angle = MathHelper.DegreesToRadians(angle);
            Delay = delay;
            Distance = distance;
        }

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float heigth, float width, char character, string text)
        {
            var delay = Delay / (count + 1);
            //var interval = (index + 1) / (count + 1);
            var currentDelay = delay * index;

            sprite.Scale(startTime + currentDelay, scale);
            sprite.Move(startTime + currentDelay, position);
            sprite.Fade(startTime + currentDelay, startTime + currentDelay + delay, 0, 1);
            sprite.Fade(endTime - delay, endTime, 1, 0);
            sprite.Move(OsbEasing.InOutBounce, startTime + currentDelay, startTime + currentDelay + delay, position + new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle)) * Distance, position);
        }
    }
}