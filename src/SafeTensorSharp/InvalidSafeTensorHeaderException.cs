using System;
using System.Collections.Generic;
using System.Text;

namespace SafeTensorSharp
{
    public class InvalidSafeTensorHeaderException:ApplicationException
    {
        public InvalidSafeTensorHeaderException(string message):base(message)
        {
            
        }
        public InvalidSafeTensorHeaderException(string message,Exception innerException):base(message,innerException)
        {
            
        }
    }
}
