using Newtonsoft.Json;

namespace Text_Speech.Model
{
    public class Word
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}