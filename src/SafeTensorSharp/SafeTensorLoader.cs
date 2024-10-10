using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace SafeTensorSharp
{
    internal class SafeTensorLoader
    {
        internal static SafeTensor LoadFromFile(string path)
        {
            using var f = MemoryMappedFile.CreateFromFile(path, FileMode.Open, Guid.NewGuid().ToString(), 0, MemoryMappedFileAccess.ReadWrite);//create a viewstream requires readwrite access
            SafeTensor result = new SafeTensor(path);
            LoadInternal(result,f,new FileInfo(path).Length);
            return result;
        }

        internal static SafeTensor<T> LoadFromFile<T>(string path)
        {
            using var f = MemoryMappedFile.CreateFromFile(path, FileMode.Open, Guid.NewGuid().ToString(), 0, MemoryMappedFileAccess.ReadWrite);//create a viewstream requires readwrite access
            SafeTensor<T> result = new SafeTensor<T>(path);
            LoadInternal(result, f,new FileInfo(path).Length);
            try
            {
                result.MetaObject = JsonSerializer.Deserialize<T>(result.MetaData);
            }
            catch (Exception ex)
            {

                throw new InvalidSafeTensorHeaderException($"parse metadata to type {typeof(T)} failed");
            }
            return result;
        }




        private static void LoadInternal(SafeTensor st,MemoryMappedFile mmf, long fileSize)
        {
            long length = 0;//header length
            using (var headerLengthView = mmf.CreateViewStream(0, 8))
            {
                using (var r = new BinaryReader(headerLengthView))
                {
                    length = r.ReadInt64();
                }
            }
            if (length == 0)
            {
                throw new InvalidSafeTensorHeaderException("Header size should not be zero");
            }
            else if (length < 0 || length > int.MaxValue)
            {
                throw new NotSupportedException($"Header size over overflow, supported header size is 0 to {int.MaxValue}");
            }
            if (length + 8 > fileSize)
            {
                throw new InvalidSafeTensorHeaderException("Invalid header length, header length shoud not larger than file size");
            }
            st.HeaderSize = (int)length;
            using var view = mmf.CreateViewStream(8, length);
            using BinaryReader reader = new BinaryReader(view);
            var buffer = reader.ReadBytes((int)length);
            string s = Encoding.UTF8.GetString(buffer);

            HeaderStub stub;
            try
            {
                stub = JsonSerializer.Deserialize<HeaderStub>(s);
            }
            catch (Exception ex)
            {
                throw new InvalidSafeTensorHeaderException("failed decode header, invalid json or duplicated item found", ex);
            }
            foreach (var item in stub.Items)
            {
                st.Items.Add(item.Key, JsonSerializer.Deserialize<HeaderItem>(item.Value));
            }
            st.MetaData = stub.Meta;

            validateItems(st,fileSize);
        }
        private static void validateItems(SafeTensor st, long fileSize)
        {
            long start = 0;
            long end = 0;
            long lastPos = 0;

            var validationList = st.Items.OrderBy(x => x.Value.DataStart).ToList();
            var last = validationList.Last();

            if (last.Value.DataStart + last.Value.DataLength + st.HeaderSize + 8 > fileSize)
            {
                throw new InvalidSafeTensorHeaderException("content size is larger than header description");
            }

            foreach (var item in validationList)
            {
                if (item.Value.DataStart > lastPos)
                {
                    throw new InvalidSafeTensorHeaderException($"data gap found in {item.Key}, expected start={lastPos} ,actual start={item.Value.DataStart}");
                }
                else if (item.Value.DataStart < lastPos)
                {
                    throw new InvalidSafeTensorHeaderException($"data overlap found in {item.Key}, expected start={lastPos} ,actual start={item.Value.DataStart}");
                }
                lastPos += item.Value.DataLength;
            }
        }
    }

}
