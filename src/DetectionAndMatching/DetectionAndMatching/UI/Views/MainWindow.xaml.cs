using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DetectionAndMatching.UI.Models;
using DetectionAndMatching.UI.ViewModels;

namespace DetectionAndMatching.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var doc = new FeaturesDoc();
            this.DataContext = new MainWindowViewModel(doc);
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var viewBox = this.leftViewBox;
            var itemsControl = this.leftItemsControl;
            int stop = -1;
        }
    }
}
