using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Storyboarding;
using System;

namespace Saheki
{
    public class TextDelay : Text.IEffect
    {
        private readonly Beatmap Beatmap;

        public bool FadeIn = true;
        public bool FadeOut = true;
        public float Opacity = 1f;

        public TextDelay(Beatmap beatmap) { Beatmap = beatmap; }

        public void Draw(OsbSprite sprite, Vector2 position, int startTime, int endTime, float scale, int index, 
            int count, float height, float width, char character, string text)
        {
            var delay = Helpers.Snap(Beatmap, startTime, 1, 1);
            int animation = index * delay / count;

            if (startTime + animation > endTime) throw new Exception($"TextDelay, {text}, {startTime}: Duration is too short to apply this effect");

            sprite.Scale(startTime + animation, scale);
            sprite.Move(startTime + animation, position);

            if (FadeIn) sprite.Fade(OsbEasing.InOutSine, startTime + animation, startTime + animation + delay, 0, Opacity);
            else sprite.Fade(startTime, Opacity);

            if (FadeOut) sprite.Fade(OsbEasing.InOutSine, endTime - delay , endTime, Opacity, 0);
            else sprite.Fade(endTime - 1, endTime, Opacity, 0);
        }
    }
}