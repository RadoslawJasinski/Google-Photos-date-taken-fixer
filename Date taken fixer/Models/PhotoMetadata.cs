using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date_taken_fixer.Models
{
    internal class PhotoMetadata
    {
        [JsonProperty("photoTakenTime")]
        public PhotoTime? PhotoTakenTime { get; set; }

        [JsonProperty("geoData")]
        public GeoData? GeoData { get; set; }
    }
}
