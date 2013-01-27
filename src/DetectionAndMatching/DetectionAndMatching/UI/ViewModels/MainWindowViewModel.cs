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
using PixelMap;

namespace DetectionAndMatching.UI.ViewModels
{
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
                System.IO.FileInfo fi = new System.IO.FileInfo(filename);
                if (fi.Extension == ".ppm")
                {
                    filename = ConvertToJpeg(fi);
                }
                _dirLocation = dlg.InitialDirectory;
               // _doc.load_query_image(filename);
                LeftPictureLocation = filename;
                load_query_image(LeftPictureLocation);
                var bi = new BitmapImage(new Uri(LeftPictureLocation, UriKind.RelativeOrAbsolute));
                LeftImageHeight = bi.PixelHeight;
                LeftImageWidth = bi.PixelWidth;                
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
    }
   
}
