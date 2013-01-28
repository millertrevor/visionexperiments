using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib.Enums
{
    public enum EBorderMode
    {
        eBorderZero = 0,    // zero padding
        eBorderReplicate = 1,    // replicate border values
        eBorderReflect = 2,    // reflect border pixels
        eBorderCyclic = 3     // wrap pixel values
    }
}
