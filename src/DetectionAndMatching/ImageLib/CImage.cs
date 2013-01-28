using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageLib.Enums;

namespace ImageLib
{
    public class CImage : CImageAttributes
    {
         CShape m_shape;         // image shape (dimensions)
     Type m_pTI; // pointer to type_info class
    int m_bandSize;         // size of each band in bytes
    int m_pixSize;          // stride between pixels in bytes
    int m_rowSize;          // stride between rows in bytes
    //char* m_memStart;       // start of addressable memory
    List<double> m_memory;    // reference counted memory

        public CImage()
        {
            SetDefaults();
        }
        public CImage(CShape s, Type ti, int bandSize)
        {
            SetDefaults();
            ReAllocate(s, ti, bandSize, null, true, 0);
        }
        public void ReAllocate(CShape s, Type ti, int bandSize,object memory, bool deleteWhenDone, int rowSize)
        {
            // Set up the type_id, shape, and size info
            m_shape = s;                        // image shape (dimensions)
            m_pTI = ti;                      // pointer to type_info class
          //  m_bandSize = bandSize;                 // size of each band in bytes
            m_bandSize = 1; //try this modification as stated below
            m_pixSize = m_bandSize * s.nBands;    // stride between pixels in bytes
           // m_pixSize = 1 * s.nBands; //change this since in c# each channel only takes up "One" unit of memory so the pixel is defined by the number of channels

            // Do the real allocation work
            //in c++ bool is 0 so if rowsize was zero it would do the more complicated step
           // m_rowSize = (rowSize>0) ? m_pixSize * rowSize :     // stride between rows in bytes
           //               (m_pixSize * s.width + 7) & -8;     // round up to 8 (quadwords)
            m_rowSize = (rowSize > 0) ? m_pixSize * rowSize :
                                        (s.nBands * s.width);
            int nBytes = m_rowSize * s.height;
            if (memory == null && nBytes > 0)          // allocate if necessary
            {
                memory = new double[(nBytes + 7) / 8];
                m_memory = new List<double>();
                for (int i = 0; i < nBytes; i++)
                {
                    m_memory.Add(0);
                }
                if (memory == null)
                    throw new Exception(string.Format("CImage::Reallocate: could not allocate %d bytes", nBytes));
            }
          //  m_memStart = (char*)memory;           // start of addressable memory
           // m_memory.ReAllocate(nBytes, memory, deleteWhenDone);
            
            
        }
        public void ReAllocate(CShape c, Type ti, int bandSize, bool evenIfShapeDiffers = false)
        {
            if (!evenIfShapeDiffers && c == m_shape && ti == m_pTI && bandSize == m_bandSize)
                return;
            ReAllocate(c, ti, bandSize, 0, true, 0);
        }
        // release the memory & set to default values
        public void DeAllocate()
        {
            // Release the memory & set to default values
            ReAllocate(new CShape(), null, 0, 0, false, 0);
            SetDefaults();
        }

        public CShape Shape()
        {
            return m_shape;
        }

        public Type PixType()
        {
            return m_pTI;
        }

        public int BandSize()
        {
            return m_bandSize;
        }
        //c++ code could set the memory directly.
        //c# code is just setting the value
        //As such we need to change the calculation away from the total memory in bytes. 
        //Need to change it to access the place in the array that represents the color channel of the pixel
        public double GetPixelAddress(int x, int y, int band)
        {
            //return (void*)&m_memStart[y * m_rowSize + x * m_pixSize + band * m_bandSize];
            int elementToGet = y * m_rowSize + x * m_pixSize + band * m_bandSize;
            return m_memory[elementToGet];
        }

        public void SetPixelAddress(int x, int y, int band, double value)
        {
            int elementToSet = y * m_rowSize + x * m_pixSize + band * m_bandSize;
            m_memory[elementToSet] = value;           
        }

        public void SetSubImage(int xO, int yO, int width, int height)   // sub-image sharing memory
        {
            // NOTE:  the subimage is with respect to the rectangle specified
            //  by the origin and current shape
            int x = xO - origin[0];
            int y = yO - origin[1];

            // Adjust the start of memory pointer
           // m_memStart = (char*)PixelAddress(x, y, 0);

            // Compute area of intersection and adjust the shape and origin
            int x1 = Math.Min(m_shape.width, x + width);    // end column
            int y1 = Math.Min(m_shape.height, y + height);   // end row
            x = Math.Max(0, Math.Min(x, m_shape.width));      // clip to original shape
            y = Math.Max(0, Math.Min(y, m_shape.height));     // clip to original shape
            m_shape.width = x1 - x;                    // actual width
            m_shape.height = y1 - y;                    // actual height
            origin[0] += x1;                            // adjust the origin
            origin[1] += y1;                            // adjust the origin
        }

        public void ClearPixels() // set all the pixels to 0
        {
            // Set all the pixels to 0
          //  for (int y = 0; y < m_shape.height; y++)
             //   memset(PixelAddress(0, y, 0), 0, m_pixSize * m_shape.width);
            //basicall for every object returned in PixelAddress and set it to 0 using the ( m_pixSize * m_shape.width)
            //finds the length ... i think. have to come back to this
            //TODO: come back to this.
        }

        private void SetDefaults() // set internal state to default values
        {
            // Set internal state to default values
          //  m_pTI = 0;              // pointer to type_info class
            m_bandSize = 0;         // size of each band in bytes
            m_pixSize = 0;          // stride between pixels in bytes
            m_rowSize = 0;          // stride between rows in bytes
          //  m_memStart = 0;         // start of addressable memory

            // Set default attribute values
            alphaChannel = 3;       // which channel contains alpha (for compositing)
            origin = new int[2];
            origin[0] = 0;          // x and y coordinate origin (for some operations)
            origin[1] = 0;          // x and y coordinate origin (for some operations)
            borderMode = EBorderMode.eBorderReplicate;   // border behavior for neighborhood operations...
        }


    }
    
}
