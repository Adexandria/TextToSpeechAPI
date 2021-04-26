using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class Lines
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("words")]
        public IList<Words> Words { get; set; }
    }
}