using Newtonsoft.Json;

namespace Date_taken_fixer.Models
{
    public class PhotoTime
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("formatted")]
        public string? Formatted { get; set; }
    }
}