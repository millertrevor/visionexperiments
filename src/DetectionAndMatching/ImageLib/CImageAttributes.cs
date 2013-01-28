using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageLib.Enums;

namespace ImageLib
{
    //Was a struct in orignal C++ code. However since structs behave much more like classes in C++ there 
    //was use of this class in an inheritance chain. Making this a class in the C# code
    public class CImageAttributes
    {
        public int alphaChannel;       // which channel contains alpha (for compositing)
        public int[] origin;          // x and y coordinate origin (for some operations)
        public EBorderMode borderMode; // border behavior for neighborhood operations...
        // public char[] colorSpace;    //size of 4 // RGBA, YUVA, etc.: not currently used
    }
}
