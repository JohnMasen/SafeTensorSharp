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
        /// Read the data from file and write to byte span
        /// </summary>
        /// <param name="itemName">The name of data section</param>
        /// <param name="span">Target span, span size should not less than data length</param>
        /// <returns>The <see cref="SafeTensorReadSession"/> object for chain calling</returns>
        public void ReadToSpan(string itemName,Span<byte> span)
        {
            using var stream = createReadStreamByItemName(itemName);
            stream.Read(span);
        }

        public ValueTask<int> ReadToSpanAsync(string itemName,Memory<byte> memory,CancellationToken token=default)
        {
            using var stream = createReadStreamByItemName(itemName);
            return stream.ReadAsync(memory,token);
        }


        public void ReadToSpan<T>(string itemName,Span<T> span) where T:struct
        {
            Span<byte> tmp = MemoryMarshal.AsBytes(span);
            ReadToSpan(itemName, tmp);
        }

        public void ReadToStream(string itemName, Stream stream)
        {
            using var sourceStream = createReadStreamByItemName(itemName);
            sourceStream.CopyTo(stream);
        }

        public Task ReadToStreamAsync(string itemName, Stream stream,CancellationToken token=default)
        {
            using var sourceStream = createReadStreamByItemName(itemName);
            return sourceStream.CopyToAsync(stream,token);
        }

        private Stream createReadStreamByItemName(string name)
        {
            var item = st.Items[name];
            return mmf.CreateViewStream(item.DataStart + st.DataOffset, item.DataLength);
        }
    }
}
