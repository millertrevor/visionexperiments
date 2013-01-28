using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public class CImageOf<T> : CImage
    {
        T _min;
        T _max;
        public CImageOf(T min, T max)
            : base(new CShape(), typeof(T), System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)))
        {
            _min = min;
            _max = max;
        }
        public CImageOf(T min, T max, CShape s)
            : base(s, typeof(T), System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)))
        {
            _min = min;
            _max = max;
        }
        public CImageOf(T min, T max, int width, int height, int nBands)
            : base(new CShape(width, height, nBands), typeof(T), System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)))
        {
            _min = min;
            _max = max;
        }
        public void ReAllocate(CShape s, bool evenIfShapeDiffers = false)
        {
            //uck
            //TODO: not sure this is going to work in C#. porting this from C++ is going to have a lot of changes
            base.ReAllocate(s, typeof(T), System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), evenIfShapeDiffers);
        }
        public void ReAllocate(CShape s, T memory, bool deleteWhenDone, int rowSize)
        {
            //uck
            //TODO: not sure this is going to work in C#. porting this from C++ is going to have a lot of changes
            base.ReAllocate(s, typeof(T), System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), memory, deleteWhenDone, rowSize);
        }
        public double GetPixel(int x, int y, int band)
        {
            // not sure if this casting will work in c#. original code was from c++
            return GetPixelAddress(x, y, band);
        }
        public void SetPixel(int x, int y, int band, double value)
        {
            SetPixelAddress(x, y, band, value);
        }

        public CImageOf<T> SubImage(int x, int y, int width, int height)
        {
            // sub-image sharing memory
            CImageOf<T> retval = this;
            retval.SetSubImage(x, y, width, height);
            return retval;
        }


        public T MinVal()
        {
            return _min;
        }
        public T MaxVal()
        {
            return _max;
        }
    }
    public class CImageOfInt : CImageOf<int>
    {
        public CImageOfInt()
            : base(int.MinValue, int.MaxValue)
        {
        }
        public CImageOfInt(CShape s)
            : base(int.MinValue, int.MaxValue, s)
        {
        }
        public CImageOfInt(int width, int height, int nBands)
            : base(int.MinValue, int.MaxValue, width, height, nBands)
        {
        }
    }
    public class CImageOfFloat : CImageOf<float>
    {
        public CImageOfFloat()
            : base(float.MinValue, float.MaxValue)
        {
        }
        public CImageOfFloat(CShape s)
            : base(float.MinValue, float.MaxValue, s)
        {
        }
        public CImageOfFloat(int width, int height, int nBands)
            : base(float.MinValue, float.MaxValue, width, height, nBands)
        {
        }
    }
    public class CImageOfByte : CImageOf<byte>
    {
        public CImageOfByte()
            : base(byte.MinValue, byte.MaxValue)
        {
        }
        public CImageOfByte(CShape s)
            : base(byte.MinValue, byte.MaxValue, s)
        {
        }
        public CImageOfByte(int width, int height, int nBands)
            : base(byte.MinValue, byte.MaxValue, width, height, nBands)
        {
        }
    }
}
