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
    /// <summary>
    /// Represents a SafeTensor file info
    /// </summary>
    public class SafeTensor 
    {
        internal readonly string filePath;
        private bool disposedValue;
        public JsonElement MetaData { get; internal set; }

        /// <summary>
        /// Data items in Safetensor
        /// Item name is stored in key, Item details stored in item
        /// </summary>
        public Dictionary<string, HeaderItem> Items { get; } = new Dictionary<string, HeaderItem>();

        /// <summary>
        /// Size of file header
        /// </summary>
        public int HeaderSize { get; internal set; }

        /// <summary>
        /// Bytes offset of data content from begging of file
        /// </summary>
        public int DataOffset => HeaderSize + 8;

        /// <summary>
        /// Load safetensor file
        /// </summary>
        /// <param name="filePath">Safetensor file path</param>
        /// <returns><see cref="SafeTensor"/> object </returns>
        /// <exception cref="InvalidSafeTensorHeaderException"/>
        public static SafeTensor Load(string filePath) => SafeTensorLoader.LoadFromFile(filePath);


        /// <summary>
        /// Load safetensor file with typed meta data
        /// </summary>
        /// <typeparam name="T">The type of meta data</typeparam>
        /// <param name="filePath">Safetensor file path</param>
        /// <returns><see cref="SafeTensor{T}"/> object</returns>
        /// <exception cref="InvalidSafeTensorHeaderException"/>
        public static SafeTensor<T> Load<T>(string filePath) => SafeTensorLoader.LoadFromFile<T>(filePath);

        internal SafeTensor(string path)
        {
            filePath = path;
        }
        

        /// <summary>
        /// Perform a read action on the safetensor file
        /// </summary>
        /// <param name="action">The read action with <see cref="SafeTensorReadSession"/></param>
        public void Read(Action<SafeTensorReadSession> action)
        {
            using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            action(new SafeTensorReadSession(this, mmf));
        }



        /// <summary>
        /// Create a Safetensor file at target path
        /// </summary>
        /// <param name="path">Safetensor file path</param>
        /// <param name="action">The write action with <see cref="SafeTensorWriteSession"/></param>
        /// <param name="headerBufferSize">The default buffer size of file header. currently the Create method will fail if file header exceeds the buffer length</param>
        public static void Create(string path,Action<SafeTensorWriteSession> action,int headerBufferSize=2048)
        {
            using var session = new SafeTensorWriteSession(path, headerBufferSize);
            action(session);
            session.EndWrite();
        }

    }
}
