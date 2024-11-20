
using System;
using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Storyboarding;

namespace Saheki
{
    public class TextUpDownEffect : Text.IEffect
    {
        private readonly Beatmap Beatmap;
        public float MovementMultiply = 1f;

        public TextUpDownEffect(Beatmap beatmap) { Beatmap = beatmap; }

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index,
            int count, float heigth, float width, char character, string text)
        {
            var delay = Helpers.Snap(Beatmap, startTime, 1, 1);
            if (endTime - delay < startTime + delay) throw new Exception($"TextUpDownEffect, text {text}, startTime {startTime}: Duration is too short to apply this effect");
            
            sprite.Scale(startTime, scale);
            sprite.Move(OsbEasing.OutBack, startTime, startTime + delay, position - new Vector2(0, heigth * MovementMultiply) / 2, position);
            sprite.Move(OsbEasing.InBack, endTime - delay, endTime, position, position + new Vector2(0, heigth * MovementMultiply) / 2);
            sprite.Fade(startTime, startTime + delay, 0, 1);
            sprite.Fade(endTime - delay, endTime, 1, 0);
        }
    }
}