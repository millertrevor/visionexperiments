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
        AdornerLayer myAdornerLayer;
        Adorner ad;
        Canvas canvasToTouch;

        public MainWindow()
        {
            InitializeComponent();
            var dc = new MainWindowViewModel();
            dc.PropertyChanged += dc_PropertyChanged;
            this.DataContext = dc;
        }

        void dc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LeftPictureLocation")
            {
                var doesThisWork = VisualTreeHelper.GetChild(leftItemsControl, 0);
                var doesThisWorkt = VisualTreeHelper.GetChild(doesThisWork, 0);
                var doesThisWorktt = VisualTreeHelper.GetChild(doesThisWorkt, 0);
                canvasToTouch = (Canvas)doesThisWorktt;
                myAdornerLayer = AdornerLayer.GetAdornerLayer((Visual)doesThisWorktt);
                ad = new SimpleCircleAdorner((UIElement)doesThisWorktt);
                myAdornerLayer.Add(ad);
                myAdornerLayer.IsHitTestVisible = false;
            }
        }

        private void Histogram_OnClick(object sender, RoutedEventArgs e)
        {
            var hw = new HistogramWindow();
            hw.DataContext = (DataContext as MainWindowViewModel).HistogramWindowViewModel;
            hw.Show();
        }

        private void itemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point mousePos = e.GetPosition(canvasToTouch);
                ADWidth = mousePos.X - mouseDownPos.X;
                ADHeight = mousePos.Y - mouseDownPos.Y;
                this.InvalidateVisual();
                this.InvalidateArrange();
                this.UpdateLayout();
                // remove and add adorner to reset
                myAdornerLayer.Remove(ad);
                myAdornerLayer.Add(ad);
            }
        }
        bool mouseDown = false; // Set to 'true' when mouse is held down.
        Point mouseDownPos; // The point where the mouse button was clicked down.

        private void RRMouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPos = e.GetPosition(canvasToTouch); 
            ADLeft = mouseDownPos.X;
            ADTop = mouseDownPos.Y;
            ADWidth = 0;
            ADHeight = 0;
            VisibleAD = true;
            mouseDown = true;
        }
        private void RRMouseUp(object sender, MouseEventArgs e)
        {
            //// Release the mouse capture and stop tracking it.
            mouseDown = false;
            leftViewBox.ReleaseMouseCapture();

            double XToUse = MainWindow.ADLeft;
            double YToUse = MainWindow.ADTop;
            double WidthToUse = MainWindow.ADWidth;
            double HeightToUse = MainWindow.ADHeight;
            if (MainWindow.ADWidth < 0)
            {
                XToUse = XToUse + WidthToUse;
                WidthToUse = Math.Abs(WidthToUse);
            }
            if (MainWindow.ADHeight < 0)
            {
                YToUse = YToUse + HeightToUse;
                HeightToUse = Math.Abs(HeightToUse);
            }

            var dc = DataContext as MainWindowViewModel;
            dc.SelectAllFeaturesInArea(XToUse, XToUse + WidthToUse, YToUse, YToUse + HeightToUse);
            VisibleAD = false;
            myAdornerLayer.Remove(ad);
            myAdornerLayer.Add(ad);
        }

    
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pt = e.GetPosition((UIElement)sender);
                var dc = DataContext as MainWindowViewModel;
                dc.SelectAllFeaturesAt(pt.X, pt.Y);
            }
        }
       public static bool VisibleAD = false;
       public static double ADTop = 0;
       public static double ADLeft = 0;
       public static double ADWidth = 0;
       public static double ADHeight = 0;
        // Adorners must subclass the abstract base class Adorner.
        public class SimpleCircleAdorner : Adorner
        {
            // Be sure to call the base class constructor.
            public SimpleCircleAdorner(UIElement adornedElement)
                : base(adornedElement)
            {
            }
            
            protected override void OnRender(DrawingContext drawingContext)
            {
                if (MainWindow.VisibleAD)
                {
                    Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

                    // Some arbitrary drawing implements.
                    SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
                    renderBrush.Opacity = 0.2;
                    Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
                    double renderRadius = 5.0;

                    // Draw a circle at each corner.
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
                    drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
                    double XToUse = MainWindow.ADLeft;
                    double YToUse = MainWindow.ADTop;
                    double WidthToUse = MainWindow.ADWidth;
                    double HeightToUse = MainWindow.ADHeight;
                    if (MainWindow.ADWidth < 0)
                    {
                        XToUse = XToUse + WidthToUse;
                        WidthToUse = Math.Abs(WidthToUse);
                    }
                    if (MainWindow.ADHeight < 0)
                    {
                        YToUse = YToUse + HeightToUse;
                        HeightToUse = Math.Abs(HeightToUse);
                    }

                    drawingContext.DrawRectangle(renderBrush, renderPen, new Rect(XToUse, YToUse, WidthToUse, HeightToUse));
                }
            }
        }
        
        private void leftItemsControl_MouseLeave_1(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.RRMouseUp(sender, e);
            }
        }

       
    }

   
}
