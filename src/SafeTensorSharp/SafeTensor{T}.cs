using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Json;

namespace SafeTensorSharp
{
	public class SafeTensor<T> : SafeTensor
	{
		public T MetaObject { get; private set; }
		internal SafeTensor(MemoryMappedFile mf) : base(mf)
		{
		}
		protected override void LoadInternal(long fileSize)
		{
			base.LoadInternal(fileSize);
			try
			{
				MetaObject = JsonSerializer.Deserialize<T>(MetaData);
			}
			catch (Exception ex)
			{

				throw new InvalidSafeTensorHeaderException($"parse metadata to type {typeof(T)} failed");
			}
			
		}
	}
}
