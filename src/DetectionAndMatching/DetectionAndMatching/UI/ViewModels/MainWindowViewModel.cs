﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DetectionAndMatching.UI.Models;
using PixelMap;

namespace DetectionAndMatching.UI.ViewModels
{
    using System.IO;
    using System.Windows.Shapes;

    using ImageLib;

    using ImageReader;

    using Point = System.Windows.Point;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _dirLocation;
        bool _showingFeatures = true;


        private string _queryImage;
        public FeatureSet _queryFeatures; 

        private ImageDatabase db;

        private string _resultImage;

        private int matchType;
        
        public void SelectAllFeaturesAt(double X, double Y)
        {
            if (QueryFeatures != null)
            {
                QueryFeatures.select_point(X, Y);
            }
        }
        public void SelectAllFeaturesInArea(double xMin, double xMax, double yMin, double yMax)
        {
            if (QueryFeatures != null)
            {
                QueryFeatures.select_box(xMin, xMax, yMin, yMax);
            }
        }

        private HistogramWindowViewModel _histogramWindowViewModel;
        public HistogramWindowViewModel HistogramWindowViewModel
        {
            get { return _histogramWindowViewModel; }
            set { _histogramWindowViewModel = value;NotifyPropertyChanged("HistogramWindowViewModel"); }
        }
        public FeatureSet QueryFeatures
        {
            get { return _queryFeatures; }
            set { _queryFeatures = value; NotifyPropertyChanged("QueryFeatures"); }
        }
        private string _leftPictureLocation;
        public String LeftPictureLocation
        {
            get { return _leftPictureLocation; }
            set
            {
                _leftPictureLocation = value;
                NotifyPropertyChanged("LeftPictureLocation");
            }
        }
        private string _rightPictureLocation;
        public String RightPictureLocation
        {
            get { return _rightPictureLocation; }
            set
            {
                _rightPictureLocation = value;
                NotifyPropertyChanged("RightPictureLocation");
            }
        }
        
        private int _leftImageWidth;
        private int _leftImageHeight;

        public int LeftImageWidth
        {
            get { return _leftImageWidth; }
            set
            {
                _leftImageWidth = value;
                NotifyPropertyChanged("LeftImageWidth");
            }
        }

        public int LeftImageHeight
        {
            get { return _leftImageHeight; }
            set
            {
                _leftImageHeight = value;
                NotifyPropertyChanged("LeftImageHeight");
            }
        }
        private int _rightImageWidth;
        private int _rightImageHeight;

        public int RightImageWidth
        {
            get { return _rightImageWidth; }
            set
            {
                _rightImageWidth = value;
                NotifyPropertyChanged("RightImageWidth");
            }
        }

        public int RightImageHeight
        {
            get { return _rightImageHeight; }
            set
            {
                _rightImageHeight = value;
                NotifyPropertyChanged("RightImageHeight");
            }
        }

        private ICommand _loadQueryImage;
        public ICommand LoadQueryImage
        {
            get
            {
                return _loadQueryImage ??
                       (_loadQueryImage =
                        new RelayCommand(param => LoadQueryImageExecute(), param => LoadQueryImageEnabled));
            }
        }
         private bool _loadQueryImageEnabled = true;
        public bool LoadQueryImageEnabled
        {
            get { return _loadQueryImageEnabled; }
            set { _loadQueryImageEnabled = value; }
        }
        public void LoadRightImage(string fullFileName)
        {
            // Open document 
            string filename = fullFileName;
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            if (fi.Extension == ".ppm" || fi.Extension == ".pgm")
            {
                filename = ConvertToJpeg(fi);
            }

            //_dirLocation = dlg.InitialDirectory;
            // _doc.load_query_image(filename);
            RightPictureLocation = filename;
            load_result_image(RightPictureLocation);
            //var bi = new BitmapImage(new Uri(LeftPictureLocation, UriKind.RelativeOrAbsolute));
            var bi = new ImageReader(RightPictureLocation);
            
            HistogramWindowViewModel.LuminanceHistogramPointsR = ConvertToPointCollection(bi.luminance);
            HistogramWindowViewModel.RedColorHistogramPointsR = ConvertToPointCollection(bi.r);
            HistogramWindowViewModel.GreenColorHistogramPointsR = ConvertToPointCollection(bi.g);
            HistogramWindowViewModel.BlueColorHistogramPointsR = ConvertToPointCollection(bi.b);
            
            RightImageHeight = bi.Height;
            RightImageWidth = bi.Width;
        }
        private void LoadQueryImageExecute()
        {
            RightImageWidth = 0;
            RightImageHeight = 0;
            RightPictureLocation = "";
            var dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image Files (*.jpg)|*.jpg;*.pgm;*.ppm";

            if (string.IsNullOrEmpty(_dirLocation))
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
            }

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                if (fi.Extension == ".ppm" || fi.Extension==".pgm")
                {
                    filename = ConvertToJpeg(fi);
                }
                _dirLocation = dlg.InitialDirectory;
               // _doc.load_query_image(filename);
                LeftPictureLocation = filename;
                load_query_image(LeftPictureLocation);
                
                var bi = new ImageReader(LeftPictureLocation);
                var hwvm = new HistogramWindowViewModel
                               {
                                   LuminanceHistogramPoints = ConvertToPointCollection(bi.luminance),
                                   RedColorHistogramPoints = ConvertToPointCollection(bi.r),
                                   GreenColorHistogramPoints = ConvertToPointCollection(bi.g),
                                   BlueColorHistogramPoints = ConvertToPointCollection(bi.b)
                               };

                HistogramWindowViewModel = hwvm;

