using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;

using AnimeMixInfo;
using Saheki;
using static Saheki.Helpers;

namespace StorybrewScripts
{
    public class AnimeMixEntry : StoryboardObjectGenerator
    {
        [Group("Geral")]
        [Configurable] public int Offset = 0;

        [Configurable] public bool EnableEntry = true;
        [Configurable] public bool EnableEntryOverlay = true;
        [Configurable] public bool EnableParts = true;
        [Configurable] public bool EnableLyrics = true;
        [Configurable] public bool EnableBackground = false;

        [Group("Fonts")]
        [Configurable] public string FontPath = ""; // Torus-Regular.ttf
        [Configurable] public int FontSize = 50;
        [Configurable] public string FontJaPath = ""; // MPLUS1p-Regular.ttf
        [Configurable] public int FontJaSize = 50;
        [Configurable] public int FontGlowRadius = 8;
        [Configurable] public float FontShadowOpacity = 100f / 3 ;
        [Configurable] public float FontShadowDistance = 6;        

        // NOTE: 452 does not support switch expression.
        private readonly Dictionary<string, Color4> SeasonColors = new Dictionary<string, Color4>
        {
            { "winter1", new Color4(0, 92, 255, 255) },
            { "winter2", new Color4(35, 222, 255, 255) },
            { "spring1", new Color4(3, 216, 2, 255) },
            { "spring2", new Color4(235, 255, 0, 255) },
            { "summer1", new Color4(254, 0, 0, 255) },
            { "summer2", new Color4(255, 206, 13, 255) },
            { "fall1", new Color4(255, 117, 23, 255) },
            { "fall2", new Color4(242, 201, 158, 255) },
        };
        private Color4 GetGetSeasonColor(Info info, int color) => SeasonColors[$"{info.Anime.StartSeason.Season}{color}"];

        private const int FontSize1 = 100;
        private const int FontSize2 = 75;
        private const int FontSize3 = 50;
        private const int FontSize4 = 25;

        private FontGenerator Font;
        private FontGenerator FontJa;

        public override void Generate()
        {
            // read all entrys infos
            var infos = Info.FromMultipleFolders(Path.Combine(AssetPath, "AnimeMixInfo"));

            // load fonts
            #pragma warning disable CA1416
            Font = LoadFont(Path.Combine("sb", "Font"), new FontDescription()
            {
                FontPath = GetFontPath(AssetPath, FontPath),
                FontSize = FontSize,
                FontStyle = FontStyle.Bold,
                Color = Color.White,
            },
            new FontShadowClone()
            {
                Distance = new Vector2(FontShadowDistance, FontShadowDistance),
                Color = new Color4(0, 0, 0, 255 * FontShadowOpacity / 100)
            },
            new FontGlow()
            {
                Color = new Color4(0, 0, 0, 255 * FontShadowOpacity / 100),
                Radius = FontGlowRadius,
            });

            FontJa = LoadFont(Path.Combine("sb", "FontJa"), new FontDescription()
            {
                FontPath = GetFontPath(AssetPath, FontJaPath),
                FontSize = FontJaSize,
                FontStyle = FontStyle.Bold,
                Color = Color.White,
            },
            new FontShadowClone()
            {
                Distance = new Vector2(FontShadowDistance, FontShadowDistance),
                Color = new Color4(0, 0, 0, 255 * FontShadowOpacity / 100)
            },
            new FontGlow()
            {
                Color = new Color4(0, 0, 0, 255),
                Radius = FontGlowRadius,
            });
            #pragma warning restore CA1416

            // generate all entries effects
            foreach (var info in infos)
            {
                var startTime = info.Entry.StartTime + Offset;
                var endTime = info.Entry.EndTime + Offset;

                if (EnableBackground)
                {
                    const int animation = 500;
                    var backgroundPath = Path.Combine("sb", LegalizeString(info.Anime.Title), "Background.jpg");
                    if (!File.Exists(Path.Combine(MapsetPath, backgroundPath)))
                    {
                        Log($"Background is missing for entry {info.Entry.Number}");
                        GenerateBackground(Beatmap.BackgroundPath, startTime - animation, endTime + animation); 
                    }
                    else GenerateBackground(backgroundPath, startTime - animation, endTime + animation); 
                }

                if (EnableEntry) Entry(info, ScreenToOsu(303, 540), startTime, endTime);
                if (EnableEntryOverlay) EntryOverlay(info, ScreenToOsu(96, 214), startTime, endTime);
                if (EnableParts)
                {
                    foreach (var part in info.Parts)
                    {
                        Parts(Font, part.Name, ScreenToOsu(1800, 900), part.StartTime + Offset, part.EndTime + Offset);
                    }
                }
                if (EnableLyrics) Lyrics(info.Lyrics);
            }
        }

