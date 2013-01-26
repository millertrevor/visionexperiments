using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DetectionAndMatching.UI.Models
{
    public interface IFeature
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
}
