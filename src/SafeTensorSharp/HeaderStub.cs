using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SafeTensorSharp
{
    public class HeaderStub
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Items
        {
            get;
            set;
        }

        [JsonPropertyName("__metadata__")]
        public JsonElement Meta { get; set; }
    }
    
}
