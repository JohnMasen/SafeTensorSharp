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
        private string testFilePath = "TestFiles";

        [TestMethod]
        public void CanLoad()
        {
            using var s=SafeTensor.Load(Path.Combine(testFilePath, "basic_model.safetensors"));
            Assert.AreEqual(s.Items.Count, 2);
        }
        [TestMethod]
        public void CanLoadWithMetadata()
        {
            using var s = SafeTensor.Load(Path.Combine(testFilePath, "with_metadata.safetensors"));
            Assert.AreEqual(s.Items.Count ,2);
            var o = JsonSerializer.Deserialize<TestMeta>(s.MetaData);
            TestMeta t = new TestMeta() { key1 = "value1", key2 = "value2" };
            Assert.IsNotNull(o);
            Assert.AreEqual(o, t);
        }

        [TestMethod]
        public void CanLoadWithGenericMetadata()
        {
            using var s = SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "with_metadata.safetensors"));
            Assert.IsNotNull(s.MetaObject);
            TestMeta t = new TestMeta() { key1 = "value1", key2 = "value2" };
            Assert.AreEqual(s.MetaObject, t);
        }

        
        [TestMethod]
        [ExpectedException(typeof(InvalidSafeTensorHeaderException))]
        public void EmptyMetadataWithGenericMetadataType()
        {
            using var s=SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "basic_model.safetensors"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSafeTensorHeaderException))]
        public void DuplicatedItemInHeader()
        {
            using var s=SafeTensor.Load<TestMeta>(Path.Combine(testFilePath, "duplicate_keys_in_header.safetensors"));
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