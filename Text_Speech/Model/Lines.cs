using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextTranslations.Model
{
    public class Lines
    {
        public string boundingBox { get; set; }
        [JsonProperty("words")]
        public IList<Words> words { get; set; }
    }
}