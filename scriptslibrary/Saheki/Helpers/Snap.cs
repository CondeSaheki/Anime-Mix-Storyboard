using System;
using StorybrewCommon.Mapset;

namespace Saheki
{
    public partial class Helpers
    {
        /// <summary>
        /// Get the time duration of the snap.
        /// </summary>
        /// <param name="beatmap">The beatmap.</param>
        /// <param name="time">The time to get the snap.</param>
        /// <param name="numerator">The numerator of the snap fraction.</param>
        /// <param name="denominator">The denominator of the snap fraction.</param>
        /// <returns>The snap duration at the given time.</returns>
        public static int Snap(Beatmap beatmap, int time, int numerator, int denominator)
        {
            var beat = beatmap.GetTimingPointAt(time).BeatDuration;
            return (int)Math.Round(beat * numerator / denominator );
        }
    }
}