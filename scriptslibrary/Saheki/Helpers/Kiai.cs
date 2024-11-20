using System.Collections.Generic;
using System.Linq;
using StorybrewCommon.Mapset;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// Returns a list of timestamps of hitobject finishes within Kiai sections.
        /// </summary>
        /// <returns>A list of timestamps of hitobject finishes.</returns>
        public static List<double> GetBeatmapHitFinishesInKiai(Beatmap beatmap, double audioDuration)
        {
            var controlPoints = beatmap.ControlPoints.Concat(beatmap.TimingPoints).OrderBy(controlPoint => controlPoint.Offset).ToList();

            var kiaiFinishes = new List<double>();
            bool inKiai = false;
            double kiaiStart = 0;

            foreach (var controlPoint in controlPoints)
            {
                if (controlPoint.IsKiai != inKiai)
                {
                    if (inKiai)
                    {
                        kiaiFinishes.AddRange(
                            beatmap.HitObjects
                                .Where(obj => obj.StartTime >= kiaiStart && 
                                    obj.StartTime <= controlPoint.Offset && 
                                    (
                                        (obj.AdditionsSampleSet == SampleSet.Soft || obj.AdditionsSampleSet == SampleSet.Normal) &&
                                        obj.Additions == HitSoundAddition.Finish
                                    )
                                )   
                                .Select(obj => obj.StartTime));
                    }
                    else
                    {
                        kiaiStart = controlPoint.Offset;
                    }
                    inKiai = !inKiai;
                }
            }

            if (inKiai)
            {
                kiaiFinishes.AddRange(
                    beatmap.HitObjects
                        .Where(obj => obj.StartTime >= kiaiStart && 
                            obj.StartTime <= audioDuration && 
                            (
                                (obj.AdditionsSampleSet == SampleSet.Soft || obj.AdditionsSampleSet == SampleSet.Normal) &&
                                obj.Additions == HitSoundAddition.Finish
                            )
                        )   
                        .Select(obj => obj.StartTime));
            }
            return kiaiFinishes;
        }

        public class KiaiSection
        {
            public double StartTime { get; set; }
            public double EndTime { get; set; }
        }

        /// <summary>
        /// Retrieves a list of Kiai sections from the beatmap's control points.
        /// </summary>
        /// <returns>A list of KiaiSection objects, each representing the start and end times of a Kiai section.</returns>
        public static List<KiaiSection> GetKiais(Beatmap beatmap, double audioDuration)
        {
            List<ControlPoint> controlPoints = beatmap.ControlPoints.Concat(beatmap.TimingPoints).ToList();
            controlPoints.Sort((x, y) => x.Offset.CompareTo(y.Offset));

            var kiais = new List<KiaiSection>();
            bool inKiai = false;
            double kiaiStartTime = 0;

            foreach (var controlPoint in controlPoints)
            {
                if (controlPoint.IsKiai != inKiai)
                {
                    if (inKiai)
                        kiais.Add(new KiaiSection { StartTime = kiaiStartTime, EndTime = controlPoint.Offset });
                    else
                        kiaiStartTime = controlPoint.Offset;
                    inKiai = !inKiai;
                }
            }
            if (inKiai) kiais.Add(new KiaiSection { StartTime = kiaiStartTime, EndTime = audioDuration });

            return kiais;
        }

        /// <summary>
        /// Returns a list of timestamps of hitobject finishes
        /// </summary>
        /// <returns>A list of timestamps of hitobject finishes.</returns>
        public static List<double> GetHitFinishes(Beatmap beatmap) => beatmap.HitObjects
            .Where(obj => (obj.AdditionsSampleSet == SampleSet.Soft || obj.AdditionsSampleSet == SampleSet.Normal) && obj.Additions == HitSoundAddition.Finish)
            .Select(obj => obj.StartTime)
            .ToList();
        
        /// <summary>
        /// Returns whether the map is in a kiai section at the given <paramref name="time"/>.
        /// If the given time is between a timing point and a control point, the one that is closer to the given time is chosen.
        /// </summary>
        /// <param name="time">The time to query.</param>
        /// <returns>True if the map is in a kiai section at the given time; false otherwise.</returns>
        public static bool IskiaiAt(Beatmap beatmap, int time)
        {
            // not sure if this is needed needs to be tested

            var redLine = beatmap.GetTimingPointAt(time);
            var greenLine = beatmap.GetControlPointAt(time);
            var closest = time - redLine.Offset > greenLine.Offset - time ? greenLine : redLine;
            return closest.IsKiai;
        }
    }
}