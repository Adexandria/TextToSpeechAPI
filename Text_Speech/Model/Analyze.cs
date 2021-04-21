using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TextTranslations.Model
{
    public class Analyze
    {
      
        public string language { get; set; }
        public string textAngle { get; set; }
        public string orientation { get; set; }
        [JsonProperty("regions")]
        public IList<RegionsItems> regions { get; set; }
       
    }
}
