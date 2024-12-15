using StorybrewCommon.Storyboarding;
using System.Collections.Generic;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Scripting;
using StorybrewCommon.Mapset;
using OpenTK.Graphics;
using AnimeMixInfo;
using System.Linq;
using System.IO;
using OpenTK;
using System;
using Saheki;
using static Saheki.Helpers;

/*
    this effect needs clenup and some refactoring, sory for the bad code
*/


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
                Font = LoadFont(Path.Combine("sb", "FontIntro"), new FontDescription()
                {
                    FontPath = GetFontPath(AssetPath, FontName),
                    FontSize = FontSize,
                    Color = Color4.White,
                },
                new FontShadowClone()
                {
                    Distance = new Vector2(10, 10),
                    Color = new Color4(0, 0, 0, 255 * 1)
                },
                new FontGlow()
                {
                    Color = new Color4(0, 0, 0, 255),
                    Radius = 10,
                });

                var infos = Info.FromMultipleFolders(Path.Combine(AssetPath, "AnimeMixInfo"));

                GenerateBlackBackground(Start + Offset, End + Offset - 500);
                GenerateBackground(1501, End + Offset - 500, Delay, 1);
                RandomSlideShow(infos, 0, 1501, -15);
                GenerateBlackBackground2(Start, Start + 500);

                GenerateVignete(0, 2000);
                GenerateCmbLogo(Start + Offset, 2500, ScreenToOsu(1920 / 2, 1080 / 2));
                // Text.Generate(this, Font, $"Anime {Year} Mix", ScreenToOsu(1920 / 2, 1080 / 2), 1501, End + Offset - 500, 100, new TextFade()
                // {
                //     Delay = 500,
                //     FadeIn = false,
                // }, OsbOrigin.Centre);

            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        public void GenerateBackground(int startTime, int endTime, int fadeInOut, double opacity)
        {
            
            if (!File.Exists(Path.Combine(MapsetPath, Beatmap.BackgroundPath))) throw new Exception($"{Beatmap.BackgroundPath} is missing");

            var background = GetLayer("").CreateSprite(Beatmap.BackgroundPath, OsbOrigin.Centre);
            background.Scale(startTime, 480f / GetMapsetBitmap(Beatmap.BackgroundPath).Size.Height);
            background.Fade(startTime, opacity);
            background.Fade(OsbEasing.InOutSine, endTime - fadeInOut, endTime, opacity, 0);
        }

        private void GenerateBlackBackground(int startTime, int endTime)
        {
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var blackBackground = GetLayer("").CreateSprite(pixelPath);
            float scale = 480f / GetMapsetBitmap(pixelPath).Size.Height;
            blackBackground.ScaleVec(startTime, scale * (16f / 9f), scale);
            blackBackground.Color(startTime, Color4.Black);
            blackBackground.Fade(startTime, 1);
            blackBackground.Fade(endTime, endTime - 1, 1, 0);
        }

        private void GenerateBlackBackground2(int startTime, int endTime)
        {
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var blackBackground = GetLayer("").CreateSprite(pixelPath);
            float scale = 480f / GetMapsetBitmap(pixelPath).Size.Height;
            blackBackground.ScaleVec(startTime, scale * (16f / 9f), scale);
            blackBackground.Color(startTime, Color4.Black);
            blackBackground.Fade(startTime, 1);
            blackBackground.Fade(startTime, endTime - 1, 1, 0);
        }

        public void GenerateVignete(int startTime, int endTime)
        {
            const float vigneteOpacity = 1f;

                var vignetePath = Path.Combine("sb", "Vignete.png");
                if (!File.Exists(Path.Combine(MapsetPath, vignetePath))) throw new Exception($"{vignetePath} is missing");

                var vignete = GetLayer("").CreateSprite(vignetePath);
                vignete.ScaleVec(startTime, 854f / GetMapsetBitmap(vignetePath).Size.Width, 480f / GetMapsetBitmap(vignetePath).Size.Height);
                vignete.Fade(startTime, vigneteOpacity);
                vignete.Fade(endTime -250, endTime - 1, vigneteOpacity, 0);
        }

        public void GenerateCmbLogo(int startTime, int endTime, Vector2 position)
        {
            var logo = GetLayer("").CreateSprite(LogoPath, OsbOrigin.Centre, position);
            logo.Color(startTime + Delay, Color4.Black);
            logo.Move(startTime + Delay * 2, position + new Vector2(10, 10)); // new Vector2(-GetMapsetBitmap(LogoPath).Size.Width * LogoScale / 2, 0)
            logo.ScaleVec(startTime, new Vector2(LogoScale, LogoScale));
            // logo.Fade(OsbEasing.InOutSine, startTime, startTime + Delay, 0, 1);
            logo.Fade(OsbEasing.InOutSine, startTime, endTime - Delay, 0.66, 0.66);
            logo.Fade(OsbEasing.InOutSine, endTime - 500, endTime, 0.66, 0);

            var logo2 = GetLayer("").CreateSprite(LogoPath, OsbOrigin.Centre, position);
            logo2.Move(startTime + Delay * 2, position); // new Vector2(-GetMapsetBitmap(LogoPath).Size.Width * LogoScale / 2, 0)
            logo2.ScaleVec(startTime, new Vector2(LogoScale, LogoScale));
            // logo.Fade(OsbEasing.InOutSine, startTime, startTime + Delay, 0, 1);
            logo2.Fade(OsbEasing.InOutSine, startTime, endTime - Delay, 1, 1);
            logo2.Fade(OsbEasing.InOutSine, endTime - 500, endTime, 1, 0);
        }

        private void RandomSlideShow(List<Info> infos, int startTime, int endTime, double acceleration = 0)
        {
            if (acceleration < -100 || acceleration > 100)
                throw new Exception($"RandomSlideShow, acceleration \"{acceleration}\": Acceleration must be between -100 and 100.");

            int duration = endTime - startTime;
            int frames = (int)Math.Round(30 * duration / (double)1000);
            Log(frames);
            var images = WeightedImagesList(infos, frames + 8); // 8 is HOTFIX, WeightedImagesList is not generating the correct list size due to roundings 
            Log(images.Count);

            double accelerationFactor = 1 - (acceleration / 100);
            double interval;
            if (acceleration == 0) interval = (double)duration / frames;
            else interval = duration * (1 - accelerationFactor) / (1 - Math.Pow(accelerationFactor, frames));
            
            double currentTime = startTime;

            foreach (var image in images)
            {
                var sprite = GetLayer("").CreateSprite(image);
                float scale = 480f / GetMapsetBitmap(image).Size.Height;
                sprite.Scale(currentTime, scale);
                sprite.Fade(currentTime, 0.66);
                sprite.Fade(currentTime + interval, 0);

                // Update
                currentTime += interval;
                interval *= accelerationFactor;
            }
        }

        private List<string> WeightedImagesList(List<Info> infos, int frameCount)
        {
            var images = new List<string>();
            double totalScore = infos.Sum(info => info.Entry.Popularity);
            if (totalScore == 0) throw new Exception("WeightedImagesList: Total score cannot be zero.");
            double factor = frameCount / totalScore;

            // Generate the list of images
            foreach (var info in infos)
            {
                var name = info.Anime.Title;
                var image = Path.Combine("sb", LegalizeString(name), "Background.jpg");

                if (!File.Exists(Path.Combine(MapsetPath, image))) throw new Exception($"WeightedImagesList, Image \"{image}\": File not found.");

                // at least one
                int amount = (int)Math.Max(info.Entry.Popularity * factor, 1);

                for (int i = 0; i < amount; i++) images.Add(image);
            }

            // Shuffle the final list to randomize image order
            images = Shuffle(images);

            return images;
        }

        private static List<string> Shuffle(List<string> input)
        {
            Random random = new Random();
            List<string> result = new List<string>();
            bool validShuffle = false;

            while (!validShuffle)
            {
                // Shuffle
                result = input.OrderBy(_ => random.Next()).ToList();

                // Check for adjacent duplicates TODO: make this safe, this will hang if is impossible to avoid duplicates
                validShuffle = true;
                for (int i = 1; i < result.Count; i++)
                {
                    if (result[i] != result[i - 1]) continue;
                    validShuffle = false;
                    break;
                }
            }

            return result;
        }
    }
}
