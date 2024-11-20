using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StorybrewCommon.Subtitles;

namespace Saheki
{
    /// <summary>
    /// A single lyric, consisting of a start time and optionally an end time and a list of lines.
    /// </summary>
    public class Ly
    {
        /// <summary>
        /// The start time of the lyric in milliseconds.
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// The end time of the lyric in milliseconds, or null if the lyric has no defined end time.
        /// </summary>
        public Nullable<double> EndTime { get; set; }

        /// <summary>
        /// The text of each of the avalible lyric.
        /// </summary>
        public List<string> Text { get; set; } = new List<string>();

        /// <summary>
        /// Returns a string representation of the lyric, with the start time and optionally end time followed by each line of the lyric text
        /// enclosed in double quotes, separated by new lines.
        /// </summary>
        /// <returns>A string representation of the lyric.</returns>
        public override string ToString()
        {
            string timePart = EndTime != null ? $"{StartTime}|{EndTime.Value}" : StartTime.ToString();
            return timePart + Environment.NewLine + string.Join(Environment.NewLine, Text.Select(line => $"\"{line}\""));
        }

        /// <summary>
        /// Converts the lyric to a <see cref="SubtitleLine"/>.
        /// </summary>
        /// <param name="index">The index of the line of the lyric to convert.</param>
        /// <returns>A <see cref="SubtitleLine"/> representing the lyric.</returns>
        /// <exception cref="Exception">Thrown if <paramref name="index"/> is out of range or if <see cref="EndTime"/> is null.</exception>
        public SubtitleLine ToSubtitle(int index)
        {
            if (index >= Text.Count) throw new Exception($"Lyric ToSubtitle, index {index}: Index out of range.");
            if (EndTime == null) throw new Exception("Lyric ToSubtitle, EndTime is null");
            return new SubtitleLine(StartTime, (double)EndTime, Text[index]);
        }
    }

    /// <summary>
    /// Contains static methods for working with lyrics from .Ly Files
    /// </summary>
    public static class LyExtentions
    {
        /// <summary>
        /// Converts the given lyrics into a sequence of <see cref="SubtitleSet"/> objects, where each set contains a <see cref="SubtitleLine"/> for each line of the lyrics.
        /// </summary>
        /// <remarks>
        /// A single <see cref="SubtitleSet"/> is created for each line of the lyrics, and each set contains a <see cref="SubtitleLine"/> for each <see cref="Ly"/> in the input sequence.
        /// </remarks>
        public static IEnumerable<SubtitleSet> ToSubtitleSets(this IEnumerable<Ly> lyrics)
            => lyrics.First().Text.Select((_, i) => new SubtitleSet(lyrics.Select(l => l.ToSubtitle(i))));

