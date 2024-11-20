
using System;
using System.Collections.Generic;
using System.IO;
using Saheki;

namespace AnimeMixInfo
{
    public class Info
    {
        public Entry Entry;
        public Anime Anime;
        public Song Song;
        public List<Part> Parts;
        public List<Ly> Lyrics { get; set; }

        /// <summary>
        /// Reads a folder containing the following files and deserializes them into properties:
        /// <list type="bullet">
        /// <item>Entry.json - An Entry object</item>
        /// <item>Anime.json - An Anime object</item>
        /// <item>Song.json - A Song object</item>
        /// <item>Parts.json - A List of Part objects</item>
        /// <item>Lyrics.ly - A List of BasicLy objects</item>
        /// </list>
        /// </summary>
        /// <param name="folderPath">The path to the folder containing the above JSON files</param>
        /// <exception cref="Exception">Thrown if the folder does not exist, flie does not exist or if deserialization fails</exception>
        public Info(string folderPath)
        {
            if (!Directory.Exists(folderPath)) throw new Exception($"Info, folderPath {folderPath}: Directory does not exist");

            Entry = EntrysInfo.FromFile(Path.Combine(folderPath, "Entry.json"));
            Anime = AnimeInfo.FromFile(Path.Combine(folderPath, "Anime.json"));
            Song = SongInfo.FromFile(Path.Combine(folderPath, "Song.json"));
            Parts = PartsInfo.FromFile(Path.Combine(folderPath, "Parts.json"));
            
            // TODO: Remove try catch 
            try
            {
                Lyrics = Path.Combine(folderPath, "Lyrics.ly").LyFromFile();
            }
            catch
            {
                Lyrics = null;
            }
        }

        /// <summary>
        /// Loads a list of Infos from the given folder path by assuming each folder inside the given folder path
        /// contains a json file named AnimeInfo.json.
        /// </summary>
        /// <param name="folderPath">The path to the folder containing the folders with the json files.</param>
        /// <returns>The list of loaded Infos.</returns>
        /// <exception cref="Exception">Thrown if the folder does not exist.</exception>
        public static List<Info> FromMultipleFolders(string folderPath)
        {
            List<Info> infos = new List<Info>();

            if (!Directory.Exists(folderPath)) throw new Exception($"LoadInfos, folderPath {folderPath}: Directory does not exist");

            var folders = Directory.GetDirectories(folderPath);

            //throw new Exception($"Found {string.Join("\n", folders)} folders");

            foreach (var folder in folders)
            {
                var info = new Info(folder);
                infos.Add(info);   
            }

            return infos;
        }
    }
}