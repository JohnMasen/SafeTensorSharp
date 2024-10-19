using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SafeTensorSharp
{
    /// <summary>
    /// Represents a header item in Safetensor file
    /// </summary>
    public class HeaderItem
    {
        /// <summary>
        /// Data type of tensor data, possible values are "I8","I16","F16"...
        /// </summary>
        [JsonPropertyName("dtype")]
        public string DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Data shape of tensor data
        /// </summary>
        [JsonPropertyName("shape")]
        public int[] Shape
        {
            get;
            set;
        }

        /// <summary>
        /// Data offset described in header file, do not use it in your code. please use <see cref="DataStart"/> and <see cref="DataLength"/> instead 
        /// </summary>
        [JsonPropertyName("data_offsets")]
        public long[] DataOffsets
        {
            get;
            set;
        }

        /// <summary>
        /// Position of first tensor data byte, offset to te data content start position.
        /// The actual file position is <see cref="DataStart"/> + <see cref="SafeTensor.DataOffset"/>
        /// </summary>

        [JsonIgnore]
        public long DataStart => DataOffsets[0];
        /// <summary>
        /// Tensor data length in bytes
        /// </summary>
        [JsonIgnore]
        public long DataLength => DataOffsets[1]-DataOffsets[0];
    }
}
