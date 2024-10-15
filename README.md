# SafeTensorSharp
### Read and write data in safetensor format.


SafeTensor is designed by Huggingface, provides safe and fast weights loading. 

This library is desgined to try avoid memory copy, implemented with netcore 2.1 to support various platforms. Please see the sample section for more information.



Some test files copied from [safetensors_util](https://github.com/by321/safetensors_util)


**This library is still working in progress, more features are on the way.**




Nuget : 
```
dotnet add package SafeTensorSharp
```


Read data from Safetensor file
```csharp
public void ReadDataSample()
{
    var s = SafeTensor.Load("X:\\mymodel\\basic_model.safetensors");
    Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
    s.Read(reader =>
    {
        foreach (var item in s.Items)
        {
            byte[] mybuffer = new byte[item.Value.DataLength]; //create buffer for item
            reader.ReadToSpan(item.Key, mybuffer);
            result.Add(item.Key, mybuffer);
        }
    });
}
```


Write tensor to Safetensor file
```csharp

public void SaveDataSample()
{
    float[] embedding = [0.1f, 0.2f, 0.3f];
    float[] weights = [0.1f, 0.2f, 0.3f, 0.4f];
    SafeTensor.Create("X:\\mydel\\weights.safetensors", writer =>
    {
        writer.MetaObject = new { Author="John", ModelUsed="MyGreatModel" }; //optional meta data
        writer.Write<float>("embedding", embedding, [4], "F16");
        writer.Write<float>("weights", weights, [2, 2], "F16");
    });
}

```

