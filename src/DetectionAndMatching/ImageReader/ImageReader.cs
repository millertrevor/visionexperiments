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

        public ImageReader(string imageFile)
        {
            Bitmap img = new Bitmap(imageFile);
            Width = img.Width;
            Height = img.Height;
            Count = 1;//Something more here!? //TODO: what is this really
            d = 3;
            Pixels = new List<byte>();
            for (int j = 0; j < img.Height; j++)           
            {
                for (int i = 0; i < img.Width; i++)
                {
                    Color pixel = img.GetPixel(i, j);
                    //will expect the data to be packed perpixel by row in pixel order RGB
                    Pixels.Add(pixel.R);
                    Pixels.Add(pixel.G);
                    Pixels.Add(pixel.B);
                  
                    // if (pixel == pixel)
                    // {
                    // **Store pixel here in a array or list or whatever** 
                    //}
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
