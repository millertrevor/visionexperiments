using System;
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

namespace DetectionAndMatching.UI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private FeaturesDoc _doc;
        private string _dirLocation;

        public MainWindowViewModel(FeaturesDoc incomingDoc)
        {
            _doc = incomingDoc;

          //  LeftPictureLocation = @"C:\Users\Trevor\Downloads\angrytree.jpg";
            //_doc.load_query_image(LeftPictureLocation);
           // var bi = new BitmapImage(new Uri(LeftPictureLocation,UriKind.RelativeOrAbsolute));
           // LeftImageHeight = bi.PixelHeight;
           // LeftImageWidth = bi.PixelWidth;

         
            LeftCollection = new ObservableCollection<IItemViewModel>();
            //ItemViewModel m1 = new ItemViewModel();
            //m1.Top = 1;
            //m1.Left = 1;
            //m1.Height = 10;
            //m1.Width = 10;
            //m1.Color = Brushes.Black;

            //ItemViewModel m2 = new ItemViewModel();
            //m2.Top = 50;
            //m2.Left = 50;
            //m2.Height = 30;
            //m2.Width = 30;
            //m2.Color = Brushes.Red;

            //LeftCollection.Add(m1);
            //LeftCollection.Add(m2);
        }

        public ObservableCollection<IItemViewModel> LeftCollection { get; private set; }
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

        //private int _width = 500;
        //private int _height = 500;

        //public int Width
        //{
        //    get { return _width; }
        //    set
        //    {
        //        _width = value;
        //        NotifyPropertyChanged("Width");
        //    }
        //}

        //public int Height
        //{
        //    get { return _height; }
        //    set
        //    {
        //        _height = value;
        //        NotifyPropertyChanged("Height");
        //    }
        //}

        private int _leftImageWidth;
        private int _leftImageHeight;

        public int LeftImageWidth
        {
            get { return _leftImageWidth; }
            set
            {
                _leftImageWidth = value;
                //Width = _leftImageWidth;
                NotifyPropertyChanged("LeftImageWidth");
            }
        }

        public int LeftImageHeight
        {
            get { return _leftImageHeight; }
            set
            {
                _leftImageHeight = value;
                //Height = _leftImageHeight;
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
                //Width = _rightImageWidth;
                NotifyPropertyChanged("RightImageWidth");
            }
        }

        public int RightImageHeight
        {
            get { return _rightImageHeight; }
            set
            {
                _rightImageHeight = value;
                //Height = _rightImageHeight;
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

        private void LoadQueryImageExecute()
        {
            //char* name = fl_file_chooser("Open File", "{*.p[gp]m,*.jpg}", NULL);

            //if (name != NULL)
            //{
            //    doc->load_query_image(name);
            //}
            // Create OpenFileDialog 
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
                _dirLocation = dlg.InitialDirectory;
               // _doc.load_query_image(filename);
                LeftPictureLocation = filename;
                _doc.load_query_image(LeftPictureLocation);
                var bi = new BitmapImage(new Uri(LeftPictureLocation, UriKind.RelativeOrAbsolute));
                LeftImageHeight = bi.PixelHeight;
                LeftImageWidth = bi.PixelWidth;
            }
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
           // char* name = fl_file_chooser("Open File", "*.key", NULL);

           // if (name != NULL)
           // {
           //     doc->load_query_features(name, true);
           // }

            // Create OpenFileDialog 
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
                // Open document 
                string filename = dlg.FileName;
                _dirLocation = dlg.InitialDirectory;
                _doc.load_query_features(filename, true);
                LeftCollection.Clear();
                foreach (var item in _doc.queryFeatures)
                {
                    
                    item.draw();
                    LeftCollection.Add(item);
                }
               
               // LeftCollection.a
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    //public class ItemViewModel
    //    : IItemViewModel
    //{
    //    public double Left { get; set; }
    //    public double Top { get; set; }
    //    public double Width { get; set; }
    //    public double Height { get; set; }
    //    public SolidColorBrush Color { get; set; }

    //    // whatever you need...
    //}

    public interface IItemViewModel
    {
         double Left { get; set; }
         double Top { get; set; }
         double Width { get; set; }
         double Height { get; set; }
         SolidColorBrush Color { get; set; }
        int X1 { get; set; }
        int Y1 { get; set; }
        int X2 { get; set; }
        int Y2 { get; set; }
        int X3 { get; set; }
        int Y3 { get; set; }
        int X4 { get; set; }
        int Y4 { get; set; }
        int X5 { get; set; }
        int Y5 { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }

    public class CollectionViewModel
    {
       

        // some code which fills the Collection with items
    }
}
