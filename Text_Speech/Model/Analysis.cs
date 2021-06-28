using Newtonsoft.Json;
using System.Collections.Generic;

namespace Text_Speech.Model
{
    public class Analysis
    {

        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("textAngle")]
        public string TextAngle { get; set; }
        [JsonProperty("orientation")]
        public string Orientation { get; set; }
        [JsonProperty("regions")]
        public IList<Region> Region { get; set; }
       
    }
}
