using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class RegionsItems
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("lines")]
        public IList<Lines> Lines { get; set; }
    }
}