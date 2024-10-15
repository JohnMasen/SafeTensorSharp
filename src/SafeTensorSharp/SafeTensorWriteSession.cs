﻿using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SafeTensorSharp
{
    public class SafeTensorWriteSession : IDisposable
    {
        FileStream fs;
        List<(string name,HeaderItem item)> Items;
        int reservedHeaderLength;
        long dataSize = 0;
        string stageFile;
        public object  MetaObject { get; set; }
        public SafeTensorWriteSession(string path, int headBufferSize)
        {
            stageFile = $"{path}.stg";
            reservedHeaderLength = headBufferSize;
            fs = File.Create(stageFile);
            fs.Write(new byte[reservedHeaderLength+8]);
            Items = new List<(string name, HeaderItem item)>();
        }

        public void Dispose()
        {
            fs.Dispose();
            string targetFilePath = stageFile.Substring(0, stageFile.Length - 4);
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }
            File.Move(stageFile, targetFilePath);
        }

        public void Write<T>(string name, Span<T> data, int[] shape, string dataType) where T : struct
        {
            fs.Write(MemoryMarshal.AsBytes(data));
            
            var blockSize = Marshal.SizeOf<T>() * data.Length;
            Items.Add(
                (name,
                new HeaderItem() {DataType = dataType, DataOffsets = new long[] { dataSize, dataSize + blockSize },Shape=shape }));
            dataSize += blockSize;
        }



        public ValueTask WriteAsync<T>(string name, Memory<byte> data, int[] shape,string dataType,CancellationToken token=default)
        {
            var blockSize = data.Length;
            Items.Add(
                (name,
                new HeaderItem() { DataType = dataType, DataOffsets = new long[] { dataSize, dataSize + blockSize }, Shape = shape }));
            dataSize += blockSize;
            return fs.WriteAsync(data);
        }


        internal void EndWrite()
        {
            //prepare header
            HeaderStub stub = new HeaderStub();
            stub.Items = new Dictionary<string, JsonElement>();
            if (MetaObject!=null)
            {
                stub.Meta = JsonSerializer.SerializeToElement(MetaObject);
            }

            foreach (var item in Items)
            {
                stub.Items.Add(item.name, JsonSerializer.SerializeToElement(item.item));
            }
            var s = JsonSerializer.Serialize(stub,
                new JsonSerializerOptions() 
                { 
                    DefaultIgnoreCondition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                });
            var headerBytes = Encoding.UTF8.GetBytes(s);


            if (headerBytes.Length>reservedHeaderLength)
            {
                //TODO:handle header buffer overflow
                fs.Dispose();
                File.Delete(stageFile);
                throw new ArgumentOutOfRangeException("Header buffer overflow, try use a larger buffer");
                

            }
            else
            {
                //write file header
                fs.Seek(0, SeekOrigin.Begin);
                    fs.Write(BitConverter.GetBytes((ulong)reservedHeaderLength));// header length at first 8 bytes

                byte[] contentByteBuffer = new byte[reservedHeaderLength];          //output buffer
                contentByteBuffer.AsSpan().Fill(0x20);                              //fill the buffer with space
                headerBytes.CopyTo(contentByteBuffer, 0);                           //copy content from temp buffer
                fs.Write(contentByteBuffer);                                    // header content with tailing space

            }
        }
    }
}
