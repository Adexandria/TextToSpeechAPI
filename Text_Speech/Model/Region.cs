using Newtonsoft.Json;
using System.Collections.Generic;

namespace Text_Speech.Model
{
    public class Region
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("lines")]
        public IList<Line> Line { get; set; }
    }
}