                LeftImageHeight = bi.Height;
                LeftImageWidth = bi.Width;                
            }
        }
        private string ConvertToJpeg(System.IO.FileInfo incomingInfo)
        {
            PixelMap.PixelMap pm = new PixelMap.PixelMap(incomingInfo.FullName);

            return pm.SaveBitmapAsJpeg(incomingInfo);
        }
        private ICommand _loadQueryFeaturesSIFT;
        public ICommand LoadQueryFeaturesSIFT
        {
            get
            {
                return _loadQueryFeaturesSIFT ??
                       (_loadQueryFeaturesSIFT =
                        new RelayCommand(param => LoadQueryFeaturesSIFTExecute(), param => LoadQueryFeaturesSIFTEnabled));
            }
        }

        private bool _loadQueryFeaturesSIFTEnabled = true;
        public bool LoadQueryFeaturesSIFTEnabled
        {
            get { return _loadQueryFeaturesSIFTEnabled; }
            set { _loadQueryFeaturesSIFTEnabled = value; }
        }
        private void LoadQueryFeaturesSIFTExecute()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".key";
            dlg.Filter = "KEY Files (*.key)|*.key";
            if (string.IsNullOrEmpty(_dirLocation))
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
            }
            
            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                _showingFeatures = true;
                // Open document 
                string filename = dlg.FileName;
                _dirLocation = dlg.InitialDirectory;
                load_query_features(filename, true);
            }
        }
       // SelectAllFeatures
        private ICommand _selectFeaturesCommand;
        public ICommand SelectAllFeatures
        {
            get
            {
                return _selectFeaturesCommand ??
                       (_selectFeaturesCommand =
                        new RelayCommand(param => SelectAllFeaturesExecute((string)param), param => SelectAllFeaturesEnabled));
            }
        }
        private bool _selectAllFeaturesEnabled = true;
        public bool SelectAllFeaturesEnabled
        {
            get { return _selectAllFeaturesEnabled; }
            set { _selectAllFeaturesEnabled = value; }
        }

        private void SelectAllFeaturesExecute(string incomingValue)
        {
            if (incomingValue == "Select")
            {
                select_all_query_features();
            }
            else if (incomingValue == "Deselect")
            {
                deselect_all_query_features();
            }
        }

        private ICommand _toggleFeaturesCommand;
        public ICommand ToggleFeatures
        {
            get
            {
                return _toggleFeaturesCommand ??
                       (_toggleFeaturesCommand =
                        new RelayCommand(param => ToggleFeaturesExecute(), param => ToggleFeaturesEnabled));
            }
        }
         private bool _toggleFeaturesEnabled = true;
        public bool ToggleFeaturesEnabled
        {
            get { return _toggleFeaturesEnabled; }
            set { _toggleFeaturesEnabled = value; }
        }

        private void ToggleFeaturesExecute()
        {
            System.Windows.Visibility v = Visibility.Hidden;
            if (_showingFeatures)
            {
                _showingFeatures = false;
            }
            else
            {
                v = Visibility.Visible;
                _showingFeatures = true;
            }
            foreach (var item in QueryFeatures)
            {
                item.Visibility = v;
            }
        }

         private ICommand _loadDataBaseCommand;
        public ICommand LoadDataBaseCommand
        {
            get
            {
                return _loadDataBaseCommand ??
                       (_loadDataBaseCommand =
                        new RelayCommand(param => LoadDataBaseExecute((string)param), param => LoadDataBaseEnabled));
            }
        }
         private bool _loadDataBaseEnabled = true;
        public bool LoadDataBaseEnabled
        {
            get { return _loadDataBaseEnabled; }
            set { _loadDataBaseEnabled = value; }
        }

        private void LoadDataBaseExecute(string type)
        {
            if (type == "SIFT")
            {
                LoadSiftDatabase();
            }
            else
            {
                LoadDataBase();
            }
        }

        private ICommand _localMeanCommand;
        public ICommand LocalMeanCommand
        {
            get
            {
                return _localMeanCommand
                       ?? (_localMeanCommand =
                           new RelayCommand(param => LocalMeanCommandExecute(), param => LocalMeanCommandEnabled));
            }
        }

        private bool _localMeanCommandEnabled = true;
        public bool LocalMeanCommandEnabled
        {
            get { return _localMeanCommandEnabled; }
            set { _localMeanCommandEnabled = value; }
        }

        private void LocalMeanCommandExecute()
        {
            var image = new ImageReader(LeftPictureLocation);
            int stroke = image.Width * 3;
            const int StartBorder = 0;
            const int EndBorder = 1; //border of one for 3x3 mean block

            var resultImage = new ImageReader();
            resultImage.Count = image.Count;
            resultImage.Height = image.Height;
            resultImage.Width = image.Width;
            resultImage.depth = image.depth;
            resultImage.Pixels = new List<byte>(image.Pixels);


            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h > StartBorder && h < image.Height - EndBorder)
                    {
                        if (w > StartBorder && w < image.Width - EndBorder)
                        {
                            //can average here
                            var returnList = this.Average(w, h, 3, image);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            //var newImageFile = new ImageReader.ImageReader();
            //newImageFile.Width = image.Width;
            //newImageFile.Height = image.Height;
            //newImageFile.Pixels = image.Pixels;
            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "LocalMean" + fi.Name;
            resultImage.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }

        private List<byte> Average(int x, int y, int squareSize, ImageReader image)
        {
            var returnList = new List<byte>();

            var oneR = image.GetPixel(x - 1, y - 1, 0);
            var twoR = image.GetPixel(x, y - 1, 0);
            var threeR = image.GetPixel(x + 1, y - 1, 0);
            var fourR = image.GetPixel(x - 1, y, 0);
            var fiveR = image.GetPixel(x, y, 0);
            var sixR = image.GetPixel(x + 1, y, 0);
            var sevenR = image.GetPixel(x - 1, y + 1, 0);
            var eightR = image.GetPixel(x, y + 1, 0);
            var nineR = image.GetPixel(x + 1, y + 1, 0);

            var oneG = image.GetPixel(x - 1, y - 1, 1);
            var twoG = image.GetPixel(x, y - 1, 1);
            var threeG = image.GetPixel(x + 1, y - 1, 1);
            var fourG = image.GetPixel(x - 1, y, 1);
            var fiveG = image.GetPixel(x, y, 1);
            var sixG = image.GetPixel(x + 1, y, 1);
            var sevenG = image.GetPixel(x - 1, y + 1, 1);
            var eightG = image.GetPixel(x, y + 1, 1);
            var nineG = image.GetPixel(x + 1, y + 1, 1);

            var oneB = image.GetPixel(x - 1, y - 1, 2);
            var twoB = image.GetPixel(x, y - 1, 2);
            var threeB = image.GetPixel(x + 1, y - 1, 2);
            var fourB = image.GetPixel(x - 1, y, 2);
            var fiveB = image.GetPixel(x, y, 2);
            var sixB = image.GetPixel(x + 1, y, 2);
            var sevenB = image.GetPixel(x - 1, y + 1, 2);
            var eightB = image.GetPixel(x, y + 1, 2);
            var nineB = image.GetPixel(x + 1, y + 1, 2);

            var r = (oneR + twoR + threeR + fourR + fiveR + sixR + sevenR + eightR + nineR) / 9;
            var g = (oneG + twoG + threeG + fourG + fiveG + sixG + sevenG + eightG + nineG) / 9;
            var b = (oneB + twoB + threeB + fourB + fiveB + sixB + sevenB + eightB + nineB) / 9;
            returnList.Add((byte)r);
            returnList.Add((byte)g);
            returnList.Add((byte)b);

            return returnList;
        }



        private ICommand _localMedianCommand;
        public ICommand LocalMedianCommand
        {
            get
            {
                return _localMedianCommand
                       ?? (_localMedianCommand =
                           new RelayCommand(param => LocalMedianCommandExecute(), param => LocalMedianCommandEnabled));
            }
        }

        private bool _localMedianCommandEnabled = true;
        public bool LocalMedianCommandEnabled
        {
            get { return _localMedianCommandEnabled; }
            set { _localMedianCommandEnabled = value; }
        }

        private void LocalMedianCommandExecute()
        {
            var image = new ImageReader(LeftPictureLocation);
            int stroke = image.Width * 3;
            const int StartBorder = 0;
            const int EndBorder = 1; //border of one for 3x3 mean block
            var resultImage = new ImageReader();
            resultImage.Count = image.Count;
            resultImage.Height = image.Height;
            resultImage.Width = image.Width;
            resultImage.depth = image.depth;
            resultImage.Pixels = new List<byte>(image.Pixels);

            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h > StartBorder && h < image.Height - EndBorder)
                    {
                        if (w > StartBorder && w < image.Width - EndBorder)
                        {
                            //can average here
                            var returnList = this.FindMedians(w, h, 3, image);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            //var newImageFile = new ImageReader.ImageReader();
            //newImageFile.Width = image.Width;
            //newImageFile.Height = image.Height;
            //newImageFile.Pixels = image.Pixels;
            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "LocalMedian" + fi.Name;
            resultImage.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }
        private List<byte> FindMedians(int x, int y, int squareSize, ImageReader image)
        {
            var returnList = new List<byte>();

            var oneR = image.GetPixel(x - 1, y - 1, 0);
            var twoR = image.GetPixel(x, y - 1, 0);
            var threeR = image.GetPixel(x + 1, y - 1, 0);
            var fourR = image.GetPixel(x - 1, y, 0);
            var fiveR = image.GetPixel(x, y, 0);
            var sixR = image.GetPixel(x + 1, y, 0);
            var sevenR = image.GetPixel(x - 1, y + 1, 0);
            var eightR = image.GetPixel(x, y + 1, 0);
            var nineR = image.GetPixel(x + 1, y + 1, 0);

            var oneG = image.GetPixel(x - 1, y - 1, 1);
            var twoG = image.GetPixel(x, y - 1, 1);
            var threeG = image.GetPixel(x + 1, y - 1, 1);
            var fourG = image.GetPixel(x - 1, y, 1);
            var fiveG = image.GetPixel(x, y, 1);
            var sixG = image.GetPixel(x + 1, y, 1);
            var sevenG = image.GetPixel(x - 1, y + 1, 1);
            var eightG = image.GetPixel(x, y + 1, 1);
            var nineG = image.GetPixel(x + 1, y + 1, 1);

            var oneB = image.GetPixel(x - 1, y - 1, 2);
            var twoB = image.GetPixel(x, y - 1, 2);
            var threeB = image.GetPixel(x + 1, y - 1, 2);
            var fourB = image.GetPixel(x - 1, y, 2);
            var fiveB = image.GetPixel(x, y, 2);
            var sixB = image.GetPixel(x + 1, y, 2);
            var sevenB = image.GetPixel(x - 1, y + 1, 2);
            var eightB = image.GetPixel(x, y + 1, 2);
            var nineB = image.GetPixel(x + 1, y + 1, 2);

            var r = GetMedian(new List<byte> { oneR, twoR, threeR, fourR, fiveR, sixR, sevenR, eightR, nineR });
            var g = GetMedian(new List<byte> { oneG, twoG, threeG, fourG, fiveG, sixG, sevenG, eightG, nineG });
            var b = GetMedian(new List<byte> { oneB, twoB, threeB, fourB, fiveB, sixB, sevenB, eightB, nineB });
            returnList.Add(r);
            returnList.Add(g);
            returnList.Add(b);

            return returnList;
        }


       // public static decimal GetMedian(this IEnumerable<int> source) //could make it an extention method
        public static byte GetMedian(IEnumerable<byte> source)
        {
            // Create a copy of the input, and sort the copy
            byte[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (byte)((a + b) / 2); // TODO: possible rounding issue here. but we are using an odd number so it isn't exposed.
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }


        private ICommand _gaussianBlurCommand;
        public ICommand GaussianBlurCommand
        {
            get
            {
                return _gaussianBlurCommand ??
                       (_gaussianBlurCommand =
                        new RelayCommand(param => GaussianBlurCommandExecute((string)param), param => GaussianBlurCommandEnabled));
            }
        }

        private bool _gaussianBlurCommandEnabled = true;
        public bool GaussianBlurCommandEnabled
        {
            get { return _gaussianBlurCommandEnabled; }
            set { _gaussianBlurCommandEnabled = value; }
        }

        private void GaussianBlurCommandExecute(string sigmaStringValue)
        {
            double sigma = Convert.ToDouble(sigmaStringValue);
            //double sigma = 4;
            var image = new ImageReader(LeftPictureLocation);
            int stroke = image.Width * 3;
            const int StartBorder = 5;
            //need to skip 5 spaces to make the 11x11 i think
            const int EndBorder = 5; //border of ? for 11x11 for block
            var resultImage = new ImageReader();
            resultImage.Count = image.Count;
            resultImage.Height = image.Height;
            resultImage.Width = image.Width;
            resultImage.depth = image.depth;
            resultImage.Pixels = new List<byte>(image.Pixels);

            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h >= StartBorder && h < image.Height - EndBorder)
                    {
                        if (w >= StartBorder && w < image.Width - EndBorder)
                        {
                            var returnList = this.FindGaussian(w, h, 5, 1.4, image);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            //var newImageFile = new ImageReader.ImageReader();
            //newImageFile.Width = image.Width;
            //newImageFile.Height = image.Height;
            //newImageFile.Pixels = image.Pixels;
            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "GaussianBlur"+sigma.ToString() + fi.Name;
            resultImage.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }
        
        private List<byte> FindGaussian(int x, int y, int size, double sigma, ImageReader image)
        {
            var locationP = (size - 1) / 2;
            GaussianBlur gb = new GaussianBlur(sigma, size);

            var countercounter = 0.0;
            foreach (var item in gb.Kernel)
            {
                countercounter += (double)item / (double)gb.Divisor;
            }
            List<byte> windowR = new List<byte>();
            for (int j = -locationP; j < locationP + 1; j++)
            {
                for (int i = -locationP; i < locationP + 1; i++)
                {
                    windowR.Add(image.GetPixel(x + i, y + j, 0));
                }
            }

            var byteWindowR = new byte[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    byteWindowR[i, j] = windowR[counter];
                    counter++;
                }
            }

            List<byte> windowG = new List<byte>();
            for (int j = -locationP; j < locationP + 1; j++)
            {
                for (int i = -locationP; i < locationP + 1; i++)
                {
                    windowG.Add(image.GetPixel(x + i, y + j, 1));
                }
            }

            var byteWindowG = new byte[size, size];
            counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    byteWindowG[i, j] = windowG[counter];
                    counter++;
                }
            }

            List<byte> windowB = new List<byte>();
            for (int j = -locationP; j < locationP + 1; j++)
            {
                for (int i = -locationP; i < locationP + 1; i++)
                {
                    windowB.Add(image.GetPixel(x + i, y + j, 2));
                }
            }

            var byteWindowB = new byte[size, size];
            counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    byteWindowB[i, j] = windowB[counter];
                    counter++;
                }
            }



            var returnList = new List<byte>();

            var runningR = 0.0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    runningR += byteWindowR[i, j] * gb.Kernel[i, j];
                }
            }

            var runningG = 0.0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    runningG += byteWindowG[i, j] * gb.Kernel[i, j];
                }
            }

            var runningB = 0.0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    runningB += byteWindowB[i, j] * gb.Kernel[i, j];
                }
            }

            var origR = image.GetPixel(x, y, 0);
            var origG = image.GetPixel(x, y, 1);
            var origB = image.GetPixel(x, y, 2);
            var r = (byte)Math.Round(runningR / gb.Divisor,MidpointRounding.AwayFromZero);
            var g = (byte)Math.Round(runningG / gb.Divisor, MidpointRounding.AwayFromZero);
            var b = (byte)Math.Round(runningB / gb.Divisor, MidpointRounding.AwayFromZero);
            returnList.Add(r);
            returnList.Add(g);
            returnList.Add(b);

            return returnList;
        }

        private ICommand _sobelCommand;
        public ICommand SobelCommand
        {
            get
            {
                return _sobelCommand ??
                       (_sobelCommand =
                        new RelayCommand(param => SobelCommandExecute(), param => SobelCommandEnabled));
            }
        }

        private bool _sobelCommandEnabled = true;
        public bool SobelCommandEnabled
        {
            get { return _sobelCommandEnabled; }
            set { _sobelCommandEnabled = value; }
        }

        private void SobelCommandExecute()
        {
            var image = new ImageReader(LeftPictureLocation);
            image.ConvertToGrey();
            const int StartBorder = 0;
            const int EndBorder = 1; //border of one for 3x3 mean block
            var resultImage = new ImageReader();
            resultImage.Count = image.Count;
            resultImage.Height = image.Height;
            resultImage.Width = image.Width;
            resultImage.depth = image.depth;
            resultImage.Pixels = new List<byte>(image.Pixels);

            //Gauss the image first
            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h >= 2 && h < image.Height - 2)
                    {
                        if (w >= 2 && w < image.Width - 2)
                        {
                            var returnList = this.FindGaussian(w, h, 5, 1.4, image);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            var GaussImage = new ImageReader();
            GaussImage.Count = resultImage.Count;
            GaussImage.Height = resultImage.Height;
            GaussImage.Width = resultImage.Width;
            GaussImage.depth = resultImage.depth;
            GaussImage.Pixels = new List<byte>(resultImage.Pixels);
            
            //Sobel operator next
            for (var h = 0; h < GaussImage.Height; h++)
            {
                for (var w = 0; w < GaussImage.Width; w++)
                {
                    if (h > StartBorder && h < GaussImage.Height - EndBorder)
                    {
                        if (w > StartBorder && w < GaussImage.Width - EndBorder)
                        {
                            var returnList = this.SobelDetection(w, h, GaussImage);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }
            
            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "Sobel" + fi.Name;
            resultImage.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }


        private ICommand _houghTransformCommand;
        public ICommand HoughTransformCommand
        {
            get
            {
                return _houghTransformCommand
                       ?? (_houghTransformCommand =
                           new RelayCommand(
                               param => HoughTransformCommandExecute(), param => HoughTransformCommandEnabled));
            }
        }

        private bool _houghTransformCommandEnabled = true;
        public bool HoughTransformCommandEnabled
        {
            get { return _houghTransformCommandEnabled; }
            set { _houghTransformCommandEnabled = value; }
        }
        //m = -cotθ  --- Math.PI/2 - Math.Atan(x)
        //c = p*cosecθ
        //cosecant (csc), secant (sec), and cotangent (cot).
        //Cosecant Cosec(X) = 1 / Sin(X) 
        //Cotangent Cotan(X) = 1 / Tan(X) 
        // var m = -1*(1/Math.tan(x));
        // var c = p*(1/Math.Sin(x));
     
        private void HoughTransformCommandExecute()
        {
            var image = new ImageReader(LeftPictureLocation);
            int rhoMax =(int) Math.Round(Math.Sqrt((image.Width*image.Width)+(image.Height*image.Height)));
              double[,] A = new double[180,rhoMax*2];
              List<Tuple<double, double, double>> listOfThings = new List<Tuple<double, double, double>>();
            // var gaussImageToSend = new byte[image.Width,image.Height];
            List<string> outputList = new List<string>();
            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    var pixel = image.GetPixel(w, h, 0);
                    if (pixel == 255)
                    {
                        for (int i = -90; i <= 90; i++)
                        {
                            double angle = Math.PI * i / 180.0;
                            var rho = (w * Math.Cos(angle)) + (h * Math.Sin(angle));
                            var rhoRounded = Math.Round(rho, 4);
                            int angleAccessor = (int)angle + 90;
                            int rhoAccessor = ((int)Math.Round(rho))+rhoMax;
                            A[angleAccessor, rhoAccessor]++;
                            bool found = false;
                            Tuple<double, double, double> itemToRemove = null;
                            foreach (var item in listOfThings)
                            {
                                if (item.Item1 == i && item.Item2 == rhoRounded)
                                {
                                    found = true;
                                    itemToRemove = item;
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                listOfThings.Add(new Tuple<double, double, double>(i, rhoRounded, 1));
                            }
                            else
                            {
                                listOfThings.Remove(itemToRemove);
                                listOfThings.Add(new Tuple<double, double, double>(itemToRemove.Item1, itemToRemove.Item2, itemToRemove.Item3 + 1));

                            }
                           outputList.Add(rho.ToString() + "," + i.ToString());
                        }
                    }
                }
            }
            System.IO.File.WriteAllLines("test.txt", outputList.ToArray());
            int counter = 0;
            //y = m*x + c
            var newLinesImage = new ImageReader();
            newLinesImage.Width = image.Width;
            newLinesImage.Height = image.Height;
            newLinesImage.Pixels = new List<byte>(image.Pixels);
            newLinesImage.depth = image.depth;
            newLinesImage.Count = image.Count;
            
            foreach (var item in listOfThings)
            {
                //if (item.Item3 >2)
               // {
                    counter++;
                    var angle = item.Item1;
                    double x = Math.PI * angle / 180.0;
                    var p = item.Item2;
                    var m = -1*(1/Math.Tan(x));
                    var c = p*(1/Math.Sin(x));
                    for (int ix = 0; ix < newLinesImage.Width; ix++)
                    {
                        var iy =(int)( (m * ix) + c);
                        if (iy > 0 && iy < newLinesImage.Height)
                        {
                            newLinesImage.SetPixel(ix, iy, 0, 255);
                        }
                    }
                //}
            }
            int stop = -1;
            newLinesImage.SaveAsBitmap("MyLinesOutputFromHT.bmp");
        }

        private ICommand _cannyCommand;
        public ICommand CannyCommand
        {
            get
            {
                return _cannyCommand ??
                       (_cannyCommand =
                        new RelayCommand(param => CannyCommandExecute(), param => CannyCommandEnabled));
            }
        }

        private bool _cannyCommandEnabled = true;
        public bool CannyCommandEnabled
        {
            get { return _cannyCommandEnabled; }
            set { _cannyCommandEnabled = value; }
        }

        private void CannyCommandExecute()
        {
           // CannyEdgeDetector ce = new CannyEdgeDetector();
            //ce.Apply(LeftPictureLocation);
            var image = new ImageReader(LeftPictureLocation);
            image.ConvertToGrey();
            image.SaveAsBitmap("1GreyTest.bmp");
            //Gauss the image first
            var gaussImageToSend = new byte[image.Width,image.Height];
            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h >= 2 && h < image.Height - 2)
                    {
                        if (w >= 2 && w < image.Width - 2)
                        {
                            var returnList = this.FindGaussian(w, h, 5, 1.4, image);
                            //resultImage.SetPixel(w, h, 0, returnList[0]);
                           // resultImage.SetPixel(w, h, 1, returnList[1]);
                            //resultImage.SetPixel(w, h, 2, returnList[2]);
                            gaussImageToSend[w, h] = returnList[0];
                        }
                    }
                }
            }

            var rect = new Rect(0, 0, image.Width, image.Height);
            var returnImage = ProcessCannyFilter(gaussImageToSend, rect, 10,15);
            ImageReader processedImage = new ImageReader();
            processedImage.Height = image.Height;
            processedImage.Width = image.Width;
            processedImage.Count = image.Count;
            processedImage.depth = image.depth;
            processedImage.Pixels = new List<byte>();
            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    byte value = 0;
                    if (returnImage[w,h] > 0)
                    {
                        value = 255;
                    }
                    processedImage.Pixels.Add(value);
                    processedImage.Pixels.Add(value);
                    processedImage.Pixels.Add(value);
                }
            }


            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "1TryMightEdgeCanny" + fi.Name;
            //resultImage.SaveAsBitmap(modifiedName);
            //LinesImage.SaveAsBitmap(modifiedName);
            processedImage.SaveAsBitmap(modifiedName);

            LoadRightImage(modifiedName);
        }

        private int[] FindGradient(int x, int y, byte[,] image)
        {
            List<int> stuff = new List<int>();


            var oneR = image[x - 1, y - 1];
            var twoR = image[x, y - 1];
            var threeR = image[x + 1, y - 1];
            var fourR = image[x - 1, y];
            var fiveR = image[x, y];
            var sixR = image[x + 1, y];
            var sevenR = image[x - 1, y + 1];
            var eightR = image[x, y + 1];
            var nineR = image[x + 1, y + 1];

            var GX = new List<int> { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            var GY = new List<int> { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
            var RArray = new List<byte> { oneR, twoR, threeR, fourR, fiveR, sixR, sevenR, eightR, nineR };

            int GXR = 0;
            int GYR = 0;
            for (int i = 0; i < 9; i++)
            {
                GXR += GX[i] * RArray[i];

                GYR += GY[i] * RArray[i];
            }
            //var newR = Math.Sqrt(GXR * GXR + GYR * GYR);
            stuff.Add(GXR);
            stuff.Add(GYR);

            return stuff.ToArray();
        }

        private byte[,] ProcessCannyFilter(byte[,] image, Rect rect, byte lowThreshold, byte highThreshold)
        {
            // processing start and stop X,Y positions
            int startX = (int)rect.X + 1;
            int startY = (int)rect.Y + 1;
            int stopX = (int)(startX + rect.Width - 2);
            int stopY = (int)(startY + rect.Height - 2);

            int width = (int)(rect.Width - 2);
            int height = (int)(rect.Height - 2);

            int dstStride = (int)rect.Width;
            int srcStride = (int)rect.Width;

            int dstOffset = dstStride - (int)rect.Width + 2;
            int srcOffset = srcStride - (int)rect.Width + 2;

            // pixel's value and gradients
            int gx, gy;
            //
            double orientation, toAngle = 180.0 / Math.PI;
            float leftPixel = 0, rightPixel = 0;

            // TODO: Image already comes in as BW and Gaussian Blurred
            // STEP 1 - blur image
           // UnmanagedImage blurredImage = gaussianFilter.Apply(source);
           // blurredImage.ToManagedImage(true).Save("CEGAUSS.bmp");
            // orientation array
            byte[] orients = new byte[width * height];
            // gradients array
            float[,] gradients = new float[(int)(rect.Width), (int)(rect.Height)];
            float maxGradient = float.NegativeInfinity;

            // do the job
           // byte* src = (byte*)blurredImage.ImageData.ToPointer();
            var src = image;
            // allign pointer
           // src += srcStride * startY + startX;

            // STEP 2 - calculate magnitude and edge orientation
            int p = 0;
            
            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, p++)
                {
                    var list = FindGradient(x, y, src);
                    gx = list[0];
                    gy = list[1];
                    //gx = src[-srcStride + 1] + src[srcStride + 1]
                    //   - src[-srcStride - 1] - src[srcStride - 1]
                    //   + 2 * (src[1] - src[-1]);

                    //gy = src[-srcStride - 1] + src[-srcStride + 1]
                    //   - src[srcStride - 1] - src[srcStride + 1]
                    //   + 2 * (src[-srcStride] - src[srcStride]);

                    // get gradient value
                    gradients[x, y] = (float)Math.Sqrt(gx * gx + gy * gy);
                    if (gradients[x, y] > maxGradient)
                        maxGradient = gradients[x, y];

                    // --- get orientation
                    if (gx == 0)
                    {
                        // can not divide by zero
                        orientation = (gy == 0) ? 0 : 90;
                    }
                    else
                    {
                        double div = (double)gy / gx;

                        // handle angles of the 2nd and 4th quads
                        if (div < 0)
                        {
                            orientation = 180 - System.Math.Atan(-div) * toAngle;
                        }
                        // handle angles of the 1st and 3rd quads
                        else
                        {
                            orientation = System.Math.Atan(div) * toAngle;
                        }

                        // get closest angle from 0, 45, 90, 135 set
                        if (orientation < 22.5)
                            orientation = 0;
                        else if (orientation < 67.5)
                            orientation = 45;
                        else if (orientation < 112.5)
                            orientation = 90;
                        else if (orientation < 157.5)
                            orientation = 135;
                        else orientation = 0;
                    }

                    // save orientation
                    orients[p] = (byte)orientation;
                }
                //src += srcOffset;
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // byte[] orients = new byte[width * height];
            // gradients array
            // float[,] gradients = new float[source.Width, source.Height];
            //Console.WriteLine("Writing out gradients");
            //foreach (var item in gradients)
            //{
            //    Console.WriteLine(item);
            //}
            //////////////////////////////////////////////////////////////////////////////
            // STEP 3 - suppres non maximums
         //   byte* dst = (byte*)destination.ImageData.ToPointer();
            // allign pointer
          //  dst += dstStride * startY + startX;
            var dst = new byte[(int)rect.Width,(int)rect.Height];

            p = 0;

            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++, p++)
                {
                    // get two adjacent pixels
                    switch (orients[p])
                    {
                        case 0:
                            leftPixel = gradients[x - 1, y];
                            rightPixel = gradients[x + 1, y];
                            break;
                        case 45:
                            leftPixel = gradients[x - 1, y + 1];
                            rightPixel = gradients[x + 1, y - 1];
                            break;
                        case 90:
                            leftPixel = gradients[x, y + 1];
                            rightPixel = gradients[x, y - 1];
                            break;
                        case 135:
                            leftPixel = gradients[x + 1, y + 1];
                            rightPixel = gradients[x - 1, y - 1];
                            break;
                    }
                    // compare current pixels value with adjacent pixels
                    if ((gradients[x, y] < leftPixel) || (gradients[x, y] < rightPixel))
                    {
                        //*dst = 0;
                        dst[x, y] = 0;
                    }
                    else
                    {
                        var value = (byte)(gradients[x, y] / maxGradient * 255);
                       // if (value != 0)
                        //    Console.WriteLine(":" + value);
                        //*dst = (byte)(gradients[x, y] / maxGradient * 255);
                        dst[x, y] = value;
                    }
                }
               // dst += dstOffset;
            }

            // STEP 4 - hysteresis
          //  dst = (byte*)destination.ImageData.ToPointer();
            // allign pointer
           // dst += dstStride * startY + startX;

            // for each line
            for (int y = startY; y < stopY; y++)
            {
                // for each pixel
                for (int x = startX; x < stopX; x++)
                {
                    if (dst[x,y] < highThreshold)
                    {
                        if (dst[x,y] < lowThreshold)
                        {
                            // non edge
                            dst[x,y] = 0;
                        }
                        else
                        {
                            var test = this.CannyHThresholdChecker(dst, x, y,(int)rect.Width,(int)rect.Height, lowThreshold, highThreshold);
                            // check 8 neighboring pixels
                            if (test)
                            {
                                dst[x,y] = 0;
                            }
                        }
                    }
                }
               // dst += dstOffset;
            }

            // STEP 5 - draw black rectangle to remove those pixels, which were not processed
            // (this needs to be done for those cases, when filter is applied "in place" -
            //  source image is modified instead of creating new copy)
          //  Drawing.Rectangle(destination, rect, Color.Black);

            // release blurred image
           // blurredImage.Dispose();
            return dst;
        }
        private bool CannyHThresholdChecker(byte[,] image,int x, int y,int width,int height, byte lowThreshold, byte highThreshold)
        {
            bool RV = false;
            var oneR = image[x - 1, y - 1];
            var twoR = image[x, y - 1];
            var threeR = image[x + 1, y - 1];
            var fourR = image[x - 1, y];
            var fiveR = image[x, y];
            var sixR = image[x + 1, y];
            var sevenR = image[x - 1, y + 1];
            var eightR = image[x, y + 1];
            var nineR = image[x + 1, y + 1];
          
            RV = oneR < highThreshold && twoR < highThreshold && threeR < highThreshold && fourR < highThreshold
                 && sixR < highThreshold && sevenR < highThreshold && eightR < highThreshold
                 && nineR < highThreshold;

            if (RV == true)
            {
                var secondTest = oneR > lowThreshold || twoR > lowThreshold || threeR > lowThreshold || fourR > lowThreshold
                 || sixR > lowThreshold || sevenR > lowThreshold || eightR > lowThreshold
                 || nineR > lowThreshold;

                if (secondTest && x > 2 && y > 2 && x < width - 2 && y < height - 2)
                {
                    int size = 5;
                    var locationP = (size - 1) / 2;

                    List<byte> LargeWindow = new List<byte>();
                    for (int j = -locationP; j < locationP + 1; j++)
                    {
                        for (int i = -locationP; i < locationP + 1; i++)
                        {
                            LargeWindow.Add(image[x + i, y + j]);
                        }
                    }

                    foreach (var item in LargeWindow)
                    {
                        if (item > highThreshold)
                        {
                            RV = false;
                            break;
                        }
                    }
                }
            }

            return RV;
        }

        private List<byte> ThresholdSupression(int x, int y,int thresholdLow, int thresholdHigh, ImageReader image)
        {
            var returnList = new List<byte>();


            var oneR = image.GetPixel(x - 1, y - 1, 0);
            var twoR = image.GetPixel(x, y - 1, 0);
            var threeR = image.GetPixel(x + 1, y - 1, 0);
            var fourR = image.GetPixel(x - 1, y, 0);
            var fiveR = image.GetPixel(x, y, 0);
            var sixR = image.GetPixel(x + 1, y, 0);
            var sevenR = image.GetPixel(x - 1, y + 1, 0);
            var eightR = image.GetPixel(x, y + 1, 0);
            var nineR = image.GetPixel(x + 1, y + 1, 0);

            var smallWindow = new List<byte> { oneR, twoR, threeR, fourR, sixR, sevenR, eightR, nineR };
            var pixelToTest = fiveR;

            int size = 5;
            var locationP = (size - 1) / 2;
            
            List<byte> LargeWindow = new List<byte>();
            for (int j = -locationP; j < locationP + 1; j++)
            {
                for (int i = -locationP; i < locationP + 1; i++)
                {
                    LargeWindow.Add(image.GetPixel(x + i, y + j, 0));
                }
            }

            /*
             * • If pixel (x, y) has gradient magnitude less than tlow discard the edge (write out black).
• If pixel (x, y) has gradient magnitude greater than thigh keep the edge (write out white).

             * 
             * */
            if (pixelToTest < thresholdLow)
            {
                returnList.Add(0);
                returnList.Add(0);
                returnList.Add(0);
            }
            else if (pixelToTest >= thresholdHigh)
            {
                returnList.Add(255);
                returnList.Add(255);
                returnList.Add(255);
            }
            else if (pixelToTest < thresholdHigh && pixelToTest >= thresholdLow)
            {
                returnList = this.WindowThresholdSuppresion(thresholdLow, thresholdHigh, smallWindow, LargeWindow);
            }
            else
            {
                throw new InvalidDataException("Shouldn't be here!");
            }

            return returnList;
        }

        private List<byte> WindowThresholdSuppresion(int thresholdLow,int thresholdHigh, List<byte> smallWindow, List<byte> largeWindow)
        {
            /*
             * • If pixel (x, y) has gradient magnitude between tlow and thigh and any of its neighbors in a 3 × 3 region around
it have gradient magnitudes greater than thigh, keep the edge (write out white).
• If none of pixel (x, y)’s neighbors have high gradient magnitudes but at least one falls between tlow and thigh,
search the 5 × 5 region to see if any of these pixels have a magnitude greater than thigh. If so, keep the edge
(write out white).
• Else, discard the edge (write out black).
             * */
            var returnList = new List<byte>();
            bool set = false;
            bool oneInRange = false;
            foreach (var item in smallWindow)
            {
                if (item > thresholdHigh)
                {
                    set = true;
                }
                if (item > thresholdLow && item < thresholdHigh)
                {
                    oneInRange = true;
                }
            }

            if (set == false && oneInRange)
            {
                foreach (var item in largeWindow)
                {
                    if (item > thresholdHigh)
                    {
                        set = true;
                        break;
                    }
                }
            }
            if (set)
            {
                returnList.Add(255);
                returnList.Add(255);
                returnList.Add(255);
            }
            else
            {
                returnList.Add(0);
                returnList.Add(0);
                returnList.Add(0);
            }
            return returnList;
        }

        private List<byte> CannyDetection(int x, int y, double[,] image, double[,] thetaImage, double maxGradient)
        {
            var returnList = new List<byte>();

            var oneR = image[x - 1, y - 1];
            var twoR = image[x, y - 1];
            var threeR = image[x + 1, y - 1];
            var fourR = image[x - 1, y];
            var fiveR = image[x, y];
            var sixR = image[x + 1, y];
            var sevenR = image[x - 1, y + 1];
            var eightR = image[x, y + 1];
            var nineR = image[x + 1, y + 1];

            var comparerR = thetaImage[x, y];

            //var oneG = image.GetPixel(x - 1, y - 1, 1);
            //var twoG = image.GetPixel(x, y - 1, 1);
            //var threeG = image.GetPixel(x + 1, y - 1, 1);
            //var fourG = image.GetPixel(x - 1, y, 1);
            //var fiveG = image.GetPixel(x, y, 1);
            //var sixG = image.GetPixel(x + 1, y, 1);
            //var sevenG = image.GetPixel(x - 1, y + 1, 1);
            //var eightG = image.GetPixel(x, y + 1, 1);
            //var nineG = image.GetPixel(x + 1, y + 1, 1);

            //var oneB = image.GetPixel(x - 1, y - 1, 2);
            //var twoB = image.GetPixel(x, y - 1, 2);
            //var threeB = image.GetPixel(x + 1, y - 1, 2);
            //var fourB = image.GetPixel(x - 1, y, 2);
            //var fiveB = image.GetPixel(x, y, 2);
            //var sixB = image.GetPixel(x + 1, y, 2);
            //var sevenB = image.GetPixel(x - 1, y + 1, 2);
            //var eightB = image.GetPixel(x, y + 1, 2);
            //var nineB = image.GetPixel(x + 1, y + 1, 2);

            var RArray = new List<double> { oneR, twoR, threeR, fourR, fiveR, sixR, sevenR, eightR, nineR };

           

                double RV = 0;
                bool set = false;
                if (comparerR == 0)
                {
                    var first = image[x + 1, y];
                    var second = image[x - 1, y];
                    if (image[x, y] < first || image[x, y] < second)
                    {
                        RV = image[x, y];
                    }
                }
                else if (comparerR == 90)//90
                {
                    var first = image[x, y + 1];
                    var second = image[x, y - 1];
                    if (image[x, y] < first || image[x, y] < second)
                    {
                        RV = image[x, y];
                    }
                }
                else if (comparerR == 45)//45
                {
                    var first = image[x + 1, y + 1];
                    var second = image[x - 1, y - 1];
                    if (image[x, y] < first || image[x, y] < second)
                    {
                        RV = image[x, y];
                    }
                }
                else if (comparerR == 135)//135
                {
                    var first = image[x + 1, y - 1];
                    var second = image[x - 1, y + 1];
                    if (image[x, y] < first || image[x, y] < second)
                    {
                        RV = image[x, y];
                    }
                }
                
                if (RV != 0)
                {
                    //Console.WriteLine((RV / maxGradient * 255));
                }
                returnList.Add((byte)(RV / maxGradient * 255));
                returnList.Add((byte)(RV / maxGradient * 255));
                returnList.Add((byte)(RV / maxGradient * 255));
            //}

//            So, three pixels in a 3 × 3 around pixel (x, y) are examined:
//• If ′(x, y) = 0◦, then the pixels (x + 1, y), (x, y), and (x − 1, y) are examined.
//• If ′(x, y) = 90◦, then the pixels (x, y + 1), (x, y), and (x, y − 1) are examined.
//• If ′(x, y) = 45◦, then the pixels (x + 1, y + 1), (x, y), and (x − 1, y − 1) are examined.
//• If ′(x, y) = 135◦, then the pixels (x + 1, y − 1), (x, y), and (x − 1, y + 1) are examined.
//If pixel (x, y) has the highest gradient magnitude of the three pixels examined, it is kept as an edge. If one of the
//other two pixels has a higher gradient magnitude, then pixel (x, y) is not on the “center” of the edge and should not
//be classified as an edge pixel.

            return returnList;
        }
        private List<double> GradiantDetection(int x, int y, ImageReader image)
        {
            var returnList = new List<double>();

            var oneR = image.GetPixel(x - 1, y - 1, 0);
            var twoR = image.GetPixel(x, y - 1, 0);
            var threeR = image.GetPixel(x + 1, y - 1, 0);
            var fourR = image.GetPixel(x - 1, y, 0);
            var fiveR = image.GetPixel(x, y, 0);
            var sixR = image.GetPixel(x + 1, y, 0);
            var sevenR = image.GetPixel(x - 1, y + 1, 0);
            var eightR = image.GetPixel(x, y + 1, 0);
            var nineR = image.GetPixel(x + 1, y + 1, 0);

            var oneG = image.GetPixel(x - 1, y - 1, 1);
            var twoG = image.GetPixel(x, y - 1, 1);
            var threeG = image.GetPixel(x + 1, y - 1, 1);
            var fourG = image.GetPixel(x - 1, y, 1);
            var fiveG = image.GetPixel(x, y, 1);
            var sixG = image.GetPixel(x + 1, y, 1);
            var sevenG = image.GetPixel(x - 1, y + 1, 1);
            var eightG = image.GetPixel(x, y + 1, 1);
            var nineG = image.GetPixel(x + 1, y + 1, 1);

            var oneB = image.GetPixel(x - 1, y - 1, 2);
            var twoB = image.GetPixel(x, y - 1, 2);
            var threeB = image.GetPixel(x + 1, y - 1, 2);
            var fourB = image.GetPixel(x - 1, y, 2);
            var fiveB = image.GetPixel(x, y, 2);
            var sixB = image.GetPixel(x + 1, y, 2);
            var sevenB = image.GetPixel(x - 1, y + 1, 2);
            var eightB = image.GetPixel(x, y + 1, 2);
            var nineB = image.GetPixel(x + 1, y + 1, 2);
            
            var GX = new List<int> { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            var GY = new List<int> { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
            var RArray = new List<byte> { oneR, twoR, threeR, fourR, fiveR, sixR, sevenR, eightR, nineR };
            var GArray = new List<byte> { oneG, twoG, threeG, fourG, fiveG, sixG, sevenG, eightG, nineG };
            var BArray = new List<byte> { oneB, twoB, threeB, fourB, fiveB, sixB, sevenB, eightB, nineB };

            int GXR = 0;
            int GXG = 0;
            int GXB = 0;
            int GYR = 0;
            int GYG = 0;
            int GYB = 0;

            for (int i = 0; i < 9; i++)
            {
                GXR += GX[i] * RArray[i];
                GXG += GX[i] * GArray[i];
                GXB += GX[i] * BArray[i];

                GYR += GY[i] * RArray[i];
                GYG += GY[i] * GArray[i];
                GYB += GY[i] * BArray[i];
            }

            //TODO: this estimation is wrong i think!
            //var newR = Math.Abs(GXR) + Math.Abs(GYR);
            //var newG = Math.Abs(GXG) + Math.Abs(GYG);
            //var newB = Math.Abs(GXB) + Math.Abs(GYB);

            //try these
            var newR = Math.Sqrt(GXR * GXR + GYR * GYR);
            var newG = Math.Sqrt(GXG * GXG + GYG * GYG);
            var newB = Math.Sqrt(GXB * GXB + GYB * GYB);
            double orientation, toAngle = 180.0 / System.Math.PI;

            // --- get orientation
            if (GXR == 0)
            {
                // can not divide by zero
                orientation = (GYR == 0) ? 0 : 90;
            }
            else
            {
                double div = (double)GYR / GXR;

                // handle angles of the 2nd and 4th quads
                if (div < 0)
                {
                    orientation = 180 - System.Math.Atan(-div) * toAngle;
                }
                // handle angles of the 1st and 3rd quads
                else
                {
                    orientation = System.Math.Atan(div) * toAngle;
                }

                // get closest angle from 0, 45, 90, 135 set
                if (orientation < 22.5)
                    orientation = 0;
                else if (orientation < 67.5)
                    orientation = 45;
                else if (orientation < 112.5)
                    orientation = 90;
                else if (orientation < 157.5)
                    orientation = 135;
                else orientation = 0;
            }

            // save orientation
           // orients[p] = (byte)orientation;
               

            //if (newR > 255) newR = 255;
            //if (newG > 255) newG = 255;
            //if (newB > 255) newB = 255;
            var r = newR;
            var g = newG;
            var b = newB;
            returnList.Add(r);
            //returnList.Add(g);
            //returnList.Add(b);
            returnList.Add(orientation);

           // returnList.AddRange(convertedThetas);


            return returnList;
        }

        private List<byte> SobelDetection(int x, int y, ImageReader image)
        {
            var returnList = new List<byte>();

            var oneR = image.GetPixel(x - 1, y - 1, 0);
            var twoR = image.GetPixel(x, y - 1, 0);
            var threeR = image.GetPixel(x + 1, y - 1, 0);
            var fourR = image.GetPixel(x - 1, y, 0);
            var fiveR = image.GetPixel(x, y, 0);
            var sixR = image.GetPixel(x + 1, y, 0);
            var sevenR = image.GetPixel(x - 1, y + 1, 0);
            var eightR = image.GetPixel(x, y + 1, 0);
            var nineR = image.GetPixel(x + 1, y + 1, 0);

            var oneG = image.GetPixel(x - 1, y - 1, 1);
            var twoG = image.GetPixel(x, y - 1, 1);
            var threeG = image.GetPixel(x + 1, y - 1, 1);
            var fourG = image.GetPixel(x - 1, y, 1);
            var fiveG = image.GetPixel(x, y, 1);
            var sixG = image.GetPixel(x + 1, y, 1);
            var sevenG = image.GetPixel(x - 1, y + 1, 1);
            var eightG = image.GetPixel(x, y + 1, 1);
            var nineG = image.GetPixel(x + 1, y + 1, 1);

            var oneB = image.GetPixel(x - 1, y - 1, 2);
            var twoB = image.GetPixel(x, y - 1, 2);
            var threeB = image.GetPixel(x + 1, y - 1, 2);
            var fourB = image.GetPixel(x - 1, y, 2);
            var fiveB = image.GetPixel(x, y, 2);
            var sixB = image.GetPixel(x + 1, y, 2);
            var sevenB = image.GetPixel(x - 1, y + 1, 2);
            var eightB = image.GetPixel(x, y + 1, 2);
            var nineB = image.GetPixel(x + 1, y + 1, 2);

            // GX
            // -1 0 1
            // -2 0 2
            // -1 0 1

            //GY
            // 1 2 1
            // 0 0 0
            // -1 -2 -1
            //abs(gx)+abs(gy)
            //clamp to 255 or 0 just in case?
            var GX = new List<int> { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            var GY = new List<int> { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
            var RArray = new List<byte> { oneR, twoR, threeR, fourR, fiveR, sixR, sevenR, eightR, nineR };
            var GArray = new List<byte> { oneG, twoG, threeG, fourG, fiveG, sixG, sevenG, eightG, nineG };
            var BArray = new List<byte> { oneB, twoB, threeB, fourB, fiveB, sixB, sevenB, eightB, nineB };

            int GXR = 0;
            int GXG = 0;
            int GXB = 0;
            int GYR = 0;
            int GYG = 0;
            int GYB = 0;

            for (int i = 0; i < 9; i++)
            {
                GXR += GX[i] * RArray[i];
                GXG += GX[i] * GArray[i];
                GXB += GX[i] * BArray[i];

                GYR += GY[i] * RArray[i];
                GYG += GY[i] * GArray[i];
                GYB += GY[i] * BArray[i];
            }

            //TODO: this estimation is wrong i think!
            //var newR = Math.Abs(GXR) + Math.Abs(GYR);
            //var newG = Math.Abs(GXG) + Math.Abs(GYG);
            //var newB = Math.Abs(GXB) + Math.Abs(GYB);

            //try these
            var newR = Math.Sqrt(GXR * GXR + GYR * GYR);
            var newG = Math.Sqrt(GXG * GXG + GYG * GYG);
            var newB = Math.Sqrt(GXB * GXB + GYB * GYB);
            double orientation, toAngle = 180.0 / System.Math.PI;
            // --- get orientation
            if (GXR == 0)
            {
                // can not divide by zero
                orientation = (GYR == 0) ? 0 : 90;
            }
            else
            {
                double div = (double)GYR / GXR;

                // handle angles of the 2nd and 4th quads
                if (div < 0)
                {
                    orientation = 180 - System.Math.Atan(-div) * toAngle;
                }
                // handle angles of the 1st and 3rd quads
                else
                {
                    orientation = System.Math.Atan(div) * toAngle;
                }

                // get closest angle from 0, 45, 90, 135 set
                if (orientation < 22.5)
                    orientation = 0;
                else if (orientation < 67.5)
                    orientation = 45;
                else if (orientation < 112.5)
                    orientation = 90;
                else if (orientation < 157.5)
                    orientation = 135;
                else orientation = 0;
            }

            // save orientation
            var convertedThetas = new List<byte>();
            convertedThetas.Add((byte)orientation);
            convertedThetas.Add((byte)orientation);
            convertedThetas.Add((byte)orientation);

            if (newR > 255) newR = 255;
            if (newG > 255) newG = 255;
            if (newB > 255) newB = 255;
            var r = newR;
            var g = newG;
            var b = newB;
            returnList.Add((byte)r);
            returnList.Add((byte)g);
            returnList.Add((byte)b);
            
            returnList.AddRange(convertedThetas);


            return returnList;
        }

        public void KrischCommandExecute()
        {
            //Bitmap ret = new Bitmap(image.Width, image.Height);
            //for (int i = 1; i < image.Width - 1; i++)
            //{
            //    for (int j = 1; j < image.Height - 1; j++)
            //    {
            //       Color cr = image.GetPixel(i + 1, j);
            //       Color cl = image.GetPixel(i - 1, j);
            //       Color cu = image.GetPixel(i, j - 1);
            //       Color cd = image.GetPixel(i, j + 1);
            //       Color cld = image.GetPixel(i - 1, j + 1);
            //       Color clu = image.GetPixel(i - 1, j - 1);
            //       Color crd = image.GetPixel(i + 1, j + 1);
            //       Color cru = image.GetPixel(i + 1, j - 1);
            //       int power = getMaxD(cr.R, cl.R, cu.R, cd.R, cld.R, clu.R, cru.R, crd.R);
            //        if (power > 50)
            //          ret.SetPixel(i, j, Color.Yellow);
            //        else
            //           ret.SetPixel(i, j, Color.Black);
            //       }
            //   }
            //   return ret;

            var image = new ImageReader(LeftPictureLocation);
            image.ConvertToGrey();
            const int StartBorder = 0;
            const int EndBorder = 1; //border of one for 3x3 mean block
            var resultImage = new ImageReader();
            resultImage.Count = image.Count;
            resultImage.Height = image.Height;
            resultImage.Width = image.Width;
            resultImage.depth = image.depth;
            resultImage.Pixels = new List<byte>(image.Pixels);



            //Gauss the image first
            for (var h = 0; h < image.Height; h++)
            {
                for (var w = 0; w < image.Width; w++)
                {
                    if (h >= 2 && h < image.Height - 2)
                    {
                        if (w >= 2 && w < image.Width - 2)
                        {
                            var returnList = this.FindGaussian(w, h, 5, 1.4, image);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            var GaussImage = new ImageReader();
            GaussImage.Count = resultImage.Count;
            GaussImage.Height = resultImage.Height;
            GaussImage.Width = resultImage.Width;
            GaussImage.depth = resultImage.depth;
            GaussImage.Pixels = new List<byte>(resultImage.Pixels);


            for (var h = 0; h < GaussImage.Height; h++)
            {
                for (var w = 0; w < GaussImage.Width; w++)
                {
                    if (h > StartBorder && h < GaussImage.Height - EndBorder)
                    {
                        if (w > StartBorder && w < GaussImage.Width - EndBorder)
                        {
                            //can average here
                            var returnList = this.Krisch(w, h, GaussImage);
                            resultImage.SetPixel(w, h, 0, returnList[0]);
                            resultImage.SetPixel(w, h, 1, returnList[1]);
                            resultImage.SetPixel(w, h, 2, returnList[2]);
                        }
                    }
                }
            }

            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "Krisch" + fi.Name;
            resultImage.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }

        private ICommand _krischCommand;
public ICommand KrischCommand
{
    get
    {
        return _krischCommand ??
               (_krischCommand =
                new RelayCommand(param => KrischCommandExecute(), param => KrischCommandEnabled));
    }
}

private bool _krischCommandEnabled = true;
public bool KrischCommandEnabled
{
    get { return _krischCommandEnabled; }
    set { _krischCommandEnabled = value; }
}

private List<byte> Krisch(int x, int y, ImageReader image)
{
    var returnList = new List<byte>();
    var cr = image.GetPixel(x + 1, y, 0);
    var cl = image.GetPixel(x - 1, y, 0);
    var cu = image.GetPixel(x, y - 1, 0);
    var cd = image.GetPixel(x, y + 1, 0);
    var cld = image.GetPixel(x - 1, y + 1, 0);
    var clu = image.GetPixel(x - 1, y - 1, 0);
    var crd = image.GetPixel(x + 1, y + 1, 0);
    var cru = image.GetPixel(x + 1, y - 1, 0);
    int power = getMaxD(cr, cl, cu, cd, cld, clu, cru, crd);
    if (power > 100)
    {
        //ret.SetPixel(i, j, Color.Yellow);
        returnList.Add(0);
        returnList.Add(0);
        returnList.Add(255);
    }
    else
    {
        //ret.SetPixel(i, j, Color.Black);
        returnList.Add(255);
        returnList.Add(255);
        returnList.Add(255);
    }
    return returnList;
}

private int getD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd, int[,] matrix)
{
   return Math.Abs(  matrix[0, 0]*clu + matrix[0, 1]*cu + matrix[0, 2]*cru
      + matrix[1, 0]*cl + matrix[1, 2]*cr
         + matrix[2, 0]*cld + matrix[2, 1]*cd + matrix[2, 2]*crd);
}
private int getMaxD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd)
{
   int max = int.MinValue;
   for (int i = 0; i < templates.Count; i++)
   {
      int newVal = getD(cr, cl, cu, cd, cld, clu, cru, crd, templates[i]);
      if (newVal > max)
         max = newVal;
    }
    return max;
}
private List<int[,]> templates = new List<int[,]> 
{
   new int[,] {{ -3, -3, 5 }, { -3, 0, 5 }, { -3, -3, 5 } },
   new int[,] {{ -3, 5, 5 }, { -3, 0, 5 }, { -3, -3, -3 } },
   new int[,] {{ 5, 5, 5 }, { -3, 0, -3 }, { -3, -3, -3 } },
   new int[,] {{ 5, 5, -3 }, { 5, 0, -3 }, { -3, -3, -3 } },
   new int[,] {{ 5, -3, -3 }, { 5, 0, -3 }, { 5, -3, -3 } },
   new int[,] {{ -3, -3, -3 }, { 5, 0, -3 }, { 5, 5, -3 } },
   new int[,] {{ -3, -3, -3 }, { -3, 0, -3 }, { 5, 5, 5 } },
   new int[,] {{ -3, -3, -3 }, { -3, 0, 5 }, { -3, 5, 5 } }
};

        private ICommand _histogramCommand;
        public ICommand EqualizeHistogramCommand
        {
            get
            {
                return _histogramCommand ??
                       (_histogramCommand =
                        new RelayCommand(param => EqualizeHistogramCommandExecute(), param => EqualizeHistogramCommandEnabled));
            }
        }

        private bool _equalizeHistogramCommandEnabled = true;
        public bool EqualizeHistogramCommandEnabled
        {
            get { return _equalizeHistogramCommandEnabled; }
            set { _equalizeHistogramCommandEnabled = value; }
        }
        private void EqualizeHistogramCommandExecute()
        {
            var image = new ImageReader(LeftPictureLocation);

            var L = 256 - 1;
            var MN = image.Width * image.Height;

            List<int> cdf = new List<int>();
            cdf.Add(image.luminance[0]);
            int min = int.MaxValue;
            int max = int.MinValue;
            for (int i = 1; i < image.luminance.Length; i++)
            {
                var tempValue = image.luminance[i] + cdf[i - 1];
                if (tempValue > 0 && tempValue < min)
                {
                    min = tempValue;
                }

                if (tempValue > max)
                {
                    max = tempValue;
                }

                cdf.Add(tempValue);
            }

            List<byte> newImage = new List<byte>();
            foreach (var item in image.Pixels)
            {
                double eq = (((double)(cdf[item] - min) / (double)(MN - min)) * (double)L);
                newImage.Add((byte)Math.Round(eq, MidpointRounding.ToEven));
            }
            var newImageFile = new ImageReader();
            newImageFile.Width = image.Width;
            newImageFile.Height = image.Height;
            newImageFile.Pixels = newImage;
            var fi = new System.IO.FileInfo(LeftPictureLocation);
            var modifiedName = fi.Directory.FullName + System.IO.Path.DirectorySeparatorChar + "Equalized" + fi.Name;
            newImageFile.SaveAsBitmap(modifiedName);
            LoadRightImage(modifiedName);
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand = new RelayCommand(param => ExitCommandExecute(), param => ExitCommandEnabled));
            }
        }
         private bool _exitCommandEnabled = true;
        public bool ExitCommandEnabled
        {
            get { return _exitCommandEnabled; }
            set { _exitCommandEnabled = value; }
        }

        private void ExitCommandExecute()
        {
            Application.Current.Shutdown(0);
        }


        // Load an image, feature set, or database.
        public void load_query_image(string queryImageName)
        {
            _queryImage = queryImageName;
        }
        public void load_result_image(string resultImageName)
        {
            _resultImage = resultImageName;
        }
        public void load_query_features(string fileName, bool sift)
        {
            if (string.IsNullOrEmpty(_queryImage))
            {
                MessageBox.Show("No query image loaded");
            }
            else
            {
                QueryFeatures = null;

                // Delete the current result image.                
                _resultImage = string.Empty;

                QueryFeatures = new FeatureSet();

                // Load the feature set.
                if (((!sift) && (QueryFeatures.load(fileName))) || ((sift) && (QueryFeatures.load_sift(fileName))))
                {
                  //might have to link the feature set to the Image that will be used for actual processing
                }
                else
                {
                    QueryFeatures = null;
                    MessageBox.Show("Couldn't load feature data file");
                }
            }
        }
        private void LoadSiftDatabase()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".kdb";
            dlg.Filter = "Database Files (*.kdb)|*.kdb";
            if (string.IsNullOrEmpty(_dirLocation))
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
            }

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _dirLocation = dlg.InitialDirectory;
                load_image_database(filename, true);
            }
        }
        private void LoadDataBase()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".db";
            dlg.Filter = "Database Files (*.db)|*.db";
            if (string.IsNullOrEmpty(_dirLocation))
            {
                dlg.InitialDirectory = Environment.CurrentDirectory;
            }

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _dirLocation = dlg.InitialDirectory;
                load_image_database(filename, false);
            }
        }
        public void load_image_database(string fileName, bool sift)
        {
            //ui->set_images(queryImage, NULL);
            //ui->set_features(queryFeatures, NULL);

            //// Delete the current database.
            //if (db != NULL)
            //{
            //    delete db;
            db = null;
            //}

            //// Delete the current result image.
            //if (resultImage != NULL)
            //{
            //    resultImage->release();
            //    resultImage = NULL;
            //}
            _resultImage = string.Empty;

            db = new ImageDatabase();

            //// Load the database.
            if (!db.load(fileName, sift))
            {
                db = null;
                MessageBox.Show("Couldn't load database");
            }

            //ui->refresh();
        }

        // Perform a query on the currently loaded image and database.
        public void perform_query()
        {
            //        ui->set_images(queryImage, NULL);
            //ui->set_features(queryFeatures, NULL);

            //if (queryImage == NULL) {
            //    fl_alert("no query image loaded");
            //}
            //else if (queryFeatures == NULL) {
            //    fl_alert("no query features loaded");
            //}
            //else if (db == NULL) {
            //    fl_alert("no image database loaded");
            //}
            //else {
            //    FeatureSet selectedFeatures;
            //    queryFeatures->get_selected_features(selectedFeatures);

            //    if (selectedFeatures.size() == 0) {
            //        fl_alert("no features selected");
            //    }
            //    else {
            //        int index;
            //        vector<FeatureMatch> matches;
            //        double score;

            //        if (!performQuery(selectedFeatures, *db, index, matches, score, ui->get_match_type())) {
            //            fl_alert("query failed");
            //        }
            //        else {
            //            // Delete the current result image.
            //            if (resultImage != NULL) {
            //                resultImage->release();
            //                resultImage = NULL;
            //            }

            //            // Load the image.
            //            resultImage = Fl_Shared_Image::get((*db)[index].name.c_str());

            //            if (resultImage == NULL) {
            //                fl_alert("couldn't load result image file");
            //            }
            //            else {
            //                (*db)[index].features.deselect_all();
            //                (*queryFeatures).deselect_all();

            //                // Select the matched features.
            //                for (unsigned int i=0; i<matches.size(); i++) {
            //                    (*queryFeatures)[matches[i].id1-1].selected = true;
            //                    (*db)[index].features[matches[i].id2-1].selected = true;
            //                }

            //                // Update the UI.
            //                if (queryImage->h() > resultImage->h()) {
            //                    ui->resize_windows(queryImage->w(), resultImage->w(), queryImage->h());
            //                }
            //                else {
            //                    ui->resize_windows(queryImage->w(), resultImage->w(), resultImage->h());
            //                }

            //                ui->set_images(queryImage, resultImage);
            //                ui->set_features(queryFeatures, &((*db)[index].features));
            //            }
            //        }
            //    }
            //}

            //ui->refresh();
        }

       
        // Select or deselect all query features.
        public void select_all_query_features()
        {
            if (QueryFeatures == null)
            {
                MessageBox.Show("No query features loaded");
            }
            else
            {
                QueryFeatures.select_all();
            }
        }
        public void deselect_all_query_features()
        {
            if (QueryFeatures == null)
            {
               MessageBox.Show("No query features loaded");
            }
            else
            {
                QueryFeatures.deselect_all();                
            }
        }

        // Set the match algorithm.
        public int get_match_algorithm() { return matchType; }
        public void set_match_algorithm(int type)
        {
            matchType = type;
        }
   

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        private PointCollection ConvertToPointCollection(int[] values)
        {
            //if (this.PerformHistogramSmoothing)
            //{
            //    values = SmoothHistogram(values);
            //}

            int max = values.Max();

            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(values.Length - 1, max));

            return points;
        }

       

    }
   
}
