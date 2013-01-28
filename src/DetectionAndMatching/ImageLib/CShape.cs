using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageLib
{
    public struct CShape
    {
        public int width, height;      // width and height in pixels
        public int nBands;             // number of bands/channels

        // Constructors and helper functions 
        public CShape(int w, int h, int nb)
        {
            width = w;
            height = h;
            nBands = nb;
        }

        public bool InBounds(int x, int y)         // is given pixel address valid?
        {
            // Is given pixel address valid?
            return (0 <= x && x < width &&
                    0 <= y && y < height);
        }

        public bool InBounds(int x, int y, int band)  // is given pixel address valid?
        {
            // Is given pixel address valid?
            return (0 <= x && x < width &&
                    0 <= y && y < height &&
                    0 <= band && band < nBands);
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            try
            {
                return (bool)(this == (CShape)obj);
            }
            catch
            {
                return false;
            }
        }

        public static bool operator ==(CShape objOne, CShape objTwo)
        {
            // Are two shapes the same?
            return (objOne.width == objTwo.width &&
                    objOne.height == objTwo.height &&
                    objOne.nBands == objTwo.nBands);
        }

        public bool SameIgnoringNBands(CShape reference) // " ignoring the number of bands?
        {
            // Are two shapes the same ignoring the number of bands?
            return (width == reference.width &&
                    height == reference.height);
        }

        public static bool operator !=(CShape objOne, CShape objTwo)
        {
            // Are two shapes not the same?
            return !((objOne) == objTwo);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
