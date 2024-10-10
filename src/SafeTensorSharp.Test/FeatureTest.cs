using System.Buffers;
using System.Collections;
using System.Text.Json;

namespace SafeTensorSharp.Test
{
    [TestClass]
    public class FeatureTest
    {

        //SafeTensor st;

        //[TestMethod]
        //public void LoadLarge()
        //{
        //    var s = SafeTensor.Load(@"C:\Tools\RWKVRunner\models\RWKV-x060-World-1B6-v2.1-20240328-ctx4096.st");
        //}

        //[TestMethod]
        //public void LoadLargeData()
        //{
        //    var s = SafeTensor.Load(@"C:\Tools\RWKVRunner\models\RWKV-x060-World-1B6-v2.1-20240328-ctx4096.st");
        //    s.ReadToMemory(reader =>
        //    {
        //        foreach (var item in s.Items)
        //        {
        //            var tmp = MemoryPool<byte>.Shared.Rent((int)item.Value.DataLength);
        //            reader.ReadToSpan(item.Key, tmp.Memory.Span);
        //        }
        //    });
        //}
        private string testFilePath = "TestFiles";

        [TestMethod]
        public void CanLoad()
        {
            var s = SafeTensor.Load(Path.Combine(testFilePath, "basic_model.safetensors"));
            Assert.AreEqual(s.Items.Count, 2);
        }
        [TestMethod]
        public void CanLoadWithMetadata()
        {
            var s = SafeTensor.Load(Path.Combine(testFilePath, "with_metadata.safetensors"));
            Assert.AreEqual(s.Items.Count, 2);
            var o = JsonSerializer.Deserialize<TestMeta>(s.MetaData);
            TestMeta t = new TestMeta() { key1 = "value1", key2 = "value2" };
            Assert.IsNotNull(o);
            Assert.AreEqual(o, t);
        }

        [TestMethod]
        public void CanLoadWithGenericMetadata()
        {
            var s = SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "with_metadata.safetensors"));
            Assert.IsNotNull(s.MetaObject);
            TestMeta t = new TestMeta() { key1 = "value1", key2 = "value2" };
            Assert.AreEqual(s.MetaObject, t);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidSafeTensorHeaderException))]
        public void EmptyMetadataWithGenericMetadataType()
        {
            var s = SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "basic_model.safetensors"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSafeTensorHeaderException))]
        public void DuplicatedItemInHeader()
        {
            var s = SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "duplicate_keys_in_header.safetensors"));
        }

        [TestMethod]
        public void CanLoadData()
        {
            var s = SafeTensor.Load(Path.Combine(testFilePath, "basic_model.safetensors"));
            s.ReadToMemory(reader =>
            {
                foreach (var item in s.Items)
                {
                    byte[] tmp = new byte[item.Value.DataLength];
                    reader.ReadToSpan(item.Key, tmp);
                    switch (item.Key)
                    {
                        case "embedding":
                            Assert.IsTrue(Enumerable.SequenceEqual(tmp, new byte[16]));
                            break;
                        case "attention":
                            Assert.IsTrue(Enumerable.SequenceEqual(tmp, new byte[] { 1, 2, 3, 4, 5, 6 }));
                            break;
                        default:
                            break;
                    }


                }
            });
        }



        public class TestMeta
        {
            public string key1 { get; set; }
            public string key2 { get; set; }
            public override bool Equals(object? obj)
            {
                TestMeta o = obj as TestMeta;
                if (o == null)
                {
                    return false;
                }
                return o.key1 == key1 && o.key2 == key2;
            }
        }
    }
}