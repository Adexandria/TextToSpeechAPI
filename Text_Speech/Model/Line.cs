using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class Line
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("words")]
        public IList<Word> Word { get; set; }
    }
}