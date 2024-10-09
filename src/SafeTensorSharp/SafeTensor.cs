using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SafeTensorSharp
{
    public class SafeTensor : IDisposable
    {
        readonly MemoryMappedFile mmf;
        private bool disposedValue;
        private FileInfo fileInfo;
        public JsonElement MetaData { get; private set; }

        public Dictionary<string, HeaderItem> Items { get; private set; }
        public int HeaderSize { get; private set; }

        public static SafeTensor Load(string filePath)
        {

            var f = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, Guid.NewGuid().ToString(), 0, MemoryMappedFileAccess.ReadWrite);//create a viewstream requires readwrite access
            SafeTensor result = new SafeTensor(f);
            result.LoadInternal(new FileInfo(filePath).Length);
            return result;
        }
        public static SafeTensor<T> Load<T>(string filePath)
        {
            var f = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, Guid.NewGuid().ToString(), 0, MemoryMappedFileAccess.ReadWrite);
            SafeTensor<T> result = new SafeTensor<T>(f);
            result.LoadInternal(new FileInfo(filePath).Length);
            return result;
        }
        private void validateItems(long fileSize)
        {
            long start = 0;
            long end = 0;
            long lastPos = 0;
            var validationList = Items.OrderBy(x => x.Value.DataStart).ToList();
            var last = validationList.Last();
            if (last.Value.DataStart+last.Value.DataLength+HeaderSize+8>fileSize)
            {
                throw new InvalidSafeTensorHeaderException("content size is larger than header description");
            }
            foreach (var item in validationList)
            {
                if (item.Value.DataStart!=lastPos)
                {
                    throw new InvalidSafeTensorHeaderException($"data gap found in {item.Key}, expected start={lastPos} ,actual start={item.Value.DataStart}");
                }
                lastPos += item.Value.DataLength;
            }
        }

        

        internal SafeTensor(MemoryMappedFile mf)
        {
            mmf = mf;
        }

        protected virtual void LoadInternal(long fileSize)
        {
            long length = 0;
            using (var headerLengthView=mmf.CreateViewStream(0,8))
            {
                using (var r=new BinaryReader(headerLengthView))
                {
                    length = r.ReadInt64();
                }
            }
            if (length==0)
            {
                throw new InvalidSafeTensorHeaderException("Header size should not be zero");
            }
            else if(length<0)
            {
                throw new NotSupportedException("Header size over 0x7FFF_FFFF_FFFF_FFFF is not supported");
            }
            using var view = mmf.CreateViewStream(8,length);
            using BinaryReader reader = new BinaryReader(view);
            if (length + 8 > fileSize)
            {
                throw new InvalidSafeTensorHeaderException("Invalid header length, header length shoud not larger than file size");
            }
            
            var buffer = reader.ReadBytes((int)length);
            HeaderSize = (int)length;
            string s = Encoding.UTF8.GetString(buffer);
            HeaderStub stub;
            try
            {
                stub = JsonSerializer.Deserialize<HeaderStub>(s);
            }
            catch (Exception ex)
            {
                throw new InvalidSafeTensorHeaderException("failed decode header, invalid json", ex);
            }
            Items = new Dictionary<string, HeaderItem>();
            foreach (var item in stub.Items)
            {
                if (!Items.TryAdd(item.Key, JsonSerializer.Deserialize<HeaderItem>(item.Value)))
                {
                    throw new InvalidSafeTensorHeaderException($"duplicated key found in header,name={item.Key}");
                }
            }
            MetaData = stub.Meta;
            validateItems(fileSize);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    mmf.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SafeTensor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
