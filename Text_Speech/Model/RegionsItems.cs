using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class RegionsItems
    {
        public string boundingBox { get; set; }
        [JsonProperty("lines")]
        public IList<Lines> lines { get; set; }
    }
}