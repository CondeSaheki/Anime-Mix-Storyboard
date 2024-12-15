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
        [Configurable] public float FontShadowOpacity = 100f / 3;
        [Configurable] public float FontShadowDistance = 6;

        // NOTE: 452 does not support switch expression.
        private readonly Dictionary<string, Color4> SeasonColors = new Dictionary<string, Color4>
        {
            { "summer1", new Color4(254, 0, 0, 255) },      //Combo2
            { "summer2", new Color4(255, 206, 13, 255) },   //Combo3
            { "winter1", new Color4(0, 92, 255, 255) },     //Combo4
            { "winter2", new Color4(35, 222, 255, 255) },   //Combo5
            { "spring1", new Color4(3, 216, 2, 255) },      //Combo6
            { "spring2", new Color4(235, 255, 0, 255) },    //Combo7
            { "fall1", new Color4(255, 117, 23, 255) },     //Combo8
            { "fall2", new Color4(242, 201, 158, 255) },    //Combo1
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

            // Apply offset
            foreach (var info in infos)
            {
                if (info.Entry.Offset == 0) continue;

                info.Entry.EntryTime += info.Entry.Offset;

                foreach (var part in info.Parts)
                {
                    part.StartTime += info.Entry.Offset;
                    if (part.EndTime.HasValue) part.EndTime += info.Entry.Offset;
                }

                if (info.Lyrics == null || info.Lyrics.Count == 0) continue;
                foreach (var lyric in info.Lyrics)
                {
                    lyric.StartTime += info.Entry.Offset;
                    if (lyric.EndTime.HasValue) lyric.EndTime += info.Entry.Offset;
                }
            }

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

                if (EnableEntry) Entry(info, ScreenToOsu(240, 540), info.Entry.EntryTime + Offset, PixelToOsu(1440, 1080));
                if (EnableEntryOverlay) EntryOverlay(info, ScreenToOsu(120 + 16, 120 + 16), startTime, endTime);
                if (EnableParts) Parts(info, ScreenToOsu(1800 - 16, 960 - 16));
                if (EnableLyrics) Lyrics(info.Lyrics, ScreenToOsu(120 + 16, 960 - 16), PixelToOsu(960, 1080), ScreenToOsu(1800 - 16, 120 + 16), PixelToOsu(1920, 840 - 120));
            }
        }

        private void Entry(Info info, Vector2 position, int startTime, Vector2 constraints)
        {

            var textConstraints = constraints;
            textConstraints.X -= 60;

            var time = startTime;
            var animation = Snap(Beatmap, time, 16, 1);
            var cursor = position;

            // anime

            cursor.X += 30;
            EntryAnimeCover(info, cursor, time, time + animation - 1);
            cursor.X += 30;
            cursor += PixelToOsu(16, 0);

            animation = Snap(Beatmap, time, 8, 1);
            EntryAnimeTitles(info, cursor, time, time + animation - 1, textConstraints);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntryAnimeStudio(info, cursor, time, time + animation - 1, textConstraints);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntryAnimeOtherInfo(info, cursor, time, time + animation - 1, textConstraints);
            time += animation;

            // song

            animation = Snap(Beatmap, time, 8, 1);
            cursor = position;
            cursor.X += 30;
            EntrySongIcon(cursor, time, time + animation - 1);
            cursor.X += 30;
            cursor += PixelToOsu(16, 0);

            animation = Snap(Beatmap, time, 4, 1);
            EntrySongTitles(info, cursor, time, time + animation - 1, textConstraints);
            time += animation;
            animation = Snap(Beatmap, time, 4, 1);
            EntrySongArtists(info, cursor, time, time + animation - 1, textConstraints);
            time += animation;

            // mappers

            animation = Snap(Beatmap, time, 8, 1);
            cursor = position;
            cursor.X += 30;
            EntryMappersIcon(cursor, time, time + animation - 1);
            cursor.X += 30;
            cursor += PixelToOsu(16, 0);

            animation = Snap(Beatmap, time, 8, 1);
            EntryMappers(info, cursor, time, time + animation - 1, textConstraints);
            // time += animation;
        }

        private Box2 EntryAnimeCover(Info info, Vector2 position, int startTime, int endTime)
        {
            const float CoverWidth = 56f;
            const float ShadowOffset = 2f;
            const float ScaleAdjustment = 1 - 25f / 100;
            const float ShadowOpacity = 0.66f;

            var animation = Snap(Beatmap, startTime, 1, 1);
            var moveOffset = Vector2.One * ShadowOffset;

            var animeCoverPath = Path.Combine("sb", LegalizeString(info.Anime.Title), "large_image.jpg");
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, animeCoverPath))) throw new Exception($"{animeCoverPath} is missing");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var coverBitmap = GetMapsetBitmap(animeCoverPath);
            var coverScale = CoverWidth / coverBitmap.Size.Width;
            var initialScale = coverScale * ScaleAdjustment;
            var coverSize = new Vector2(coverBitmap.Size.Width, coverBitmap.Size.Height) * coverScale;

            var coverShadow = GetLayer("").CreateSprite(pixelPath);
            var cover = GetLayer("").CreateSprite(animeCoverPath);

            // Intro
            cover.Move(startTime, position);
            cover.Scale(startTime, initialScale);
            cover.Fade(OsbEasing.Out, startTime, startTime + animation / 2, 0, 1);
            cover.Move(OsbEasing.OutBack, startTime, startTime + animation, position, position - moveOffset);
            cover.Scale(OsbEasing.OutBack, startTime, startTime + animation, initialScale, coverScale);

            coverShadow.Move(startTime, position);
            coverShadow.ScaleVec(startTime, coverSize * ScaleAdjustment);
            coverShadow.Color(startTime, Color.Black);
            coverShadow.Fade(OsbEasing.Out, startTime, startTime + animation / 2, 0, ShadowOpacity);
            coverShadow.ScaleVec(OsbEasing.OutBack, startTime, startTime + animation, coverSize * ScaleAdjustment, coverSize);
            coverShadow.Move(OsbEasing.OutBack, startTime, startTime + animation, position, position + moveOffset);

            // Outro
            cover.Fade(OsbEasing.In, endTime - animation / 2, endTime, 1, 0);
            cover.Move(OsbEasing.InBack, endTime - animation, endTime, position - moveOffset, position);
            cover.Scale(OsbEasing.InBack, endTime - animation, endTime, coverScale, initialScale);

            coverShadow.Fade(OsbEasing.In, endTime - animation / 2, endTime, ShadowOpacity, 0);
            coverShadow.Move(OsbEasing.InBack, endTime - animation, endTime, position + moveOffset, position);
            coverShadow.ScaleVec(OsbEasing.InBack, endTime - animation, endTime, coverSize, coverSize * ScaleAdjustment);

            // Calculate bounding box
            var minPosition = position - coverSize / 2 - moveOffset;
            var maxPosition = position + coverSize / 2 + moveOffset;
            return new Box2(minPosition, maxPosition);
        }

        private Box2 EntryMappersIcon(Vector2 position, int startTime, int endTime)
        {
            var profilePath = Path.Combine("sb", "Profile.png");
            if (!File.Exists(Path.Combine(MapsetPath, profilePath))) throw new Exception($"{profilePath} is missing");

            const float profileHeight = 57f;
            const float ShadowOffset = 2f;
            const float ScaleAdjustment = 1 - 25f / 100;;
            const float ShadowOpacity = 0.66f;

            var profileBitmap = GetMapsetBitmap(profilePath);
            float profileScale = profileHeight / profileBitmap.Size.Height;
            var initialScale = profileScale * ScaleAdjustment;
            var profileSize = new Vector2(profileBitmap.Size.Width, profileBitmap.Size.Height) * profileScale;

            var profileShadow = GetLayer("").CreateSprite(profilePath);
            var profile = GetLayer("").CreateSprite(profilePath);

            profileShadow.Color(startTime, Color.Black);

            // Intro animation
            profileShadow.Move(startTime, position);
            profileShadow.Scale(startTime, initialScale);
            profileShadow.Fade(OsbEasing.Out, startTime, startTime + Snap(Beatmap, startTime, 1, 1) / 2, 0, ShadowOpacity);
            profileShadow.Move(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), position, position + Vector2.One * ShadowOffset);
            profileShadow.Scale(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), initialScale, profileScale);

            profile.Move(startTime, position);
            profile.Scale(startTime, initialScale);
            profile.Fade(OsbEasing.Out, startTime, startTime + Snap(Beatmap, startTime, 1, 1) / 2, 0, 1);
            profile.Move(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), position, position - Vector2.One * ShadowOffset);
            profile.Scale(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), initialScale, profileScale);

            // Outro animation
            profileShadow.Fade(OsbEasing.In, endTime - Snap(Beatmap, startTime, 1, 1) / 2, endTime, ShadowOpacity, 0);
            profileShadow.Move(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, position + Vector2.One * ShadowOffset, position);
            profileShadow.Scale(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, profileScale, initialScale);

            profile.Fade(OsbEasing.In, endTime - Snap(Beatmap, startTime, 1, 1) / 2, endTime, 1, 0);
            profile.Move(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, position - Vector2.One * ShadowOffset, position);
            profile.Scale(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, profileScale, initialScale);

            return new Box2(position - profileSize / 2, position + profileSize / 2);
        }

        private Box2 EntrySongIcon(Vector2 position, int startTime, int endTime)
        {
            var diskPath = Path.Combine("sb", "Disk.png");
            if (!File.Exists(Path.Combine(MapsetPath, diskPath))) throw new Exception($"{diskPath} is missing");

            const float diskHeight = 57f;
            const float ShadowOffset = 2f;
            const float ScaleAdjustment = 0.10f;
            const float ShadowOpacity = 0.66f;

            var diskBitmap = GetMapsetBitmap(diskPath);
            float diskScale = diskHeight / diskBitmap.Size.Height;
            var initialScale = diskScale * (1 - ScaleAdjustment);
            var diskSize = new Vector2(diskBitmap.Size.Width, diskBitmap.Size.Height) * diskScale;

            var diskShadow = GetLayer("").CreateSprite(diskPath);
            var disk = GetLayer("").CreateSprite(diskPath);

            diskShadow.Color(startTime, Color.Black);

            // Intro animation
            diskShadow.Move(startTime, position);
            diskShadow.Scale(startTime, initialScale);
            diskShadow.Fade(OsbEasing.Out, startTime, startTime + Snap(Beatmap, startTime, 1, 1) / 2, 0, ShadowOpacity);
            diskShadow.Move(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), position, position + Vector2.One * ShadowOffset);
            diskShadow.Scale(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), initialScale, diskScale);

            disk.Move(startTime, position);
            disk.Scale(startTime, initialScale);
            disk.Fade(OsbEasing.Out, startTime, startTime + Snap(Beatmap, startTime, 1, 1) / 2, 0, 1);
            disk.Move(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), position, position - Vector2.One * ShadowOffset);
            disk.Scale(OsbEasing.OutBack, startTime, startTime + Snap(Beatmap, startTime, 1, 1), initialScale, diskScale);

            // Outro animation
            diskShadow.Fade(OsbEasing.In, endTime - Snap(Beatmap, startTime, 1, 1) / 2, endTime, ShadowOpacity, 0);
            diskShadow.Move(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, position + Vector2.One * ShadowOffset, position);
            diskShadow.Scale(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, diskScale, initialScale);

            disk.Fade(OsbEasing.In, endTime - Snap(Beatmap, startTime, 1, 1) / 2, endTime, 1, 0);
            disk.Move(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, position - Vector2.One * ShadowOffset, position);
            disk.Scale(OsbEasing.InBack, endTime - Snap(Beatmap, startTime, 1, 1), endTime, diskScale, initialScale);

            return new Box2(position - diskSize / 2, position + diskSize / 2);
        }


        private void EntryAnimeTitles(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var texts = new List<TextItem>
            {
                new TextItem(info.Anime.AlternativeTitles.Ja, FontJa, FontSize3),
                new TextItem(info.Anime.Title, Font, FontSize1),
                new TextItem(info.Anime.AlternativeTitles.En, Font, FontSize3)
            };
            texts = texts.GroupBy(x => x.Text).Select(x => x.First()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryAnimeStudio(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);
            var studios = string.Join(" · ", info.Anime.Studios.Select(s => s.Name));

            var texts = new List<TextItem>
            {
                new TextItem(studios, Font, FontSize2)
            };
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryAnimeOtherInfo(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);
            var generes = string.Join(" · ", info.Anime.Genres.Select(g => g.Name));
            // var episodes = $"{info.Anime.NumEpisodes} Episodes";
            var score = $"{info.Anime.Mean} MyAnimeList";
            var season = info.Anime.StartSeason.Season.First().ToString().ToUpper() + info.Anime.StartSeason.Season.Substring(1);
            if (info.Anime.StartSeason.Year != 2024) Log($"{info.Anime.Title} Season: {season} Year: {info.Anime.StartSeason.Year}");

            var texts = new List<TextItem>();

            if (info.Anime.Mean != 0) texts.Add(new TextItem(score, Font, FontSize3));
            texts.Add(new TextItem(season, Font, FontSize2));
            texts.Add(new TextItem(generes, Font, FontSize3));
            // if(info.Anime.NumEpisodes != 0) texts.Add(new TextItem(episodes, Font, FontSize4));

            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntrySongTitles(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var texts = new List<TextItem>
            {
                new TextItem(info.Song.Title, FontJa, FontSize3),
                new TextItem(info.Song.TitleRomanised, Font, FontSize1),
            };

            texts = texts.GroupBy(x => x.Text).Select(x => x.Last()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntrySongArtists(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var texts = new List<TextItem>
            {
                new TextItem(info.Song.Artist, FontJa, FontSize3),
                new TextItem(info.Song.ArtistRomanised, Font, FontSize2),
            };

            texts = texts.GroupBy(x => x.Text).Select(x => x.Last()).ToList();
            GenerateTextBox(this, texts, position, startTime, endTime, constraints, effect);
        }

        private void EntryMappers(Info info, Vector2 position, int startTime, int endTime, Vector2 constraints)
        {
            var effect = new TextUpDownEffect(Beatmap);

            var mappers = string.Join(" · ", info.Entry.Mappers);

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
                bpm = GetBpm(Beatmap, startTime, endTime, 2000) + " BPM";
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

            VerticalProgressBar(position + PixelToOsu(5, 5), startTime, endTime, barSize, Color4.Black, 1f / 2);
            VerticalProgressBar(position, startTime, endTime, barSize, Color4.White);
        }

        private void Parts(Info info, Vector2 position)
        {
            if (info.Parts.Count == 0)
            {
                Log(info.Entry.Number + " has no parts");
                return;
            }

            info.Parts.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
            for (int i = 0; i < info.Parts.Count - 1; i++)
            {
                var part = info.Parts[i];
                var nextPart = info.Parts[i + 1];

                if (!part.EndTime.HasValue || part.EndTime == nextPart.StartTime)
                {
                    Mapper(Font, part.Name, position, part.StartTime + Offset, nextPart.StartTime - 1 + Offset);
                    continue;
                }

                Mapper(Font, part.Name, position, part.StartTime + Offset, part.EndTime.Value + Offset);
            }

            var lastPart = info.Parts.Last();

            if (lastPart.EndTime.HasValue)
            {
                Mapper(Font, lastPart.Name, position, lastPart.StartTime + Offset, lastPart.EndTime.Value + Offset);
            }
            else
            {
                var entryObjects = Beatmap.HitObjects.Where(h => h.StartTime >= info.Entry.StartTime && h.StartTime < info.Entry.EndTime).OrderBy(h => h.StartTime);
                if (!entryObjects.Any())
                {
                    Mapper(Font, lastPart.Name, position, lastPart.StartTime + Offset, info.Entry.EndTime + Offset);
                    Log("Parts for Entry " + info.Entry.Number + " do no have end time, using entry end time");
                    return;
                }

                Mapper(Font, lastPart.Name, position, lastPart.StartTime + Offset, (int)entryObjects.Last().EndTime);
                // Log("Parts for Entry " + info.Entry.Number + " do no have end time, using last object");
            }
        }

        private void Mapper(FontGenerator Font, string name, Vector2 position, int startTime, int endTime)
        {
            const int barWidth = 8;

            var effect = new TextUpDownEffect(Beatmap);

            var textHeight = Text.Generate(this, Font, name, position - PixelToOsu(barWidth, 0), startTime, endTime, FontSize3, effect, OsbOrigin.BottomRight).Height;
            var barSize = new Vector2(PixelToOsu(barWidth), textHeight);

            position.Y -= textHeight;
            VerticalProgressBar(position + PixelToOsu(5, 5), startTime, endTime, barSize, Color4.Black, 1f / 2);
            VerticalProgressBar(position, startTime, endTime, barSize, Color4.White);
        }

        private void Lyrics(List<Ly> lyrics, Vector2 position, Vector2 constraints, Vector2 positionJa, Vector2 constraintsJa)
        {
            if (lyrics == null) return; // NOTE: Hotfix 452

            const bool ResnapLyrics = true;
            const int SnapNumerator = 3;
            const int SnapDenominator = 2;

            var effectJa = new TextUpDownEffect(Beatmap) { MovementMultiply = 1f / 8 };
            var effect = new TextUpDownEffect(Beatmap);

            // resnap lyrics and calculate ends
            var lyricSnaped = lyrics;
            if (ResnapLyrics)
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

                var size = Text.ScaleFill(Font, lyric.Text[0], constraints, FontSize3);
                Text.Generate(this, Font, lyric.Text[0], position, startTime, endTime, size, effect, OsbOrigin.BottomLeft);

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

                GenerateVerticalTextBox(this, texts, positionJa, startTime, endTime, constraintsJa, effectJa, OsbOrigin.TopCentre);
            }
        }

        private void GenerateBackground(string backgroundPath, int startTime, int endTime)
        {
#pragma warning disable CS0162

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

#pragma warning restore CS0162
        }

        // other functions

        private void VerticalProgressBar(Vector2 position, int startTime, int endTime, Vector2 size, Color4 color, float opacity = 1f)
        {
            var pixelPath = Path.Combine("sb", "Pixel.png");
            if (!File.Exists(Path.Combine(MapsetPath, pixelPath))) throw new Exception($"{pixelPath} is missing");

            var animation = Snap(Beatmap, startTime, 1, 1);
            if (endTime - (startTime + animation) <= animation) animation = Snap(Beatmap, startTime, 1, 4);
            if (endTime - (startTime + animation) <= animation) throw new Exception($"VerticalProgressBar, startTime {startTime}, endTime {endTime}: Interval is too small");

            var flat = new Vector2(size.X, 0);
            var positionOffset = position + new Vector2(size.X, 0) / 2;
            var height = new Vector2(0, size.Y);
            var bar = GetLayer("").CreateSprite(pixelPath);
            bar.Color(startTime, color);

            // intro
            bar.Fade(startTime, startTime + animation, 0, opacity);
            bar.ScaleVec(OsbEasing.OutBack, startTime, startTime + animation, flat, size);
            bar.Move(OsbEasing.OutBack, startTime, startTime + animation, positionOffset - new Vector2(0, size.Y / 2), positionOffset + height / 2);

            // outro
            bar.Fade(endTime, 0);
            bar.ScaleVec(startTime + animation, endTime, size, flat);
            bar.Move(startTime + animation, endTime, positionOffset + height / 2, positionOffset + height);
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