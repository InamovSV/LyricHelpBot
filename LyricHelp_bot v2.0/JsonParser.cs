// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var result = Result.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Producers
    {
        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image_url")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("is_meme_verified")]
        public bool IsMemeVerified { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("annotation_count")]
        public long AnnotationCount { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("full_title")]
        public string FullTitle { get; set; }

        [JsonProperty("header_image_thumbnail_url")]
        public Uri HeaderImageThumbnailUrl { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("lyrics_owner_id")]
        public long LyricsOwnerId { get; set; }

        [JsonProperty("lyrics_state")]
        public string LyricsState { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("pyongs_count")]
        public string PyongsCount { get; set; }

        [JsonProperty("song_art_image_thumbnail_url")]
        public Uri SongArtImageThumbnailUrl { get; set; }

        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_with_featured")]
        public string TitleWithFeatured { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("primary_artist")]
        public PrimaryArtist PrimaryArtist { get; set; }
    }

    public partial class PrimaryArtist
    {
        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image_url")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("is_meme_verified")]
        public bool IsMemeVerified { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("iq")]
        public long Iq { get; set; }
    }

    public partial class Stats
    {
        [JsonProperty("hot")]
        public bool Hot { get; set; }

        [JsonProperty("unreviewed_annotations")]
        public long UnreviewedAnnotations { get; set; }

        [JsonProperty("concurrents")]
        public long Concurrents { get; set; }

        [JsonProperty("pageviews")]
        public long Pageviews { get; set; }
    }

    public partial class Producers
    {
        public static List<Producers> FromJson(string json) => JsonConvert.DeserializeObject<List<Producers>>(json, QuickType.Converter.Settings);
    }

    public partial class Result
    {
        public static Result FromJson(string json) => JsonConvert.DeserializeObject<Result>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Result self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        public static string ToJson(this List<Producers> self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
