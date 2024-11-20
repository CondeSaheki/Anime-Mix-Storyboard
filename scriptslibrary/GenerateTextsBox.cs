using System;
using System.Collections.Generic;
using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using static Saheki.Text;

namespace Saheki
{
    public static partial class Helpers
    {
        /*
            TODO: REFACTOR EVERYTHING HERE
        */

        public class TextItem
        {
            public string Text { get; set; }
            public FontGenerator Font { get; set; }
            public uint IntendedSize { get; set; }
            public Orientation Orientation { get; set; }

            public TextItem(string text, FontGenerator font, uint intendedSize, Saheki.Text.Orientation orientation = Orientation.Horizontal)
            {
                Text = text;
                Font = font;
                IntendedSize = intendedSize;
                Orientation = orientation;
            }
        }

        public static void GenerateTextBox(StoryboardObjectGenerator script,
            List<TextItem> texts,
            Vector2 position,
            int startTime,
            int endTime,
            Vector2 constraints, IEffect effect)
        {
            if (texts == null || texts.Count == 0)
                throw new ArgumentException("No texts provided to display.");

            float multiplier = 1f;
            int iterationLimit = 100;
            int iterationCount = 0;

            var scaledSizes = new List<uint>();

            // Calculate scaling multiplier
            while (true)
            {
                if (multiplier <= 0)
                    throw new Exception($"Size overflow: multiplier reached {multiplier}");
                if (++iterationCount > iterationLimit)
                    throw new Exception("Iteration limit exceeded while calculating text sizes.");

                bool allFit = true;
                scaledSizes.Clear();

                foreach (var textItem in texts)
                {
                    uint scale = ScaleFill(textItem.Font, textItem.Text, constraints, (uint)(textItem.IntendedSize * multiplier));
                    scaledSizes.Add(scale);

                    if (scale != (uint)(textItem.IntendedSize * multiplier))
                    {
                        multiplier *= (float)scale / textItem.IntendedSize;
                        allFit = false;
                        break;
                    }
                }

                if (allFit) break;
            }

            // Calculate total height of scaled texts
            var cursor = Vector2.Zero;
            float totalHeight = 0;

            for (int i = 0; i < texts.Count; i++)
            {
                var textItem = texts[i];
                uint size = scaledSizes[i];
                totalHeight += Text.Size(textItem.Font, textItem.Text, size).Y;
            }

            // Center texts vertically
            cursor = position - new Vector2(0, totalHeight / 2);

            // Render texts
            for (int i = 0; i < texts.Count; i++)
            {
                var textItem = texts[i];
                uint size = scaledSizes[i];

                cursor.Y += Text.Generate(script, textItem.Font, textItem.Text, cursor, startTime, endTime, size, effect, OsbOrigin.TopLeft, Orientation.Horizontal).Height;
            }
        }

        public static void GenerateVerticalTextBox(StoryboardObjectGenerator script,
            List<TextItem> texts,
            Vector2 position,
            int startTime,
            int endTime,
            Vector2 constraints, 
            IEffect effect,
            OsbOrigin origin = OsbOrigin.TopRight)
        {
            Vector2 totalSize = Vector2.Zero;
            foreach (var textItem in texts)
            {
                var dimensions = Size(textItem.Font, textItem.Text, textItem.IntendedSize, Orientation.Vertical);
                totalSize.X = Math.Max(dimensions.X, totalSize.X);
                totalSize.Y += dimensions.Y;
            }
            var multiplier = Math.Min(Math.Min(constraints.X / totalSize.X, constraints.Y / totalSize.Y), 1);    
            
            var cursor = position;
            for (int i = 0; i < texts.Count; i++)
            {
                var textItem = texts[i];
                var size = (uint)Math.Round(textItem.IntendedSize * multiplier);

                cursor.Y += Text.Generate(script, textItem.Font, textItem.Text, cursor, startTime, endTime, size, effect, origin, textItem.Orientation).Height;
            }
        }
    }
}