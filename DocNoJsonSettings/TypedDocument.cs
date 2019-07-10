using Newtonsoft.Json;
using System;

namespace DocNoJsonSettings
{
    public class TypedDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("tag")]
        public string Tag { get => nameof(TypedDocument); }

        [JsonProperty("localTime")]
        public DateTimeOffset LocalTime { get; set; }

        [JsonProperty("utcTime")]
        public DateTimeOffset UtcTime { get; set; }
    }
}