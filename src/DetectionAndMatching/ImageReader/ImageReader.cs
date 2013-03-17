using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;


namespace ImageReader
{
    using System.Diagnostics.CodeAnalysis;

    public class ImageReader
    {
        int pixels = 0;
        int pixelsWithoutBlack = 0;

        int pixelsRGB = 0;
        int pixelsWithoutBlackRGB = 0;

        public int[] r = new int[256];
        public int[] g = new int[256];
        public int[] b = new int[256];

        int[] rwb = new int[256];
        int[] gwb = new int[256];
        int[] bwb = new int[256];

        int[] saturation = new int[256];
        public int[] luminance = new int[256];
        int[] swb = new int[256];
        int[] lwb = new int[256];

        int[] cdf = new int[256];



        public ImageReader(string imageFile)
        {
            Bitmap img = new Bitmap(imageFile);
            Width = img.Width;
            Height = img.Height;
            Count = 1;//Something more here!? //TODO: what is this really
            this.depth = 3;
            Pixels = new List<byte>();

         
            RGB rgb = new RGB();
            HSL hsl = new HSL();
           

            for (int j = 0; j < img.Height; j++)           
            {
                for (int i = 0; i < img.Width; i++)
                {
                    Color pixel = img.GetPixel(i, j);
                    //will expect the data to be packed perpixel by row in pixel order RGB
                    Pixels.Add(pixel.R);
                    Pixels.Add(pixel.G);
                    Pixels.Add(pixel.B);
                    
                    rgb.Red = pixel.R;
                    rgb.Green = pixel.G;
                    rgb.Blue = pixel.B;
                    
                    // convert to HSL color space
                    HSL.FromRGB(rgb, hsl);

                    this.saturation[(int)(hsl.Saturation * 255)]++;
                    this.luminance[(int)(hsl.Luminance * 255)]++;
                    pixels++;

                    if (hsl.Luminance != 0.0)
                    {
                        swb[(int)(hsl.Saturation * 255)]++;
                        lwb[(int)(hsl.Luminance * 255)]++;
                        pixelsWithoutBlack++;
                    }
                    
                    byte rValue, gValue, bValue;

                    // get pixel values
                    rValue = pixel.R;
                    gValue = pixel.G;
                    bValue = pixel.B;

                    r[rValue]++;
                    g[gValue]++;
                    b[bValue]++;
                    pixelsRGB++;

                    if ((rValue != 0) || (gValue != 0) || (bValue != 0))
                    {
                        rwb[rValue]++;
                        gwb[gValue]++;
                        bwb[bValue]++;
                        pixelsWithoutBlackRGB++;
                    }
                }
            }
        }
        public List<byte> Pixels
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public int Count
        {
            get;
            set;
        }
        public int depth
        {
            get;
            set;
        }
        public ImageReader()
        {
        }
        private int[] SmoothHistogram(int[] originalValues)
        {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++)
            {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++)
                {
                    smoothedValue += originalValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues;
        }

        public byte GetPixel(int x, int y, int band)
        {
            // bandsize is one
            int bandSize = 1;
            // mpix size = m_bandSize * s.nBands
            int pixSize = bandSize * this.depth;
            // bands is 3 for RGB
            // row size is bands * width
            int rowSize = this.depth * this.Width;

            int elementToGet = y * rowSize + x * pixSize + band * bandSize;
            return Pixels[elementToGet];
        }
        public void SetPixel(int x, int y, int band, byte value)
        {
            // bandsize is one
            int bandSize = 1;
            // mpix size = m_bandSize * s.nBands
            int pixSize = bandSize * this.depth;
            // bands is 3 for RGB
            // row size is bands * width
            int rowSize = this.depth * this.Width;
            int elementToSet = y * rowSize + x * pixSize + band * bandSize;
            Pixels[elementToSet] = value;
        }
        
        public void ConvertToAverageGrey()
        {
            for (var i = 0; i < this.Pixels.Count; i += 3)
            {
                var red = (int)this.Pixels[i];
                var green = (int)this.Pixels[i + 1];
                var blue = (int)this.Pixels[i + 2];
                var grey = (byte)((red + green + blue) / 3);
                this.Pixels[i] = grey;
                this.Pixels[i + 1] = grey;
                this.Pixels[i + 2] = grey;
            }
        }
        public void ConvertToGrey()
        {
            ConvertToGrey(.2125, .7154, .0721);
        }
        public void ConvertToGrey(double rFactor, double gFactor, double bFactor)
        {
            for (var i = 0; i < this.Pixels.Count; i += 3)
            {
                var red = (int)this.Pixels[i];
                var green = (int)this.Pixels[i + 1];
                var blue = (int)this.Pixels[i + 2];
                //float Y = 0.212671f * p.R + 0.715160f * p.G + 0.072169f * p.B;
                // (r*.2125)+(g*.7154)+(b*.0721)
                var grey = (byte)(Math.Floor((red*rFactor) + (green*gFactor) + (blue*bFactor)) );
                this.Pixels[i] = grey;
                this.Pixels[i + 1] = grey;
                this.Pixels[i + 2] = grey;
            }
        }

        public void SaveAsBitmap(string name)
        {
            int pixelTracker = 0;
            var bm = new Bitmap(Width, Height);
            for (int j = 0; j < bm.Height; j++)
            {
                for (int i = 0; i < bm.Width; i++)
                {
                    bm.SetPixel(i,j,Color.FromArgb(Pixels[pixelTracker++],Pixels[pixelTracker++],Pixels[pixelTracker++]));
                }
            }
            bm.Save(name);
        }
    }
}
