using System;
using System.Collections.Generic;
using System.Linq;
using StorybrewCommon.Mapset;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// Gets a string representing the BPM range of a given beatmap in the range [startTime, endTime].
        /// The string format is "minBPM-maxBPM(dominantBPM)" where dominantBPM is the BPM of the timing point that lasts the longest within the range.
        /// </summary>
        /// <param name="beatmap">The beatmap containing timing points.</param>
        /// <param name="startTime">The start time for the BPM calculation.</param>
        /// <param name="endTime">The end time for the BPM calculation.</param>
        /// <param name="negativeOffset">An optional offset to account for redlines with negative offset. Default is 1000.</param>
        /// <returns>A string representing the BPM range as "minBPM-maxBPM(dominantBpm)".</returns>
        /// <exception cref="Exception">Thrown if no redlines exist in the specified range.</exception>
        public static string GetBpm(Beatmap beatmap, int startTime, int endTime, int negativeOffset = 1000)
        {
            // Filter redlines and objects that fall within the specified time range
            var relevantRedlines = beatmap.TimingPoints.Where(tp => tp.Offset >= startTime - negativeOffset && tp.Offset <= endTime).ToList();
            var relevantObjects = beatmap.HitObjects.Where(o => o.StartTime >= startTime && o.EndTime <= endTime).ToList();

            // Filter redlines ranges with objects
            if (relevantObjects.Count != 0)
            {
                    relevantRedlines = relevantRedlines
                        .Where(tp =>
                        {
                            // Calculate the end of the current redline's range
                            var rangeEnd = relevantRedlines
                                .Where(nextTp => nextTp.Offset > tp.Offset)
                                .Select(nextTp => nextTp.Offset)
                                .DefaultIfEmpty(endTime) // Use endTime if no next timing point
                                .Min();

                            // Check if any object overlaps with the redline range
                            return relevantObjects.Any(o => o.StartTime < rangeEnd && o.EndTime > tp.Offset);
                        })
                        .ToList();
            }

            if (relevantRedlines.Count == 0) throw new Exception("GetBpm, No redlines in range");
            if (relevantRedlines.Count == 1) return $"{Math.Round(relevantRedlines[0].Bpm, 2)}";

            double minBPM = relevantRedlines.Min(tp => tp.Bpm);
            double maxBPM = relevantRedlines.Max(tp => tp.Bpm);

            // Calculate the redline that lasts the longest within the range
            double dominantBpm = 0;
            double longestDuration = 0;
            for (int i = 0; i < relevantRedlines.Count - 1; ++i)
            {
                var currentRedline = relevantRedlines[i];
                double duration = relevantRedlines[i + 1].Offset - currentRedline.Offset;

                if (duration > longestDuration)
                {
                    longestDuration = duration;
                    dominantBpm = currentRedline.Bpm;
                }
            }
            var durationLast = endTime - relevantRedlines.Last().Offset;
            if (durationLast > longestDuration)
            {
                longestDuration = durationLast;
                dominantBpm = relevantRedlines.Last().Bpm;
            }

            if(Math.Round(minBPM, 2) == Math.Round(maxBPM, 2) && Math.Round(dominantBpm, 2) == Math.Round(minBPM, 2)) return $"{Math.Round(minBPM, 2)}*";

            return $"{Math.Round(minBPM, 2)}-{Math.Round(maxBPM, 2)}({Math.Round(dominantBpm, 2)})";
        }
    }
}