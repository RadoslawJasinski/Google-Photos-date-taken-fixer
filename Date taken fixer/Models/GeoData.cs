using Newtonsoft.Json;

namespace Date_taken_fixer.Models
{
    public class GeoData
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("altitude")]
        public double Altitude { get; set; }

        [JsonProperty("latitudeSpan")]
        public double LatitudeSpan { get; set; }

        [JsonProperty("longitudeSpan")]
        public double LongitudeSpan { get; set; }
    }
}