        /// <summary>
        /// Deserializes a file at the specified path into a list of <see cref="Ly"/> objects.
        /// </summary>
        /// <param name="filePath">The path to the file containing lyric data.</param>
        /// <returns>A list of <see cref="Ly"/> objects deserialized from the file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist.</exception>
        public static List<Ly> LyFromFile(this string filePath)
        {
            if (!File.Exists(filePath)) throw new Exception($"Ly DeserializeFile, filePath {filePath}: File does not exist.");
            return LyFromString(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Serializes a list of <see cref="Ly"/> objects into a string and writes it into a file.
        /// </summary>
        /// <param name="lyrics">The list of <see cref="Ly"/> objects to serialize.</param>
        /// <param name="filePath">The path to the file to write the serialized data to.</param>
        /// <exception cref="Exception">Thrown if the file already exists.</exception>
        public static void LyToFile(this List<Ly> lyrics, string filePath)
        {
            if (File.Exists(filePath)) throw new Exception($"Ly SerializeFile, filePath {filePath}: File already exist.");
            File.WriteAllText(filePath, LyToString(lyrics));
        }

        /// <summary>
        /// Deserializes into a list of <see cref="Ly"/> objects.
        /// </summary>
        /// <param name="input">The path to the file containing lyric data.</param>
        /// <returns>A list of <see cref="Ly"/> objects deserialized from the file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if reading fails.</exception>
        /// <remarks>
        /// The file is expected to have lyrics in a specific format where each lyric consists of a start time and optionally
        /// an end time, followed by lines of text. The start and end times are separated by a '|'. Each line of the lyric text 
        /// is enclosed in double quotes.
        /// </remarks>
        public static List<Ly> LyFromString(this string input)
        {
            List<Ly> lyrics = new List<Ly>();
            using (var reader = new StringReader(input))
            {
                int character;
                while ((character = reader.Read()) != -1)
                {
                    var lyric = new Ly();

                    var timeBuilder = new System.Text.StringBuilder();
                    bool isEndTime = false;

                    while (character != -1 && character != '\n')
                    {
                        if ((char)character == '|')
                        {
                            lyric.StartTime = int.Parse(timeBuilder.ToString());
                            timeBuilder.Clear();
                            isEndTime = true;
                        }
                        else if (char.IsDigit((char)character))
                        {
                            timeBuilder.Append((char)character);
                        }

                        character = reader.Read();
                    }

                    if (!isEndTime) lyric.StartTime = int.Parse(timeBuilder.ToString());
                    else lyric.EndTime = int.Parse(timeBuilder.ToString());

                    while ((character = reader.Read()) != -1)
                    {
                        if ((char)character == '"')
                        {
                            var lineBuilder = new System.Text.StringBuilder();
                            bool isEscaped = false;

                            while ((character = reader.Read()) != -1)
                            {
                                if ((char)character == '\\' && !isEscaped)
                                {
                                    isEscaped = true;
                                }
                                else if ((char)character == '"' && !isEscaped) break;
                                else
                                {
                                    lineBuilder.Append((char)character);
                                    isEscaped = false;
                                }
                            }

                            lyric.Text.Add(lineBuilder.ToString());
                        }
                        else if (character == '\n' && reader.Peek() != '"') break;
                    }
                    lyrics.Add(lyric);
                }
            }
            return lyrics;
        }

        /// <summary>
        /// Serializes a list of <see cref="Ly"/> objects into a string representation.
        /// </summary>
        /// <param name="lyrics">The list of <see cref="Ly"/> objects to serialize.</param>
        /// <returns>A string representation of the serialized <see cref="Ly"/> objects, with each lyric's details
        /// separated by new lines.</returns>
        public static string LyToString(this List<Ly> lyrics)
        {
            return string.Join(Environment.NewLine, lyrics.Select(segment => segment.ToString()));
        }

        /// <summary>
        /// Returns a clone of the input list of lyrics with the end times set automatically.
        /// If a lyric does not have an end time, it is set to 1 millisecond less than the start time of the next lyric.
        /// If a lyric already has an end time, it is left unchanged.
        /// </summary>
        /// <param name="lyrics">The list of <see cref="Ly"/> objects to modify.</param>
        /// <returns>A clone of the input list of lyrics with end times set.</returns>
        public static List<Ly> AddLyEnds(this List<Ly> lyrics)
        {
            if (!lyrics.Last().EndTime.HasValue) throw new Exception("Last lyric does not have an end time");
            List<Ly> lyricsClone = new List<Ly>();
            for (int i = 0; i < lyrics.Count - 1; i++)
            {
                var lyric = lyrics[i];

                if (!lyric.EndTime.HasValue) lyric.EndTime = lyrics[i + 1].StartTime - 1;

                lyricsClone.Add(lyric);
            }
            lyricsClone.Add(lyrics.Last());
            return lyricsClone;
        }

        /// <summary>
        /// Returns a clone of the input list of lyrics with any automatically added end times removed.
        /// If a lyric's end time is equal to the start time of the next lyric minus one, it is set to null.
        /// </summary>
        /// <param name="lyrics">The list of <see cref="Ly"/> objects to modify.</param>
        /// <returns>A clone of the input list of lyrics with end times set.</returns>
        public static List<Ly> RemoveLyEnds(this List<Ly> lyrics)
        {
            List<Ly> lyricsClone = new List<Ly>();
            for (int i = 0; i < lyrics.Count - 1; i++)
            {
                var lyric = lyrics[i];
                if (lyric.EndTime != null && lyric.EndTime == lyrics[i + 1].StartTime - 1) lyric.EndTime = null;
                lyricsClone.Add(lyric);
            }
            return lyricsClone;
        }
    }
}
