using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Json;

namespace SafeTensorSharp
{
    /// <summary>
    /// Represents a SafeTensor file info with meta data of type T
    /// </summary>
    /// <typeparam name="T">The type of meta data</typeparam>
    public class SafeTensor<T> : SafeTensor
	{
		/// <summary>
		/// The meta data object
		/// </summary>
		public T MetaObject { get; internal set; }
		internal SafeTensor(string path) : base(path)
		{
		}

	}
}
