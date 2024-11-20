using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saheki;

namespace StorybrewScripts
{
    public class Flashes : StoryboardObjectGenerator
    {
        [Group("General")]
        [Description("Comma separated list of times in milliseconds aka bookmarks")]
        [Configurable] public string ManualOverrideList = string.Empty;
        [Configurable] public double Opacity = 100 * (double)2 / 3;

        [Group("Timing")]
        [Configurable] public int Offset = 0;

        public override void Generate()
        {
            try
            {
                var pixelPath = Path.Combine("sb", "Pixel.png");
                if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

                var manualOverrides = ManualOverrideList == string.Empty ? new List<double>() : ManualOverrideList
                    .Split(',')
                    .Select(x => (double)int.Parse(x.Trim()) + Offset)
                    .ToList();
                
                var flashes = manualOverrides
                    .Concat(Helpers.GetBeatmapHitFinishesInKiai(Beatmap, AudioDuration))
                    .GroupBy(x => x)
                    .Where(g => g.Count() == 1)
                    .Select(g => g.Key)
                    .OrderBy(x => x)
                    .ToList();

                if(flashes.Count == 0) return;

                var flash = GetLayer("").CreateSprite(pixelPath);
                float scale = 480f / GetMapsetBitmap(pixelPath).Size.Height;
                flash.ScaleVec(flashes.First(), scale * (16f / 9f), scale);
                flash.Color(flashes.First(), Color4.White);
                flash.Additive(flashes.First());

                foreach (var time in flashes)
                {
                    var delay = Helpers.Snap(Beatmap, (int)time, 1, 1);;
                    flash.Fade(OsbEasing.OutExpo, time + Offset, time + delay + Offset, Opacity / 100, 0);
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }
    }
}
