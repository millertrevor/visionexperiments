using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ImageReader;
using ImageLib;
using DetectionAndMatching.UI.Models;

using System.Drawing;

namespace DetectionAndMatching
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //e.Args is the string[] of command line argruments
            //  var reader = new ImageReader.ImageReader("test.jpg");

            //   reader.SampleCode();
            if (e.Args.Length > 0)
            {
                ProcessCommandLineParameters(e.Args);
            }
            else
            {
                this.StartupUri = new System.Uri(@"UI\Views\MainWindow.xaml", System.UriKind.Relative);
            }
          
        }


        //private void TestAddition()
        //{
        //    var imageOne = new ImageReader.ImageReader("son1.gif");
        //    var imageTwo = new ImageReader.ImageReader("son2.gif");
        //    List<byte> resultPixels = new List<byte>();
        //    for (int i = 0; i < imageOne.Pixels.Count; i++)
        //    {
        //        resultPixels.Add((byte)
        //            (
        //           // Convert.ToByte(
        //              (byte)imageOne.Pixels[i]+100
        //            -
        //            (byte)imageTwo.Pixels[i]
        //          //  )
                  
        //            )
        //            );
        //    }

        //    Bitmap img = new Bitmap(imageOne.Width, imageOne.Height);
        //   // img.Width = imageOne.Width;
        //   // img.Height = imageOne.Height;
        //   // Count = 1;//Something more here!? //TODO: what is this really
        //   // depth = 3;
        //  //  Pixels = new List<byte>();
        //    int pixelCount=0;
        //    for (int j = 0; j < img.Height; j++)
        //    {
        //        for (int i = 0; i < img.Width; i++)
        //        {
        //          //  Color pixel = img.GetPixel(i, j);
        //           var color =  Color.FromArgb(resultPixels[pixelCount++], resultPixels[pixelCount++], resultPixels[pixelCount++]);

        //           img.SetPixel(i, j, color);
                    
        //        }
        //    }

        //    img.Save("TESTOUTPUTAddition.bmp");
        //}

        private void ProcessCommandLineParameters(string[] argv)
        {
            int argc = argv.Length;
            if ((argv[0].Equals("computeFeatures")) == true)
            {
                mainComputeFeatures(argc, argv);
            }
            else if ((argv[0].Equals("matchFeatures")) == true)
            {
                // return mainMatchFeatures(argc, argv);
            }
            else if ((argv[0].Equals("matchSIFTFeatures")) == true)
            {
                //  return mainMatchSIFTFeatures(argc, argv);
            }
            else if ((argv[0].Equals("testMatch")) == true)
            {
                // return mainTestMatch(argc, argv);
            }
            else if ((argv[0].Equals("testSIFTMatch")) == true)
            {
                //return mainTestSIFTMatch(argc, argv);
            }
            else if ((argv[0].Equals("benchmark")) == true)
            {
                // return mainBenchmark(argc, argv);
            }
            else if ((argv[0].Equals("rocSIFT")) == true)
            {
                //return saveRoc(argc,argv);
                // mainRocTestSIFTMatch(argc, argv);
            }
            else if ((argv[0].Equals("roc")) == true)
            {
                //return saveRoc(argc,argv);
                // mainRocTestMatch(argc, argv);
            }

            else
            {
                var fullName = Environment.GetCommandLineArgs()[0];
                System.IO.FileInfo fi = new System.IO.FileInfo(fullName);
                var shortName = fi.Name;
                Console.WriteLine("usage:\n");
                Console.WriteLine("\t{0}\n", shortName);
                Console.WriteLine("\t{0} computeFeatures imagefile featurefile [featuretype]\n", shortName);
                Console.WriteLine("\t{0} matchFeatures featurefile1 featurefile2 matchfile [matchtype]\n", shortName);
                Console.WriteLine("\t{0} matchSIFTFeatures featurefile1 featurefile2 matchfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} testMatch featurefile1 featurefile2 homographyfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} testSIFTMatch featurefile1 featurefile2 homographyfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} benchmark imagedir [featuretype matchtype]\n", shortName);
                Console.WriteLine("\t{0} rocSIFT featurefile1 featurefile2 homographyfile [matchtype] rocfilename aucfilename\n", shortName);
                Console.WriteLine("\t{0} roc featurefile1 featurefile2 homographyfile [matchtype] rocfilename aucfilename\n", shortName);

                Application.Current.Shutdown(-1);
            }
        }


        CImageOfFloat LoadImageFile(string filename)//this won't work in C# land, CImageOfFloat image)
        {
            CImageOfFloat image = null;
            // Load the query image.
            //Fl_Shared_Image *fl_image = Fl_Shared_Image::get(filename);
            var fl_image = new ImageReader.ImageReader(filename);

            if (fl_image == null)
            {
                //// printf("couldn't load query image\n");
                //CImageOfByte byteImage=null;
                //ReadFile(byteImage, filename);

                //CShape sh = byteImage.Shape();
                //sh.nBands = 3;

                //image = new CImageOfFloat(sh);
                //convertToFloatImage(byteImage, image);

                //return true;
                return null;//Until we figure out hte fall back with FileIO
            }
            else
            {
                CShape sh = new CShape(fl_image.Width, fl_image.Height, 3);
                image = new CImageOfFloat(sh);

                // Convert the image to the CImage format.
                if (!convertImage(fl_image, image))
                {
                    Console.WriteLine("Couldn't convert image to RGB format\n");
                    return null;
                }

                return image;
            }
        }

        // Convert Fl_Image to CFloatImage.
        bool convertImage(ImageReader.ImageReader image, CImageOfFloat convertedImage)
        {
            if (image == null)
            {
                return false;
            }

            // Let's not handle indexed color images.
            if (image.Count != 1)
            {
                return false;
            }

            int w = image.Width;
            int h = image.Height;
            int d = image.depth;

            // Get the image data.
            var data = image.Pixels;

            int index = 0;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (d < 3)
                    {
                        // If there are fewer than 3 channels, just use the
                        // first one for all colors.
                        convertedImage.SetPixel(x, y, 0, (data[index]) / 255.0f);
                        convertedImage.SetPixel(x, y, 1, (data[index]) / 255.0f);
                        convertedImage.SetPixel(x, y, 2, (data[index]) / 255.0f);
                        //convertedImage.Pixel(x, y, 0) = (data[index]) / 255.0f;
                        //convertedImage.Pixel(x, y, 1) = (data[index]) / 255.0f;
                        //convertedImage.Pixel(x, y, 2) = (data[index]) / 255.0f;
                    }
                    else
                    {
                        // Otherwise, use the first 3.
                        convertedImage.SetPixel(x, y, 0, (data[index]) / 255.0f);
                        convertedImage.SetPixel(x, y, 1, (data[index + 1]) / 255.0f);
                        convertedImage.SetPixel(x, y, 2, (data[index + 2]) / 255.0f);
                        //convertedImage.Pixel(x, y, 0) = (data[index]) / 255.0f;
                        //convertedImage.Pixel(x, y, 1) = (data[index + 1]) / 255.0f;
                        //convertedImage.Pixel(x, y, 2) = (data[index + 2]) / 255.0f;
                    }

                    index += d;
                }
            }

            return true;
        }
        // Convert CFloatImage to CByteImage.
        void convertToByteImage(CImageOfFloat floatImage, CImageOfByte byteImage)
        {
            CShape sh = floatImage.Shape();

            System.Diagnostics.Debug.Assert(floatImage.Shape().nBands == byteImage.Shape().nBands);
            for (int y = 0; y < sh.height; y++)
            {
                for (int x = 0; x < sh.width; x++)
                {
                    for (int c = 0; c < sh.nBands; c++)
                    {

                        double value = Math.Floor(255 * floatImage.GetPixel(x, y, c) + 0.5);

                        if (value < byteImage.MinVal())
                        {
                            value = byteImage.MinVal();
                        }
                        else if (value > byteImage.MaxVal())
                        {
                            value = byteImage.MaxVal();
                        }

                        // We have to flip the image and reverse the color
                        // channels to get it to come out right.  How silly!
                        byteImage.SetPixel(x, sh.height - y - 1, sh.nBands - c - 1, value);
                    }
                }
            }
        }




        void convertToFloatImage(CImageOfByte byteImage, CImageOfFloat floatImage)
        {
            CShape sh = byteImage.Shape();

            System.Diagnostics.Debug.Assert(floatImage.Shape().nBands == Math.Min(byteImage.Shape().nBands, 3));
            for (int y = 0; y < sh.height; y++)
            {
                for (int x = 0; x < sh.width; x++)
                {
                    for (int c = 0; c < Math.Min(3, sh.nBands); c++)
                    {
                        float value = (float)byteImage.GetPixel(x, y, c) / 255.0f;

                        if (value < floatImage.MinVal())
                        {
                            value = floatImage.MinVal();
                        }
                        else if (value > floatImage.MaxVal())
                        {
                            value = floatImage.MaxVal();
                        }

                        // We have to flip the image and reverse the color
                        // channels to get it to come out right.  How silly!
                        floatImage.SetPixel(x, sh.height - y - 1, Math.Min(3, sh.nBands) - c - 1, value);
                        //item = value;
                        //TODO: i think value needs to be set at the Pixel location in the float Image
                        //I dont' think it is done currently
                    }
                }
            }
        }


        // Compute the features for a single image.
        int mainComputeFeatures(int argc, string[] argv)
        {
            if ((argc < 3) || (argc > 4))
            {
                var fullName = Environment.GetCommandLineArgs()[0];
                System.IO.FileInfo fi = new System.IO.FileInfo(fullName);
                var shortName = fi.Name;
                Console.WriteLine("usage: {0} computeFeatures imagefile featurefile [featuretype]\n", argv[0]);

                return -1;
            }

            // Use feature type 1 as default.
            int type = 1;

            if (argc > 3)
            {
                type = int.Parse(argv[3]);
            }

            CImageOfFloat floatQueryImage = null;
            //bool success = false;
            floatQueryImage = LoadImageFile(argv[1]);//, floatQueryImage);

            if (floatQueryImage == null)
            {
                Console.WriteLine("Couldn't load query image\n");
                return -1;
            }

            // Compute the image features.
            FeatureSet features = null;
            computeFeatures(floatQueryImage, features, type);

            // Save the image features.
            //features.save(argv[2]);

            return 0;
        }
        //Should go in another file
        // Compute features of an image.
        public bool computeFeatures(CImageOfFloat image, FeatureSet features, int featureType)
        {
            // TODO: Instead of calling dummyComputeFeatures, write your own
            // feature computation routines and call them here.
            switch (featureType)
            {
                case 1:
                    dummyComputeFeatures(image, features);
                    break;
                case 2:
                    ComputeHarrisFeatures(image, features);
                    break;
                default:
                    return false;
            }

            // This is just to make sure the IDs are assigned in order, because
            // the ID gets used to index into the feature array.
            for (int i = 0; i < features.Count; i++)
            {
                features[i].Id = i + 1;
            }

            return true;
        }




        // Compute silly example features.  This doesn't do anything
        // meaningful.
        void dummyComputeFeatures(CImageOfFloat image, FeatureSet features)
        {
            CShape sh = image.Shape();
            Feature f = new Feature();

            for (int y = 0; y < sh.height; y++)
            {
                for (int x = 0; x < sh.width; x++)
                {
                    double r = image.GetPixel(x, y, 0);
                    double g = image.GetPixel(x, y, 1);
                    double b = image.GetPixel(x, y, 2);

                    if ((int)(255 * (r + g + b) + 0.5) % 100 == 1)
                    {
                        // If the pixel satisfies this meaningless criterion,
                        // make it a feature.

                        f.type = 1;
                        f.Id += 1;
                        f.X = x;
                        f.Y = y;

                        // f.Data.resize(1);
                        f.Data.Add(r + g + b);

                        // features.push_back(f);
                        features.Add(f);
                    }
                }
            }
        }

        void ComputeHarrisFeatures(CImageOfFloat image, FeatureSet features)
        {
            //Create grayscale image used for Harris detection
            var grayImage = (CImageOfFloat)ConvertToGray(image);

            //Create image to store Harris values
            CImageOfFloat harrisImage = new CImageOfFloat(image.Shape().width, image.Shape().height, 1);

            //Create image to store local maximum harris values as 1, other pixels 0
            CImageOfByte harrisMaxImage = new CImageOfByte(image.Shape().width, image.Shape().height, 1);


            //compute Harris values puts harris values at each pixel position in harrisImage. 
            //You'll need to implement this function.
            computeHarrisValues(grayImage, harrisImage);

            // Threshold the harris image and compute local maxima.  You'll need to implement this function.
            computeLocalMaxima(harrisImage, harrisMaxImage);


            // Prints out the harris image for debugging purposes
            CImageOfByte tmp = new CImageOfByte(harrisImage.Shape());
            convertToByteImage(harrisImage, tmp);
            WriteFile(tmp, "harris.tga");


            // TO DO--------------------------------------------------------------------
            //Loop through feature points in harrisMaxImage and create feature descriptor 
            //for each point above a threshold

            for (int y = 0; y < harrisMaxImage.Shape().height; y++)
            {
                for (int x = 0; x < harrisMaxImage.Shape().width; x++)
                {

                    // Skip over non-maxima
                    if (harrisMaxImage.GetPixel(x, y, 0) == 0)
                        continue;



                    //TO DO---------------------------------------------------------------------
                    // Fill in feature with descriptor data here. 
                    Feature f = new Feature();

                    // Add the feature to the list of features
                    //features.push_back(f);
                    features.Add(f);
                }
            }
        }



        //TO DO---------------------------------------------------------------------
        //Loop through the image to compute the harris corner values as described in class
        // srcImage:  grayscale of original image
        // harrisImage:  populate the harris values per pixel in this image
        void computeHarrisValues(CImageOfFloat srcImage, CImageOfFloat harrisImage)
        {

            int w = srcImage.Shape().width;
            int h = srcImage.Shape().height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {

                    // TODO:  Compute the harris score for 'srcImage' at this pixel and store in 'harrisImage'.  See the project
                    //   page for pointers on how to do this

                }
            }
        }



        // TO DO---------------------------------------------------------------------
        // Loop through the harrisImage to threshold and compute the local maxima in a neighborhood
        // srcImage:  image with Harris values
        // destImage: Assign 1 to a pixel if it is above a threshold and is the local maximum in 3x3 window, 0 otherwise.
        //    You'll need to find a good threshold to use.
        void computeLocalMaxima(CImageOfFloat srcImage, CImageOfByte destImage)
        {

        }
        // Compute SSD distance between two vectors.
        double distanceSSD(List<double> v1, List<double> v2)
        {
            int m = v1.Count;
            int n = v2.Count;

            if (m != n)
            {
                // Here's a big number.
                return 1e100;
            }

            double dist = 0;


            for (int i = 0; i < m; i++)
            {
                dist += Math.Pow(v1[i] - v2[i], 2);
            }


            return Math.Sqrt(dist);
        }


        // Perform simple feature matching.  This just uses the SSD
        // distance between two feature vectors, and matches a feature in the
        // first image with the closest feature in the second image.  It can
        // match multiple features in the first image to the same feature in
        // the second image.
        void ssdMatchFeatures(FeatureSet f1, FeatureSet f2, List<FeatureMatch> matches, double totalScore)
        {
            int m = f1.Count;
            int n = f2.Count;

            matches = new List<FeatureMatch>();
            totalScore = 0;

            double d;
            double dBest;
            int idBest;

            for (int i = 0; i < m; i++)
            {
                dBest = 1e100;
                idBest = 0;

                for (int j = 0; j < n; j++)
                {
                    d = distanceSSD(f1[i].Data, f2[j].Data);

                    if (d < dBest)
                    {
                        dBest = d;
                        idBest = f2[j].Id;
                    }
                }

                FeatureMatch match = new FeatureMatch();
                match.id1 = f1[i].Id;
                match.id2 = idBest;
                match.score = dBest;
                matches.Add(match);
                totalScore += matches[i].score;
            }
        }

        // TODO: Write this function to perform ratio feature matching.  
        // This just uses the ratio of the SSD distance of the two best matches as the score
        // and matches a feature in the first image with the closest feature in the second image.
        // It can match multiple features in the first image to the same feature in
        // the second image.  (See class notes for more information, and the sshMatchFeatures function above as a reference)
        void ratioMatchFeatures(FeatureSet f1, FeatureSet f2, List<FeatureMatch> matches, double totalScore)
        {


        }



        //template <class T>//CImageOf<T>
        CImageOf<T> ConvertToGray<T>(CImageOf<T> src)
        {
            // Check if already gray
            CShape sShape = src.Shape();
            if (sShape.nBands == 1)
                return src;

            //#if 0
            //    // Make sure the source is a color image
            //    if (sShape.nBands != 4 || src.alphaChannel != 3)
            //        throw CError("ConvertToGray: can only convert from 4-band (RGBA) image");
            //#else
            // Make sure the source is a color image
            if (sShape.nBands != 3)
                throw new Exception("ConvertToGray: can only convert from 3-band (RGB) image");
            //#endif

            // Allocate the new image
            CShape dShape = new CShape(sShape.width, sShape.height, 1);
            CImageOf<T> dst = new CImageOf<T>(src.MinVal(), src.MaxVal(), dShape);

            // Process each row
            var minVal = dst.MinVal();
            var maxVal = dst.MaxVal();
            for (int y = 0; y < sShape.height; y++)
            {
                var srcP = src.GetPixel(0, y, 0);
                var dstP = dst.GetPixel(0, y, 0);
                for (int x = 0; x < sShape.width; x++, srcP += 3/*4*/, dstP++)
                {
                    //Ok more c++ template magic.
                    //Somehow we can grab the whole pixel RGBA and all.
                    //THen used the templated class/struct THING to access each pixel individually to do the transformation
                    //Wow this sucks. How am i going to do this in c#
                    //Guess i can git rid of that stupid RGBA class because that ain't gonna work!
                    //RGBA<T> p = *(RGBA<T>*)srcP;
                    //float Y = 0.212671f * p.R + 0.715160f * p.G + 0.072169f * p.B;
                    //dstP = Math.Min(maxVal, Math.Max(minVal, Y));
                    ////RGBA < T > &p = *(RGBA<T>*)srcP;
                    ////float Y = 0.212671f * p.R + 0.715160f * p.G + 0.072169f * p.B;
                    ////*dstP = (T)__min(maxVal, __max(minVal, Y));
                    //TODO: WOW FIGURE THIS MESS OUT
                }
            }
            throw new NotImplementedException("Wow look up above in this method, how are we going to convert this mess");
            return dst;
        }


        void WriteFile(CImage img, string filename)
        {
            // Determine the file extension
            //const char *dot = strrchr(filename, '.');
            string dot = new System.IO.FileInfo(filename).Extension;
            if (string.Equals(dot, ".tga") || string.Equals(dot, ".tga"))
            {
                if (img.PixType() == typeof(byte))
                    WriteFileTGA(img, filename);
                else
                    throw new Exception(string.Format("ReadFile({0}): haven't implemented conversions yet", filename));
            }
            else
                throw new Exception(string.Format("WriteFile({0}): file type not supported", filename));
        }

        public struct CTargaHead
        {
            public byte idLength;     // number of chars in identification field
            public byte colorMapType;	// color map type
            public byte imageType;	// image type code
            public byte[] cMapOrigin;// color map origin // 2
            public byte[] cMapLength;// color map length // 2
            public byte cMapBits;     // color map entry size
            public short x0;			// x-origin of image
            public short y0;			// y-origin of image
            public short width;		// width of image
            public short height;		// height of image
            public byte pixelSize;    // image pixel size
            public byte descriptor;   // image descriptor byte
        };

        private byte[] SeralizeCTargaHeadAsArray(CTargaHead h)
        {
            return BitConverter.GetBytes(h.idLength).Concat(
            BitConverter.GetBytes(h.colorMapType)).Concat(
            BitConverter.GetBytes(h.imageType)).Concat(
            BitConverter.GetBytes(h.cMapOrigin[0])).Concat(
            BitConverter.GetBytes(h.cMapOrigin[1])).Concat(
            BitConverter.GetBytes(h.cMapLength[0])).Concat(
            BitConverter.GetBytes(h.cMapLength[1])).Concat(
            BitConverter.GetBytes(h.cMapBits)).Concat(
            BitConverter.GetBytes(h.x0)).Concat(
            BitConverter.GetBytes(h.y0)).Concat(
            BitConverter.GetBytes(h.width)).Concat(
            BitConverter.GetBytes(h.height)).Concat(
            BitConverter.GetBytes(h.pixelSize)).Concat(
            BitConverter.GetBytes(h.descriptor)).ToArray();
        }

        // Image data type codes
        const byte TargaRawColormap = 1;
        const byte TargaRawRGB = 2;
        const byte TargaRawBW = 3;
        const byte TargaRunColormap = 9;
        const byte TargaRunRGB = 10;
        const byte TargaRunBW = 11;

        // Descriptor fields
        const int TargaAttrBits = 15;
        const int TargaScreenOrigin = (1 << 5);
        const int TargaCMapSize = 256;
        const int TargaCMapBands = 3;
        /*
        const int TargaInterleaveShift = 6;
        const int TargaNON_INTERLEAVE	0
        const int TargaTWO_INTERLEAVE	1
        const int TargaFOUR_INTERLEAVE	2
        const int PERMUTE_BANDS		1
        */

        void WriteFileTGA(CImage img, string filename)
        {
            // Only 1, 3, or 4 bands supported
            CShape sh = img.Shape();
            int nBands = sh.nBands;
            if (nBands != 1 && nBands != 3 && nBands != 4)
                throw new Exception(string.Format("WriteFileTGA({0}): can only write 1, 3, or 4 bands", filename));

            //    // Only unsigned_8 supported directly
            //#if 0   // broken for now
            //    if (img.PixType() != unsigned_8)
            //    {
            //        CImage u8img(sh, unsigned_8);
            //        TypeConvert(img, u8img);
            //        img = u8img;
            //    }
            //#endif

            // Fill in the header structure
            CTargaHead h = new CTargaHead();
            h.cMapOrigin = new byte[2];
            h.cMapLength = new byte[2];
            //memset(&h, 0, sizeof(h));
            h.imageType = (nBands == 1) ? TargaRawBW : TargaRawRGB;
            // TODO:  is TargaRawBW the right thing, or only binary?
            h.width = (short)sh.width;
            h.height = (short)sh.height;
            h.pixelSize = Convert.ToByte(8 * nBands);
            bool reverseRows = false;   // TODO: when is this true?

            using (System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Create))
            {
                //// Open the file and write the header
                //FILE *stream = fopen(filename, "wb");
                //if (stream == 0)
                //    throw CError("WriteFileTGA: could not open %s", filename);
                //if (fwrite(&h, sizeof(CTargaHead), 1, stream) != 1)
                //    throw CError("WriteFileTGA(%s): file is too short", filename);
                var arrayToSend = SeralizeCTargaHeadAsArray(h);
                stream.Write(arrayToSend, 0, arrayToSend.Length);
                // Write out the rows
                for (int y = 0; y < sh.height; y++)
                {
                    int yr = reverseRows ? sh.height - 1 - y : y;
                    var ptr = img.GetPixelAddress(0, yr, 0);
                    int n = sh.width * sh.nBands; // THIS IS HOW MANY PIXELS WE NEED TO WRITE OUT a whole rows worth!
                    //works really great in c++ but not so great in our modified c# version!!
                    //and with this crap about reversing rows. was that a quirk of the C++ Code?
                    throw new NotImplementedException("Have to figure out how to write the bytes that represent the pixels here!");
                    //if (fwrite(ptr, sizeof(uchar), n, stream) != n)
                    //  throw new Exception(string.Format("WriteFileTGA({0}): file is too short", filename));
                }

                //if (fclose(stream))
                //    throw new Exception(string.Format("WriteFileTGA({0}): error closing file", filename));
            }
        }

    }
}
