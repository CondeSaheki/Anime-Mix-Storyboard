using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AnimeMixInfo
{
    public static class EntrysInfo
    {
        /// <summary>
        /// Reads a JSON file from the specified file path and deserializes it into an Entry object.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing entry data.</param>
        /// <returns>An Entry object deserialized from the JSON file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if deserialization fails.</exception>
        public static Entry FromFile(string jsonFilePath)
        {
            // nulabble is not avaliable the current c# version

            if (!File.Exists(jsonFilePath)) throw new Exception($"EntrysInfo FromFile, jsonFilePath {jsonFilePath}: File does not exist.");
            
            var content = File.ReadAllText(jsonFilePath);
            
            try
            {
                return JsonConvert.DeserializeObject<Entry>(content);
            }
            catch (Exception ex)
            {
                // Make the broken file traceable
                throw new Exception($"EntrysInfo FromFile, jsonFilePath {jsonFilePath}: Deserialization failed. Exception: {ex}");
            }
        }
    }

    public class Entry
    {
        public int Number { get; set; } = 0;
        public string Style { get; set; } = string.Empty;
        public int Popularity { get; set; } = 0;
        public List<string> Mappers { get; set; } = new List<string>();
        public int Offset { get; set; } = 0;
        public int EntryTime { get; set; } = 0;
        public int StartTime { get; set; } = 0;
        public int EndTime { get; set; } = 0;
    }
}