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
            var doc = new FeaturesDoc();
            this.DataContext = new MainWindowViewModel(doc, this);
           
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //var itemToAdorn = (Canvas)this.Template.FindName("leftCanvas", this);
            //myAdornerLayer = AdornerLayer.GetAdornerLayer(itemToAdorn);
            //ad = new SimpleCircleAdorner(itemToAdorn);
            //myAdornerLayer.Add(ad);

            ////foreach (UIElement toAdorn in myStackPanel.Children)
            ////    myAdornerLayer.Add(new SimpleCircleAdorner(toAdorn));
        }
        private void leftItemsControl_MouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            //var st = (ScaleTransform)((TransformGroup)leftItemsControl.RenderTransform).Children.First(stTransform => stTransform is ScaleTransform);
            //double zoom = e.Delta > 0 ? .2 : -.2;
            
            //st.ScaleX += zoom;
            //st.ScaleY += zoom;
        }
        Point start;
        Point origin;
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //leftItemsControl.CaptureMouse();
            //var tt = (TranslateTransform)((TransformGroup)leftItemsControl.RenderTransform)
            //    .Children.First(tr => tr is TranslateTransform);
            //start = e.GetPosition(leftViewBox);
            //origin = new Point(tt.X, tt.Y);
        }
        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //leftItemsControl.ReleaseMouseCapture();
        }
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            //if (leftItemsControl.IsMouseCaptured)
            //{
            //    var tt = (TranslateTransform)((TransformGroup)leftItemsControl.RenderTransform)
            //        .Children.First(tr => tr is TranslateTransform);
            //    Vector v = start - e.GetPosition(leftViewBox);
            //    tt.X = origin.X - v.X;
            //    tt.Y = origin.Y - v.Y;
            //}

            if (mouseDown)
            {
            //    // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(canvasToTouch);//leftviewbox

                //if (mouseDownPos.X < mousePos.X)
                //{
                //    //Canvas.SetLeft(selectionBox, mouseDownPos.X);
                //    //selectionBox.Width = mousePos.X - mouseDownPos.X;
                //    ADWidth = mousePos.X - mouseDownPos.X;
                //}
                //else
                //{
                //    //Canvas.SetLeft(selectionBox, mousePos.X);
                //    //selectionBox.Width = mouseDownPos.X - mousePos.X;
                //    ADWidth = mouseDownPos.X - mousePos.X;
                //}

                //if (mouseDownPos.Y < mousePos.Y)
                //{
                //    //Canvas.SetTop(selectionBox, mouseDownPos.Y);
                //    //selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                //    ADHeight = mousePos.Y - mouseDownPos.Y;
                //}
                //else
                //{
                //    //Canvas.SetTop(selectionBox, mousePos.Y);
                //    //selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                //    ADHeight = mouseDownPos.Y - mousePos.Y;
                //}
                //ADWidth = Math.Abs(mousePos.X - mouseDownPos.X);
                //ADHeight = Math.Abs(mousePos.Y - mouseDownPos.Y);
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
            mouseDownPos = e.GetPosition(canvasToTouch); //leftviewbox
            ADLeft = mouseDownPos.X;
            ADTop = mouseDownPos.Y;
            ADWidth = 0;
            ADHeight = 0;
            VisibleAD = true;
           // this.InvalidateVisual();
           // this.InvalidateArrange();
          //  this.UpdateLayout();

            //// Capture and track the mouse.
            mouseDown = true;
           // mouseDownPos = e.GetPosition(leftViewBox);
            //leftViewBox.CaptureMouse();

            //// Initial placement of the drag selection box.         
            //Canvas.SetLeft(selectionBox, mouseDownPos.X);
            //Canvas.SetTop(selectionBox, mouseDownPos.Y);
            //selectionBox.Width = 0;
            //selectionBox.Height = 0;

            //// Make the drag selection box visible.
            //selectionBox.Visibility = Visibility.Visible;
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
            //// Hide the drag selection box.
            //selectionBox.Visibility = Visibility.Collapsed;

           // Point mouseUpPos = e.GetPosition(canvasToTouch);//leftviewbox
            VisibleAD = false;
           // this.InvalidateVisual();
          //  this.InvalidateArrange();
           // this.UpdateLayout();
            myAdornerLayer.Remove(ad);
            myAdornerLayer.Add(ad);

            //// TODO: 
            ////
            //// The mouse has been released, check to see if any of the items 
            //// in the other canvas are contained within mouseDownPos and 
            //// mouseUpPos, for any that are, select them!
            ////
        }

       //List<DependencyObject> hitResultsList = new List<DependencyObject>();
       // public HitTestResultBehavior SomeTypeHitCallback(HitTestResult result)
       // {
       //    hitResultsList.Add(result.VisualHit);
       //     if (result.VisualHit is Ellipse)
       //     {
       //         var ellipse = result.VisualHit as Ellipse;

       //         // Does not work...
       //         object item = leftItemsControl.ItemContainerGenerator.ItemFromContainer(ellipse);
       //         // item now equals DependencyProperty.UnsetValue

       //         // Here I want to change the property of the object
       //         // associated with the Ellipse...
       //         var o = item as Feature;
       //         o.selected = !o.selected;
       //        // o.IsSelected = !o.IsSelected;

       //         return HitTestResultBehavior.Continue;
       //     }

       //     return HitTestResultBehavior.Continue;
       // }
       // protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
       // {
       //     Point pt = hitTestParameters.HitPoint;
       //     //return base.HitTestCore(hitTestParameters);
       //     return new PointHitTestResult(this, pt);
       // }
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pt = e.GetPosition((UIElement)sender);

                var dc = DataContext as MainWindowViewModel;
                var possible = dc.LeftCollection.Where(f => f.Left <= pt.X && f.Top <= pt.Y);
                var hits = possible.Where(f => (f.Left + f.Width) < pt.X && (f.Top + f.Height) < pt.Y);
                int stop = -1;
                //dc.LeftCollection
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

            //protected override Size MeasureOverride(Size constraint)
            //{
            //    var result = base.MeasureOverride(constraint);
            //    // ... add custom measure code here if desired ...
            //    InvalidateVisual();
            //    return result;
            //}
            // A common way to implement an adorner's rendering behavior is to override the OnRender
            // method, which is called by the layout system as part of a rendering pass.
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

        public void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
           var doesThisWork= VisualTreeHelper.GetChild(leftItemsControl, 0);
           var doesThisWorkt = VisualTreeHelper.GetChild(doesThisWork, 0);
           var doesThisWorktt = VisualTreeHelper.GetChild(doesThisWorkt, 0);
           canvasToTouch = (Canvas)doesThisWorktt;
           //var tryTryAgain= this.FindName("leftCanvas") as Canvas;
           // var itemToAdorn = (Canvas)this.Template.FindName("leftCanvas", this);
           myAdornerLayer = AdornerLayer.GetAdornerLayer((Visual)doesThisWorktt);
           ad = new SimpleCircleAdorner((UIElement)doesThisWorktt);
           myAdornerLayer.Add(ad);
           myAdornerLayer.IsHitTestVisible = false;
        }

        private void leftItemsControl_MouseLeave_1(object sender, MouseEventArgs e)
        {
            this.RRMouseUp(sender, e);
        }
 
    }

   
}
