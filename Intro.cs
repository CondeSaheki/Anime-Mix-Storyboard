using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.IO;
using Saheki;

namespace StorybrewScripts
{
    public class Intro : StoryboardObjectGenerator
    {
        [Group("Timing")]
        [Configurable] public int Start = 0;
        [Configurable] public int End = 0;
        [Configurable] public int Offset = 0;
        [Configurable] public int Delay = 500; // delete

        [Group("General")]
        [Configurable] public string Year = "????";
        [Configurable] public Vector2 Position = new Vector2(320, 240);
        [Configurable] public string LogoPath = Path.Combine("sb", "Logo.png");
        [Configurable] public float BackgroundOpacity = 0.66f;
        [Configurable] public float LogoScale = 0.5f;
        [Configurable] public int Spacing = 10;

        [Group("Font")]
        [Configurable] public string FontName = "";
        [Configurable] public int FontSize = 50;

        private FontGenerator Font;

        public override void Generate()
        {
            try
            {
                Font = LoadFont(Path.Combine("sb", "fIntro"), new FontDescription()
                {
                    FontPath = Helpers.GetFontPath(AssetPath, FontName),
                    FontSize = FontSize,
                    Color = Color4.White,
                    
                },
                new FontShadowClone()
                {
                    Distance = new Vector2(6, 6),
                    Color = new Color4(0, 0, 0, 255 * 1f / 3)
                },
                new FontGlow()
                {
                    Color = new Color4(0, 0, 0, 255),
                    Radius = 6,
                });

                GenerateBackground(Start + Offset, End + Offset, Delay, BackgroundOpacity);
                GenerateCmbLogo(Start + Offset, End + Offset, Position + new Vector2(-Spacing, 0));
                Text.Generate(this, Font, $"Anime {Year} Mix", Position, Start + Offset, End + Offset,100, new TextDelay());
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        public void GenerateBackground(int startTime, int endTime, int fadeInOut, double opacity)
        {
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");
            if (!File.Exists(Beatmap.BackgroundPath)) throw new Exception($"{Beatmap.BackgroundPath} is missing");
            var blackBackground = GetLayer("").CreateSprite(pixelPath);

            float scale = 480f / GetMapsetBitmap(pixelPath).Size.Height;
            blackBackground.ScaleVec(startTime, scale * (16f / 9f), scale);
            blackBackground.Color(startTime, Color4.Black);
            blackBackground.Fade(startTime, 1);
            blackBackground.Fade(endTime, endTime - 1, 1, 0);

            var background = GetLayer("").CreateSprite(Beatmap.BackgroundPath, OsbOrigin.Centre);
            background.Scale(startTime, 480f / GetMapsetBitmap(Beatmap.BackgroundPath).Size.Height);
            background.Fade(OsbEasing.InOutSine, startTime, startTime + fadeInOut, 0, opacity);
            background.Fade(OsbEasing.InOutSine, endTime - fadeInOut, endTime, opacity, 0);
        }

        public void GenerateCmbLogo(int startTime, int endTime, Vector2 position)
        {
            var logo = GetLayer("").CreateSprite(LogoPath, OsbOrigin.Centre, position);
            logo.Move(startTime + Delay * 2, position + new Vector2(-GetMapsetBitmap(LogoPath).Size.Width * LogoScale / 2, 0));
            logo.ScaleVec(startTime, new Vector2(LogoScale, LogoScale));
            logo.Fade(OsbEasing.InOutSine, startTime, startTime + Delay, 0, 1);
            logo.Fade(OsbEasing.InOutSine, startTime + Delay, endTime - Delay, 1, 1);
            logo.Fade(OsbEasing.InOutSine, endTime - Delay, endTime, 1, 0);
        }
    }
}