        private void Entry(Info info, Vector2 position, int startTime, int endTime)
        {
            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            // anime

            var time = startTime;
            var animation = Snap(Beatmap, time, 16, 1);
            EntryAnimeCover(info, position - PixelToOsu(10, 0), time, time + animation -1);
            // time += animation;

            animation = Snap(Beatmap, time, 8, 1);
            EntryAnimeTitles(info, position, time, time + animation -1);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntryAnimeStudio(info, position, time, time + animation -1);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntryAnimeOtherInfo(info, position, time, time + animation -1);
            time += animation;
            
            // song

            animation = Snap(Beatmap, time, 8, 1);
            EntrySongIcon(position, time, time + animation -1);
            // time += animation;

            animation = Snap(Beatmap, time, 4, 1);
            EntrySongTitles(info, position, time, time + animation -1);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntrySongArtists(info, position, time, time + animation -1);
            time += animation;
            
            // mappers

            animation = Snap(Beatmap, time, 8, 1);
            EntryMappersIcon(position, time, time + animation -1);
            // time += animation;

            animation = Snap(Beatmap, time, 8, 1);
            EntryMappers(info, position, time, time + animation -1);
            time += animation;
        }

        private void EntryAnimeCover(Info info, Vector2 position, int startTime, int endTime)
        {
            const float CoverHeight = 81f;

            var animation = Snap(Beatmap, startTime, 1, 1);

            var animeCoverPath = Path.Combine("sb", LegalizeString(info.Anime.Title), "large_image.png");
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, animeCoverPath))) throw new Exception($"{animeCoverPath} is missing");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var seasonColor1 = GetGetSeasonColor(info, 1);
            var seasonColor2 = GetGetSeasonColor(info, 2);

            var coverbitmap = GetMapsetBitmap(animeCoverPath);
            var coverScale = CoverHeight / coverbitmap.Size.Height;
            var coverSize = new Vector2(coverbitmap.Size.Width, coverbitmap.Size.Height) * coverScale;
            var initialSize = new Vector2(0, CoverHeight);

            var cover = GetLayer("").CreateSprite(animeCoverPath);
            var coverShadow = GetLayer("").CreateSprite(pixelPath);

            coverShadow.Color(OsbEasing.InOutSine, startTime, endTime, seasonColor1, seasonColor2);
            // intro
            coverShadow.Move(OsbEasing.InOutSine, startTime, startTime + animation, position, position - new Vector2(coverSize.X / 2, 0));
            coverShadow.ScaleVec(OsbEasing.InOutSine, startTime, startTime + animation, initialSize, coverSize);
            coverShadow.Fade(startTime, 1);
            coverShadow.Fade(startTime + animation, startTime + animation * 2, 1, 0);

            cover.Move(startTime + animation, position - new Vector2(coverSize.X, 0) / 2);
            cover.Scale(startTime + animation, coverScale);
            cover.Fade(startTime + animation, 1);

            // outro
            cover.Fade(endTime - animation, endTime, 1, 0);

            // coverShadow.Fade(endTime - animation / 2, endTime, 1, 0);
            // coverShadow.Move(OsbEasing.InOutSine, endTime - animation, endTime, position - new Vector2(coverSize.X / 2, 0), position);
            // coverShadow.ScaleVec(OsbEasing.InOutSine, endTime - animation, endTime, coverSize, initialSize);
            // coverShadow.Fade(endTime - animation, endTime, 1, 1);
        }

        private void EntryMappersIcon(Vector2 position, int startTime, int endTime)
        {
            var profilePath = Path.Combine("sb", "Profile.png");
            if (!File.Exists(Path.Combine(MapsetPath, profilePath))) throw new Exception($"{profilePath} is missing");

            const float profileHeight = 57f;

            var profilebitmap = GetMapsetBitmap(profilePath);
            float profileScale = profileHeight / profilebitmap.Size.Height;
            var profileSize = new Vector2(profilebitmap.Size.Width, profilebitmap.Size.Height) * profileScale;

            var profile = GetLayer("").CreateSprite(profilePath);
            profile.Scale(startTime, profileScale);
            profile.Move(startTime, position - new Vector2(profileSize.X, 0) / 2);
            profile.Fade(startTime, endTime, 1, 1);
        }

        private void EntrySongIcon(Vector2 position, int startTime, int endTime)
        {
            var diskPath = Path.Combine("sb", "Disk.png");
            if (!File.Exists(Path.Combine(MapsetPath, diskPath))) throw new Exception($"{diskPath} is missing");

            const float diskHeight = 57f;

            var diskbitmap = GetMapsetBitmap(diskPath);
            float diskScale = diskHeight / diskbitmap.Size.Height;
            var diskSize = new Vector2(diskbitmap.Size.Width, diskbitmap.Size.Height) * diskScale;

            var disk = GetLayer("").CreateSprite(diskPath);
            disk.Scale(startTime, diskScale);
            disk.Move(startTime, position - new Vector2(diskSize.X, 0) / 2);
            disk.Fade(startTime, endTime, 1, 1);
            disk.Rotate(OsbEasing.InOutBack, startTime, startTime + (endTime - startTime) / 2, MathHelper.DegreesToRadians(100), MathHelper.DegreesToRadians(97.5));
            disk.Rotate(OsbEasing.InOutBack, startTime + (endTime - startTime) / 2, endTime, MathHelper.DegreesToRadians(97.5), MathHelper.DegreesToRadians(102.5));
        }

        private void EntryAnimeTitles(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(info.Anime.AlternativeTitles.Ja, FontJa, FontSize3),
                new TextItem(info.Anime.Title, Font, FontSize1),
                new TextItem(info.Anime.AlternativeTitles.En, Font, FontSize3)
            };
            texts = texts.GroupBy(x => x.Text).Select(x => x.First()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryAnimeStudio(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);
            var studios = string.Join(" · ", info.Anime.Studios.Select(s => s.Name));

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(studios, Font, FontSize2)
            };
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryAnimeOtherInfo(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);
            var generes = string.Join(" · ", info.Anime.Genres.Select(g => g.Name));
            var episodes = $"{info.Anime.NumEpisodes} Episodes";

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(generes, Font, FontSize3),
                new TextItem(episodes, Font, FontSize3)
            };
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntrySongTitles(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(info.Song.Title, FontJa, FontSize1),
                new TextItem(info.Song.TitleRomanised, Font, FontSize3),
            };

            texts = texts.GroupBy(x => x.Text).Select(x => x.First()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntrySongArtists(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(info.Song.Artist, FontJa, FontSize2),
                new TextItem(info.Song.ArtistRomanised, Font, FontSize3),
            };

            texts = texts.GroupBy(x => x.Text).Select(x => x.First()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryMappers(Info info, Vector2 position, int startTime, int endTime)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var mappers = string.Join(" · ", info.Entry.Mappers);

            var constraints = ScreenToOsu(1920 * 3 / 4, 1080);

            var texts = new List<TextItem>
            {
                new TextItem(mappers, Font, FontSize1)
            };
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryOverlay(Info info, Vector2 position, int startTime, int endTime)
        {
            const int barWidth = 8;

            var effect = new TextUpDownEffect(Beatmap);

            var cursor = position + PixelToOsu(barWidth * 2, 0);
            string bpm;
            try
            {
                bpm = GetBpm(Beatmap, startTime, endTime) + " BPM";
            }
            catch
            {
                Log($"Failed to calculate BPM for Entry {info.Entry.Number}");
                bpm = "???-???(???) BPM";
            }

            // TODO: Maybe add more infos about the the entry? like max combo or polularity

            cursor.Y += Text.Generate(this, Font, bpm, cursor, startTime, endTime, FontSize4, effect).Height;
            cursor.Y += Text.Generate(this, Font, $"#{info.Entry.Number}", cursor, startTime, endTime, FontSize2, effect).Height;

            var barSize = new Vector2(PixelToOsu(barWidth), cursor.Y - position.Y);

            VerticalProgressBar(position + PixelToOsu(5, 5), startTime, endTime, barSize, Color4.Black, 1f / 3);
            VerticalProgressBar(position, startTime, endTime, barSize, Color4.White);
        }

        private void Parts(FontGenerator Font, string name, Vector2 position, int startTime, int endTime)
        {
            const int barWidth = 8;

            var effect = new TextUpDownEffect(Beatmap);

            var textHeight = Text.Generate(this, Font, name, position - PixelToOsu(barWidth, 0), startTime, endTime, FontSize3, effect, OsbOrigin.TopRight).Height;
            var barSize = new Vector2(PixelToOsu(barWidth), textHeight);

            VerticalProgressBar(position + PixelToOsu(5, 5), startTime, endTime, barSize, Color4.Black, 1f / 3);
            VerticalProgressBar(position, startTime, endTime, barSize, Color4.White);
        }

        private void Lyrics(List<Ly> lyrics)
        {
            if (lyrics == null) return; // NOTE: Hotfix 452
            
            const bool ResnapLyrics = true;
            const int SnapNumerator = 2;
            const int SnapDenominator = 1;

            var lyricPosition = ScreenToOsu(100, 900);
            var lyricJaPosition = ScreenToOsu(1800, 215);

            var lyricContraints = ScreenToOsu(1920 * 3 / 4, 1080);
            var lyricJaContraints = ScreenToOsu(1920, 1080 / 2);

            var effect = new TextDefaultEffect(); // new TextUpDownEffect(Beatmap);

            // resnap lyrics and calculate ends
            var lyricSnaped = lyrics;
            if(ResnapLyrics)
            {
                for (int i = 0; i < lyricSnaped.Count; i++)
                {
                    var lyric = lyricSnaped[i];
                    lyric.StartTime -= Snap(Beatmap, (int)lyric.StartTime, SnapNumerator, SnapDenominator);
                }
            }
            lyricSnaped = lyrics.AddLyEnds();

            foreach (var lyric in lyricSnaped)
            {
                var startTime = (int)lyric.StartTime + Offset;
                var endTime = (int)lyric.EndTime.Value + Offset;

                // bottom lyrics romanji / english

                var size = Text.ScaleFill(Font, lyric.Text[0], lyricContraints, FontSize3);
                Text.Generate(this, Font, lyric.Text[0], lyricPosition, startTime, endTime, size, effect);

                // Right lyrics Ja

                if (lyrics.Count < 2) continue; // no Ja lyrics

                var lyricsJaUnformated = lyric.Text[1];
                var texts = new List<TextItem>();
                var regex = new Regex(@"(?<ja>[^{]+)|{(?<en>[^}]+)}");
                
                // match {en} or {ja} lyrics for apropriate fomat
                foreach (Match match in regex.Matches(lyricsJaUnformated))
                {
                    if (match.Groups["ja"].Success)
                    {
                        texts.Add(new TextItem(match.Groups["ja"].Value, FontJa, FontSize3, Text.Orientation.Vertical));
                    }
                    else if (match.Groups["en"].Success)
                    {
                        var specialText = match.Groups["en"].Value;
                        var specialWords = specialText.Split(' ');
                        foreach (var word in specialWords) texts.Add(new TextItem(word, FontJa, FontSize4));
                    }
                }

                GenerateVerticalTextBox(this, texts, lyricJaPosition, startTime, endTime, lyricJaContraints, effect, OsbOrigin.TopCentre);
            }
        }

        private void GenerateBackground(string backgroundPath, int startTime, int endTime)
        {
            #pragma warning disable CS0162 // Unreachable code detected

            const bool FadeIn = true;
            const bool FadeOut = true;
            const bool Vignete = true;
            const bool Shake = true;
            const float Opacity = 1f;
            float AngleVariation = MathHelper.DegreesToRadians(5);
            const float MovementScale = 0.25f;
            const int fade = 500;

            // background
            var background = GetLayer("").CreateSprite(backgroundPath);
            background.Scale(startTime, 480f / GetMapsetBitmap(backgroundPath).Size.Height);


            if (FadeIn) background.Fade(OsbEasing.InOutSine, startTime, startTime + fade, 0, Opacity);
            else background.Fade(startTime, Opacity);


            if (FadeOut) background.Fade(OsbEasing.InOutSine, endTime - fade, endTime, Opacity, 0);
            else background.Fade(OsbEasing.InOutSine, endTime - 1, endTime, Opacity, 0);

            if (Shake)
            {
                int a = (int)(16 * MovementScale / 2); // Semi-major axis for 16:9 aspect ratio
                int b = (int)(9 * MovementScale / 2);  // Semi-minor axis

                double eccentricity = Math.Sqrt(1 - Math.Pow(b, 2) / Math.Pow(a, 2)); // Ellipse eccentricity

                var timeStep = Beatmap.GetTimingPointAt(startTime).BeatDuration / 0.125;
                for (double i = startTime; i <= endTime; i += timeStep)
                {
                    // Adjust timeStep based on Kiai or low BPM
                    if (Beatmap.GetControlPointAt((int)i).IsKiai || Beatmap.GetTimingPointAt((int)i).Bpm < 150)
                    {
                        timeStep = Beatmap.GetTimingPointAt((int)i).BeatDuration / 0.25;
                    }
                    else
                    {
                        timeStep = Beatmap.GetTimingPointAt((int)i).BeatDuration / 0.125;
                    }

                    // Generate a random angle avoiding too high values, maintaining the movement within ellipse
                    double randomAngle = Random(0, 2 * Math.PI);
                    
                    // Calculate the maximum distance for this random angle along the ellipse
                    double maxDistance = Math.Sqrt(Math.Pow(b, 2) / (1 - eccentricity * eccentricity * Math.Pow(Math.Cos(randomAngle), 2)));
                    double distance = Random(0, Math.Min(10, maxDistance)); // Constrain to a max movement distance (10 in this example)

                    // Calculate new position based on polar coordinates
                    float newX = (float)(background.PositionAt((int)i).X + distance * Math.Cos(randomAngle));
                    float newY = (float)(background.PositionAt((int)i).Y + distance * Math.Sin(randomAngle));

                    // Apply movement and rotation with easing
                    background.Move(OsbEasing.InOutSine, i, i + timeStep, background.PositionAt((int)i).X, background.PositionAt(i).Y, newX, newY);
                    background.Rotate(OsbEasing.InOutSine, i, i + timeStep, background.RotationAt(i), MathHelper.DegreesToRadians(Random(-AngleVariation, AngleVariation)));
                }
            }

            // vignette
            if (Vignete)
            {
                const float vigneteOpacity = 1f;
                
                var vignetePath = Path.Combine("sb", "Vignete.png");
                if (!File.Exists(Path.Combine(MapsetPath, vignetePath))) throw new Exception($"{vignetePath} is missing");

                var vignete = GetLayer("").CreateSprite(vignetePath);
                vignete.ScaleVec(startTime, 854f / GetMapsetBitmap(vignetePath).Size.Width, 480f / GetMapsetBitmap(vignetePath).Size.Height);
                vignete.Fade(startTime, vigneteOpacity);
                vignete.Fade(endTime, endTime - 1, vigneteOpacity, 0);
            }

            #pragma warning restore CS0162 // Unreachable code detected
        }

        // other functions

        private void VerticalProgressBar(Vector2 position, int startTime, int endTime, Vector2 size, Color4 color, float opacity = 1f)
        {
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var animation = Snap(Beatmap, startTime, 1, 1);
            if (endTime - startTime <= animation) animation = Snap(Beatmap, startTime, 1, 4);

            var flat = new Vector2(size.X, 0);
            var positionOffset = position + new Vector2(size.X, 0) / 2;
            var height = new Vector2(0, size.Y);
            var bar = GetLayer("").CreateSprite(pixelPath);
            bar.Color(startTime, color);

            // intro
            bar.Fade(startTime, opacity);
            bar.ScaleVec(OsbEasing.Out, startTime, startTime + animation, flat, size);
            bar.Move(OsbEasing.Out, startTime, startTime + animation, positionOffset, positionOffset + height / 2);

            // outro
            bar.Fade(endTime, 0);
            bar.ScaleVec(OsbEasing.In, startTime + animation, endTime, size, flat);
            bar.Move(OsbEasing.In, startTime + animation, endTime, positionOffset + height / 2, positionOffset + height);
        }

        private void BlackBackground(int startTime, int endTime)
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
    }
}