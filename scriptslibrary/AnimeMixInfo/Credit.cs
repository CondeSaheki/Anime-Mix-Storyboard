using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AnimeMixInfo
{
    public class Credit
    {
        public string Group { get; set; } = string.Empty;
        public uint GroupSize { get; set; } = 100;
        public List<string> Members { get; set; } = new List<string>();
        public uint MemberSize { get; set; } = 50;

        /// <summary>
        /// Reads a JSON file from the specified file path and deserializes it into a List of Credit objects.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing credits data.</param>
        /// <returns>A List of Credit objects deserialized from the JSON file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if deserialization fails.</exception>
        public static List<Credit> FromFile(string jsonFilePath)
        {
            // nulabble is not avaliable the current c# version

            if (!File.Exists(jsonFilePath)) throw new Exception($"SongInfo FromFile, jsonFilePath {jsonFilePath}: File does not exist.");
            
            var content = File.ReadAllText(jsonFilePath);
            
            try
            {
                return JsonConvert.DeserializeObject<List<Credit>>(content);
            }
            catch (Exception ex)
            {
                // Make the broken file traceable
                throw new Exception($"Credit FromFile, jsonFilePath {jsonFilePath}: Deserialization failed. Exception: {ex}");
            }
        }
    }
}