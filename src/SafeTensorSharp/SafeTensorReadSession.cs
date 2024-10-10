using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

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
        public SafeTensorReadSession ReadToSpan(string itemName,Span<byte> span)
        {
            var item = st.Items[itemName];
            using var stream = mmf.CreateViewStream(item.DataStart+st.DataOffset,item.DataLength);
            stream.Read(span);
            return this;
        }


        public SafeTensorReadSession ReadToSpan<T>(string itemName,Span<T> span) where T:struct
        {
            Span<byte> tmp = MemoryMarshal.AsBytes(span);
            return ReadToSpan(itemName, tmp);
        }
    }
}
