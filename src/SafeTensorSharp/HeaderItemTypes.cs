using System;
using System.Collections.Generic;
using System.Text;

namespace SafeTensorSharp
{
    public enum HeaderItemTypes
    {
        BOOL,       // Boolean type  
        U8,         // Unsigned byte  
        I8,         // Signed byte  
        F8_E5M2,    // FP8 https://arxiv.org/pdf/2209.05433.pdf_ (E5M2)  
        F8_E4M3,    // FP8 https://arxiv.org/pdf/2209.05433.pdf_ (E4M3)  
        I16,        // Signed integer (16-bit)  
        U16,        // Unsigned integer (16-bit)  
        F16,        // Half-precision floating point  
        BF16,       // Brain floating point  
        I32,        // Signed integer (32-bit)  
        U32,        // Unsigned integer (32-bit)  
        F32,        // Floating point (32-bit)  
        F64,        // Floating point (64-bit)  
        I64,        // Signed integer (64-bit)  
        U64         // Unsigned integer (64-bit) 
    }
}
