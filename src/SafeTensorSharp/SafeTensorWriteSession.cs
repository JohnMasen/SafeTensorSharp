using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace SafeTensorSharp
{
    public class SafeTensorWriteSession
    {
        MemoryMappedFile mmf;
        List<(HeaderItem, Stream)> Items;
        public SafeTensorWriteSession(string path)
        {
            mmf = MemoryMappedFile.CreateFromFile(path, System.IO.FileMode.CreateNew);
        }
    }
}
