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
            
            LeftCollection = new ObservableCollection<IFeature>();           
        }
        public void SelectAllFeaturesAt(double X, double Y)
        {
            if (_doc.queryFeatures != null)
            {
                _doc.queryFeatures.select_point(X, Y);
            }
        }
        public void SelectAllFeaturesInArea(double xMin, double xMax, double yMin, double yMax)
        {
            if (_doc.queryFeatures != null)
            {
                _doc.queryFeatures.select_box(xMin, xMax, yMin, yMax);
            }
        }

        public ObservableCollection<IFeature> LeftCollection { get; private set; }
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

        private void LoadQueryImageExecute()
        {
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
   
}
