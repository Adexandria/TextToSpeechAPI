using Newtonsoft.Json;
using System.Collections.Generic;

namespace Text_Speech.Model
{
    public class Line
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("words")]
        public IList<Word> Word { get; set; }
    }
}