using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AnimeMixInfo
{
    public static class AnimeInfo
    {
        /// <summary>
        /// Reads a JSON file from the specified file path and deserializes it into an Anime object.
        /// </summary>
        /// <param name="jsonFilePath">The path to the JSON file containing anime data.</param>
        /// <returns>An Anime object deserialized from the JSON file.</returns>
        /// <exception cref="Exception">Thrown if the file does not exist or if deserialization fails.</exception>
        public static Anime FromFile(string jsonFilePath)
        {
            // nulabble is not avaliable the current c# version

            if (!File.Exists(jsonFilePath)) throw new Exception($"AnimeInfo FromFile, jsonFilePath {jsonFilePath}: File does not exist.");

            var content = File.ReadAllText(jsonFilePath);

            try
            {
                return JsonConvert.DeserializeObject<Anime>(content);
            }
            catch (Exception ex)
            {
                // Make the broken file traceable
                throw new Exception($"AnimeInfo FromFile, jsonFilePath {jsonFilePath}: Deserialization failed. Exception: {ex}");
            }
        }
    }
    public class MainPicture
    {
        [JsonProperty("medium")]
        public string Medium { get; set; } = string.Empty;

        [JsonProperty("large")]
        public string Large { get; set; } = string.Empty;
    }

    public class AlternativeTitles
    {
        [JsonProperty("synonyms")]
        public List<string> Synonyms { get; set; } = new List<string>();

        [JsonProperty("en")]
        public string En { get; set; } = string.Empty;

        [JsonProperty("ja")]
        public string Ja { get; set; } = string.Empty;
    }

    public class Genre
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class StartSeason
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; } = string.Empty;
    }

    public class Broadcast
    {
        [JsonProperty("day_of_the_week")]
        public string DayOfTheWeek { get; set; } = string.Empty;

        [JsonProperty("start_time")]
        public string StartTime { get; set; } = string.Empty;
    }

    public class Studio
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Status
    {
        [JsonProperty("watching")]
        public int Watching { get; set; } = 0;

        [JsonProperty("completed")]
        public int Completed { get; set; } = 0;

        [JsonProperty("on_hold")]
        public int OnHold { get; set; } = 0;

        [JsonProperty("dropped")]
        public int Dropped { get; set; } = 0;

        [JsonProperty("plan_to_watch")]
        public int PlanToWatch { get; set; } = 0;
    }

    public class Statistics
    {
        [JsonProperty("status")]
        public Status Status { get; set; } = new Status();

        [JsonProperty("num_list_users")]
        public int NumListUsers { get; set; } = 0;
    }


    public class Anime
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("main_picture")]
        public MainPicture MainPicture { get; set; } = new MainPicture();

        [JsonProperty("alternative_titles")]
        public AlternativeTitles AlternativeTitles { get; set; } = new AlternativeTitles();

        [JsonProperty("start_date")]
        public string StartDate { get; set; } = string.Empty;

        [JsonProperty("end_date")]
        public string EndDate { get; set; } = string.Empty;

        [JsonProperty("synopsis")]
        public string Synopsis { get; set; } = string.Empty;

        [JsonProperty("mean")]
        public double Mean { get; set; } = 0;

        [JsonProperty("rank")]
        public int Rank { get; set; } = 0;

        [JsonProperty("popularity")]
        public int Popularity { get; set; } = 0;

        [JsonProperty("num_list_users")]
        public int NumListUsers { get; set; } = 0;

        [JsonProperty("num_scoring_users")]
        public int NumScoringUsers { get; set; } = 0;

        [JsonProperty("nsfw")]
        public string Nsfw { get; set; } = string.Empty;

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonProperty("media_type")]
        public string MediaType { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        [JsonProperty("num_episodes")]
        public int NumEpisodes { get; set; } = 0;

        [JsonProperty("start_season")]
        public StartSeason StartSeason { get; set; } = new StartSeason();

        [JsonProperty("broadcast")]
        public Broadcast Broadcast { get; set; } = new Broadcast();

        [JsonProperty("source")]
        public string Source { get; set; } = string.Empty;

        [JsonProperty("average_episode_duration")]
        public int AverageEpisodeDuration { get; set; } = 0;

        [JsonProperty("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonProperty("studios")]
        public List<Studio> Studios { get; set; } = new List<Studio>();

        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; } = new Statistics();
    }
}