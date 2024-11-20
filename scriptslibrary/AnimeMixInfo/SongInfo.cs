using System;
using System.IO;
using Newtonsoft.Json;

namespace AnimeMixInfo
{
    public static class SongInfo
    {
        /// <summary>
        /// Reads a JSON file from the specified file path and deserializes it into a Song object.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing song data.</param>
        /// <returns>A Song object deserialized from the JSON file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if deserialization fails.</exception>
        public static Song FromFile(string jsonFilePath)
        {
            // nulabble is not avaliable the current c# version

            if (!File.Exists(jsonFilePath)) throw new Exception($"SongInfo FromFile, jsonFilePath {jsonFilePath}: File does not exist.");
            
            var content = File.ReadAllText(jsonFilePath);
            
            try
            {
                return JsonConvert.DeserializeObject<Song>(content);
            }
            catch (Exception ex)
            {
                // Make the broken file traceable
                throw new Exception($"SongInfo FromFile, jsonFilePath {jsonFilePath}: Deserialization failed. Exception: {ex}");
            }
        }
    }

    public class Song
    {
        public string Title { get; set; } = string.Empty;
        public string TitleRomanised { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string ArtistRomanised { get; set; } = string.Empty;
    }
}