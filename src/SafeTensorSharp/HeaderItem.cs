using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SafeTensorSharp
{
    public class HeaderItem
    {
        [JsonPropertyName("dtype")]
        public string DataType
        {
            get;
            set;
        }
        [JsonPropertyName("shape")]
        public int[] Shape
        {
            get;
            set;
        }
        [JsonPropertyName("data_offsets")]
        public long[] DataOffsets
        {
            get;
            set;
        }
        public long DataStart => DataOffsets[0];
        public long DataLength => DataOffsets[1]-DataOffsets[0];
    }
}
