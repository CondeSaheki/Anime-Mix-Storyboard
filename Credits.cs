using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.Collections.Generic;
using System.IO;
using Saheki;
using AnimeMixInfo;
using System.Text.RegularExpressions;
using static Saheki.Helpers;

namespace StorybrewScripts
{
    public class Credits : StoryboardObjectGenerator
    {
        [Group("General")]
        [Configurable] public int Start = 0;
        [Configurable] public int End = 0;
        [Configurable] public int Offset = 0;

        [Group("Fonts")]
        [Configurable] public string FontName = "";
        [Configurable] public int FontSize = 50;

        private FontGenerator Font;

        public override void Generate()
        {
            try
            {
                Font = LoadFont(Path.Combine("sb", "Credits"), new FontDescription()
                {
                    FontPath = GetFontPath(AssetPath, FontName),
                    FontSize = FontSize,
                    Color = Color4.White
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

                List<Credit> credits = Credit.FromFile(Path.Combine(AssetPath, "Credits.json"));

                const uint CharCount = 50;

                var timeStep = (int)Math.Round((double)(End - Start) / credits.Count);
                var currentTime = Start + Offset;
                var position = ScreenToOsu(1920 / 2, 1080 / 2);
                foreach (var credit in credits)
                {
                    var lines = JoinSpecial(credit.Members, " · ", CharCount).Split('\n');
                    
                    // Calculate Members height
                    var height = 0f;
                    foreach (var line in lines)
                    {
                        height -= Text.Size(Font, line, credit.MemberSize).Y;
                    }
                    var cursor = position + new Vector2(0, height) / 2;
                    
                    // Group
                    var scale = Text.ScaleFill(Font, credit.Group, ScreenToOsu(1920 * 3 / 4, 1080 * 3 / 4), credit.GroupSize);
                    Text.Generate(this, Font, credit.Group, cursor, currentTime, currentTime + timeStep - 1, scale, OsbOrigin.BottomCentre);
                    
                    // Members
                    foreach (var line in lines)
                    {
                        scale = Text.ScaleFill(Font, line, ScreenToOsu(1920 * 3 / 4, 1080 * 3 / 4), credit.MemberSize);
                        cursor.Y += Text.Generate(this, Font, line, cursor, currentTime, currentTime + timeStep - 1, scale, OsbOrigin.TopCentre).Height;
                    }

                    currentTime += timeStep;
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }
    }
}
