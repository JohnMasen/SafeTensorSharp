using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SafeTensorSharp
{
    public class SafeTensor 
    {
        internal readonly string filePath;
        private bool disposedValue;
        public JsonElement MetaData { get; internal set; }

        public Dictionary<string, HeaderItem> Items { get; } = new Dictionary<string, HeaderItem>();
        public int HeaderSize { get; internal set; }
        public int DataOffset => HeaderSize + 8;

        public static SafeTensor Load(string filePath) => SafeTensorLoader.LoadFromFile(filePath);

        public static SafeTensor<T> Load<T>(string filePath) => SafeTensorLoader.LoadFromFile<T>(filePath);

        internal SafeTensor(string path)
        {
            filePath = path;
        }
        
        public void Read(Action<SafeTensorReadSession> action)
        {
            using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            action(new SafeTensorReadSession(this, mmf));
        }

        public static void Create(string path,Action<SafeTensorWriteSession> action,int headerBufferSize=2048)
        {
            using var session = new SafeTensorWriteSession(path, headerBufferSize);
            action(session);
            session.EndWrite();
        }

    }
}
