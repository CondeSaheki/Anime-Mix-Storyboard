using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AnimeMixInfo
{
    public static class PartsInfo
    {

        /// <summary>
        /// Reads a JSON file from the specified file path and deserializes it into a <see cref="List{Part}"/> object.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing parts data.</param>
        /// <returns>A <see cref="List{Part}"/> object deserialized from the JSON file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if deserialization fails.</exception>
        public static List<Part> FromFile(string jsonFilePath)
        {
            // nulabble is not avaliable the current c# version

            if (!File.Exists(jsonFilePath)) throw new Exception($"AnimeInfo FromFile, jsonFilePath {jsonFilePath}: File does not exist.");
            
            var content = File.ReadAllText(jsonFilePath);
            
            try
            {
                return JsonConvert.DeserializeObject<List<Part>>(content);
            }
            catch (Exception ex)
            {
                // Make the broken file traceable
                throw new Exception($"AnimeInfo FromFile, jsonFilePath {jsonFilePath}: Deserialization failed. Exception: {ex}");
            }
        }
    }

    public class Part
    {
        public string Name { get; set; } = string.Empty;
        public int StartTime { get; set; } = 0;
        public int EndTime { get; set; } = 0;
    }
}