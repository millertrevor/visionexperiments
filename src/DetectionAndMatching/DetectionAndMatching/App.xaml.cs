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
            var reader = new ImageReader.ImageReader("test.jpg");

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


        bool LoadImageFile(string filename, CImageOfFloat image)
        {
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
                return false;//Until we figure out hte fall back with FileIO
            }
            else
            {
                CShape sh = new CShape(fl_image.Width, fl_image.Height, 3);
                image = new CImageOfFloat(sh);

                // Convert the image to the CImage format.
                if (!convertImage(fl_image, image))
                {
                    Console.WriteLine("Couldn't convert image to RGB format\n");
                    return false;
                }

                return true;
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
            int d = image.d;

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
                        convertedImage.SetPixel(x, y, 0, (data[index]    ) / 255.0f);
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
                        var item = floatImage.GetPixel(x, sh.height - y - 1, Math.Min(3, sh.nBands) - c - 1);
                        item = value;
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
            bool success = false;
            success = LoadImageFile(argv[1], floatQueryImage);

            if (!success)
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
            return false;
        }
    }

}
