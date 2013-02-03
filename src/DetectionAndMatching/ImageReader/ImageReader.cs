using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;


namespace ImageReader
{
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

        int[] s = new int[256];
        public int[] l = new int[256];
        int[] swb = new int[256];
        int[] lwb = new int[256];

        int[] cdf = new int[256];



        public ImageReader(string imageFile)
        {
            Bitmap img = new Bitmap(imageFile);
            Width = img.Width;
            Height = img.Height;
            Count = 1;//Something more here!? //TODO: what is this really
            d = 3;
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

                    s[(int)(hsl.Saturation * 255)]++;
                    l[(int)(hsl.Luminance * 255)]++;
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
        public int d
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
