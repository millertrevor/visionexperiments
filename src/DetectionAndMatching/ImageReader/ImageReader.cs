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
        //http://en.wikipedia.org/wiki/HSL_and_HSV

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
                    var tryOne =  0.299*pixel.R + 0.587*pixel.G + 0.114*pixel.B;
                   // //Y = 0.2126 R + 0.7152 G + 0.0722 B
                    var tryTwo = (0.257 * pixel.R) + (0.504 * pixel.G) + (0.098 * pixel.B) + 16;
                    var lum = (0.299 * pixel.R) + (0.587 * pixel.G) + (0.114 * pixel.B);
                    var lumConverted = Convert.ToInt32(lum);
                    if (lum != lumConverted)
                    {
                        int breakage = -1;
                    }

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
                    int stop = -1;
                    // create histograms
                    //saturation = new ContinuousHistogram(s, new DoubleRange(0, 1));
                    //luminance = new ContinuousHistogram(l, new DoubleRange(0, 1));

                    //saturationWithoutBlack = new ContinuousHistogram(swb, new DoubleRange(0, 1));
                    //luminanceWithoutBlack = new ContinuousHistogram(lwb, new DoubleRange(0, 1));

                   

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

                    // myLum.Add(lum);
                }
            }
            int stopper = -1;
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
        //private void OnButtonClick(object sender, RoutedEventArgs e)
        //{
        //    this.Cursor = Cursors.Wait;
        //    try
        //    {
        //        if (String.IsNullOrWhiteSpace(this.ImageURL))
        //        {
        //            MessageBox.Show("Image URL is mandatory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }

        //        String localFilePath = null;
        //        try
        //        {
        //            localFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        //            using (WebClient client = new WebClient())
        //            {
        //                client.DownloadFile(this.ImageURL, localFilePath);
        //            }
        //            this.LocalImagePath = localFilePath;
        //        }
        //        catch (Exception)
        //        {
        //            MessageBox.Show("Invalid image URL. Image could not be retrieved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            return;
        //        }
        //        //green represents 59% of the perceived luminosity, while the red and blue channels account for just 30% and 11%,
        //        // 0.3 R + 0.59 G + 0.11 B
        //        List<int> myLum = new List<int>();
        //        using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(localFilePath))
        //        {
        //            for (int h = 0; h < bmp.Height; h++)
        //            {
        //                for (int j = 0; j < bmp.Width; j++)
        //                {
        //                    var pixel = bmp.GetPixel(j, h);
        //                    //0.299 red + 0.587 green + 0.114 blue.
        //                    //Y = 0.2126 R + 0.7152 G + 0.0722 B
        //                    var lum = (0.299 * pixel.R) + (0.587 * pixel.G) + (0.114 * pixel.B);
        //                    var lumConverted = Convert.ToInt32(lum);
        //                    if (lum != lumConverted)
        //                    {
        //                        int breakage = -1;
        //                    }
        //                    // myLum.Add(lum);
        //                }
        //            }
        //            // Luminance
        //            ImageStatisticsHSL hslStatistics = new ImageStatisticsHSL(bmp);
        //            this.LuminanceHistogramPoints = ConvertToPointCollection(hslStatistics.Luminance.Values);
        //            // RGB
        //            ImageStatistics rgbStatistics = new ImageStatistics(bmp);
        //            this.RedColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Red.Values);
        //            this.GreenColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Green.Values);
        //            this.BlueColorHistogramPoints = ConvertToPointCollection(rgbStatistics.Blue.Values);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error generating histogram: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    finally
        //    {
        //        this.Cursor = Cursors.Arrow;
        //    }
        //}


        //public void SampleCode()
        //{
        //     // Create a new bitmap.
        //    Bitmap bmp = new Bitmap(@"C:\Users\Trevor\Documents\GitHub\visionexperiments\src\DetectionAndMatching\DetectionAndMatching\bin\Debug\test.jpg");

        //    // Lock the bitmap's bits.  
        //    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        //    System.Drawing.Imaging.BitmapData bmpData =
        //        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //        bmp.PixelFormat);

        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap. 
        //    int bytes  = Math.Abs(bmpData.Stride) * bmp.Height;
        //    byte[] rgbValues = new byte[bytes];

        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        //    // Set every third value to 255. A 24bpp bitmap will look red.   
        //    for (int counter = 2; counter < rgbValues.Length; counter += 3)
        //        rgbValues[counter] = 255;

        //    // Copy the RGB values back to the bitmap
        //    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

        //    // Unlock the bits.
        //    bmp.UnlockBits(bmpData);

        //    // Draw the modified image.
        //    //e.Graphics.DrawImage(bmp, 0, 150);
        //    bmp.Save("DistoredImage.bmp", ImageFormat.Bmp);
        //}

    }
}
