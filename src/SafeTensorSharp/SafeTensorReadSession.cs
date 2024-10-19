using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SafeTensorSharp
{
    /// <summary>
    /// Represents a safetensor reading session
    /// </summary>
    public class SafeTensorReadSession
    {
        SafeTensor st;
        private MemoryMappedFile mmf;
        internal SafeTensorReadSession(SafeTensor safeTensor,MemoryMappedFile memoryMappedFile)
        {
            st = safeTensor;
            mmf = memoryMappedFile;
        }

        /// <summary>
        /// Read the data to byte span
        /// </summary>
        /// <param name="itemName">Data section name</param>
        /// <param name="span">Target span, span size should not less than data length</param>
        public void ReadToSpan(string itemName,Span<byte> span)
        {
            using var stream = createReadStreamByItemName(itemName);
            stream.Read(span);
        }


        /// <summary>
        /// Asynchronously read the data to byte span
        /// </summary>
        /// <param name="itemName">Data section name</param>
        /// <param name="memory">Target memory, memory size should not less than data length</param>
        /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="System.Threading.CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous read operation. The value of its Result property contains the total number of bytes read into the buffer. </returns>
        public ValueTask<int> ReadToSpanAsync(string itemName,Memory<byte> memory,CancellationToken token=default)
        {
            using var stream = createReadStreamByItemName(itemName);
            return stream.ReadAsync(memory,token);
        }

        /// <summary>
        /// Read the data to a structure span
        /// </summary>
        /// <typeparam name="T">Type of items in target span</typeparam>
        /// <param name="itemName">Data section name</param>
        /// <param name="span">The span to write data</param>
        public void ReadToSpan<T>(string itemName,Span<T> span) where T:struct
        {
            Span<byte> tmp = MemoryMarshal.AsBytes(span);
            ReadToSpan(itemName, tmp);
        }


        /// <summary>
        /// Read the data to a stream
        /// </summary>
        /// <param name="itemName">Data section name</param>
        /// <param name="stream">The stream to write data</param>
        public void ReadToStream(string itemName, Stream stream)
        {
            using var sourceStream = createReadStreamByItemName(itemName);
            sourceStream.CopyTo(stream);
        }

        /// <summary>
        /// Asynchronously read the data to a stream
        /// </summary>
        /// <param name="itemName">Data section name</param>
        /// <param name="stream">The stream to write data</param>
        /// <param name="token">The token to monitor for cancellation requests. The default value is <see cref="System.Threading.CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        public Task ReadToStreamAsync(string itemName, Stream stream,CancellationToken token=default)
        {
            using var sourceStream = createReadStreamByItemName(itemName);
            return sourceStream.CopyToAsync(stream,token);
        }


        /// <summary>
        /// Get the data stream by section name. 
        /// </summary>
        /// <param name="name">Data section name</param>
        /// <returns>Data stream. Make sure dispose it after use</returns>
        private Stream createReadStreamByItemName(string name)
        {
            var item = st.Items[name];
            return mmf.CreateViewStream(item.DataStart + st.DataOffset, item.DataLength);
        }
    }
}
