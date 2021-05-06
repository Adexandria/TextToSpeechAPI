﻿using Newtonsoft.Json;

namespace TextTranslations.Model
{
    public class Word
    {
        [JsonProperty("boundingBox")]
        public string BoundingBox { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}