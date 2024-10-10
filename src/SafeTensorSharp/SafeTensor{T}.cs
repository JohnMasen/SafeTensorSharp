using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Json;

namespace SafeTensorSharp
{
	public class SafeTensor<T> : SafeTensor
	{
		public T MetaObject { get; internal set; }
		internal SafeTensor(string path) : base(path)
		{
		}

	}
}
