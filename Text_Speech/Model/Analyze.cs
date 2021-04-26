using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class Analyze
    {

        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("textAngle")]
        public string TextAngle { get; set; }
        [JsonProperty("orientation")]
        public string Orientation { get; set; }
        [JsonProperty("regions")]
        public IList<RegionsItems> Regions { get; set; }
       
    }
}
