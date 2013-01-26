using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DetectionAndMatching.UI.ViewModels;

namespace DetectionAndMatching.UI.Models
{
    // FeatureMatch is used by your feature matching routine to store the
// ID of the matching feature in the other image, as well as the score
// of the match.
struct FeatureMatch {
	int id1, id2;
	double score;
};

// The Feature class stores the feature ID, location, and a vector of
// whatever attributes you choose to use.  It also has methods for
// drawing the feature and printing its description to the console.
// Feel free to change these.
public class Feature : IItemViewModel, INotifyPropertyChanged
{

    public int type;
    public int id;
    private int x;
    private int y;
    public double angleRadians;

    private List<double> data = new List<double>();

    public bool selected;
    private double _left;
    private double _top;
    private double _width;
    private double _height;
    private SolidColorBrush _color;


    // Create a feature.
    public Feature()
    {

    }

    static int iround(double x)
    {
        if (x < 0.0)
        {
            return (int)(x - 0.5);
        }
        else
        {
            return (int)(x + 0.5);
        }
    }

    private int _x1;
    private int _y1;
    private int _x2;
    private int _y2;
    private int _x3;
    private int _y3;
    private int _x4;
    private int _y4;
    private int _x5;
    private int _y5;
    // Draw the feature, currently as a square.
    // Draw a feature.  Currently, this just draws a green box for selected
    // features, and a red box for unselected features.
    public void draw() //const;
    {
        Color = selected ? Brushes.Green : Brushes.Red;

        // fl_rect(x-3, y-3, 7, 7);


        var d1 = new double[] { 5.0, 5.0 };
        var d2 = new double[] { 5.0, -5.0 };
        var d3 = new double[] { -5.0, -5.0 };
        var d4 = new double[] { -5.0, 5.0 };
        var d5 = new double[] { 5.0, 0.0 };

        double s = Math.Sin(angleRadians);
        double c = Math.Cos(angleRadians);

        _x1 = iround(x + (d1[0] * c - d1[1] * s));
        _y1 = iround(y + (d1[0] * s + d1[1] * c));

        _x2 = iround(x + (d2[0] * c - d2[1] * s));
        _y2 = iround(y + (d2[0] * s + d2[1] * c));

        _x3 = iround(x + (d3[0] * c - d3[1] * s));
        _y3 = iround(y + (d3[0] * s + d3[1] * c));

        _x4 = iround(x + (d4[0] * c - d4[1] * s));
        _y4 = iround(y + (d4[0] * s + d4[1] * c));

        _x5 = iround(x + (d5[0] * c - d5[1] * s));
        _y5 = iround(y + (d5[0] * s + d5[1] * c));

        var xm1 = Math.Max(_x1, _x2);
        var xm2 = Math.Max(_x3, _x4);
        var maxX = Math.Max(xm1, xm2);

        var ym1 = Math.Max(_y1, _y2);
        var ym2 = Math.Max(_y3, _y4);
        var yMax = Math.Max(ym1, ym2);

        var xmin1 = Math.Min(_x1, _x2);
        var xmin2 = Math.Min(_x3, _x4);
        var minX = Math.Min(xmin1, xmin2);

        var ymin1 = Math.Min(_y1, _y2);
        var ymin2 = Math.Min(_y3, _y4);
        var yMin = Math.Min(ymin1, ymin2);

        Width = maxX - minX;

        Top = yMin;
        Left = minX;
        Height = yMax - yMin;
    }

    // Print the feature, currently just prints the location.
    public void print()
    {

    }

    // Reads a SIFT feature.
    public void read_sift(TextReader incomingStream)
    {
        // Let's use type 9 for SIFT features.
        type = 9;

        double xSub;
        double ySub;
        double scale;
        double rotation;

        // Read the feature location, scale, and orientation.
        //is >> xSub >> ySub >> scale >> rotation;
        string fourParams = incomingStream.ReadLine();
        string[] bits = fourParams.Split(' ');
        //int x = int.Parse(bits[0]);
        //double y = double.Parse(bits[1]);
        //string z = bits[2];
        xSub = double.Parse(bits[0]);
        ySub = double.Parse(bits[1]);
        scale = double.Parse(bits[2]);
        rotation = double.Parse(bits[3]);


        // They give row first, then column.
        x = (int)(ySub + 0.5);
        y = (int)(xSub + 0.5);
        angleRadians = rotation;

        //data.resize(128);
        data.Capacity = 128;

        // Read the descriptor vector.
        // for (int i = 0; i < 128; i++)
        // {
        //is >> data[i];

        // }
        for (int i = 0; i < 7; i++)
        {
            string oneOfSeven = incomingStream.ReadLine();
            oneOfSeven = oneOfSeven.Trim();
            string[] dataSplit = oneOfSeven.Split(' ');
            foreach (var item in dataSplit)
            {
                data.Add(double.Parse(item));
            }
        }
    }

    public double Left
    {
        get { return _left; }
        set { _left = value; }
    }

    public double Top
    {
        get { return _top; }
        set { _top = value; }
    }

    public double Width
    {
        get { return _width; }
        set { _width = value; }
    }

    public double Height
    {
        get { return _height; }
        set { _height = value; }
    }

    public SolidColorBrush Color
    {
        get { return _color; }
        set { _color = value; NotifyPropertyChanged("Color"); }
    }

    public int X1
    {
        get { return _x1; }
        set { _x1 = value; }
    }

    public int Y1
    {
        get { return _y1; }
        set { _y1 = value; }
    }

    public int X2
    {
        get { return _x2; }
        set { _x2 = value; }
    }

    public int Y2
    {
        get { return _y2; }
        set { _y2 = value; }
    }

    public int X3
    {
        get { return _x3; }
        set { _x3 = value; }
    }

    public int Y3
    {
        get { return _y3; }
        set { _y3 = value; }
    }

    public int X4
    {
        get { return _x4; }
        set { _x4 = value; }
    }

    public int Y4
    {
        get { return _y4; }
        set { _y4 = value; }
    }

    public int X5
    {
        get { return _x5; }
        set { _x5 = value; }
    }

    public int Y5
    {
        get { return _y5; }
        set { _y5 = value; }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
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

// You don't have to modify these.
//ostream &operator<<(ostream &os, const Feature &f);
//istream &operator>>(istream &is, Feature &f);

// The FeatureSet class represents a vector of features for a single
// image.  You don't need to modify it.
public class FeatureSet : List<Feature>
{

	// Create a feature set.
	public FeatureSet()
	{
	    
	}

	// Load a feature set from a file.
    public bool load(string incomingFileName)
    {
        return false;
    }

    // Load a SIFT feature set.
	public bool load_sift(string incomingFileName)
	{
	    int n;
	int m;

	// Clear the currently loaded features.
	Clear();

	// Open the file.
    using (TextReader reader = File.OpenText(incomingFileName))
	    {
	        //string text = reader.ReadLine();
	        //string[] bits = text.Split(' ');
	        //int x = int.Parse(bits[0]);
	        //double y = double.Parse(bits[1]);
	        //string z = bits[2];

            //We will throw an exception if the OpenText commanddoesn't work.
            //if (!f.is_open())
            //{
            //    return false;
            //}

	        // Read the total number of features.
	        string totalNumberFeaturesAndFeatureLength = reader.ReadLine();
	        var parsed = totalNumberFeaturesAndFeatureLength.Split(' ');
            n = int.Parse(parsed[0]);
            m = int.Parse(parsed[1]);
	       // f >> n;

	        // Read the length of each feature.  It better be 128.
	        //f >> m;

	        if (m != 128)
	        {
	            //f.close();
	            return false;
	        }

	        // Resize the vector of features.
            //List Should AutoGrow
	        this.Capacity = n;
	        //resize(n);

	        // Read each of the features.
	       // iterator i = begin();
	        int id = 1;

            //while (i != end())
            //{
            //    (*i).read_sift(f);
            //    (*i).id = id;

            //    i++;
            //    id++;
            //}
            for (int i = 0; i < n; i++)
            {
                var feature = new Feature();
                feature.read_sift(reader);
                feature.id = id;
                id++;
                Add(feature);
            }

	        // Close the file.
	       // f.close();
	    }

	    return true;
	}

	// Save a feature set to file.
	public bool save( /*const char *name*/)
	{
	    return false;
	}

	// Select (or deselect) features at a location.
	public void select_point(double x, double y)
	{
        foreach (var item in this)
        {
            if((Math.Abs(item.X-x)<=5)&&(Math.Abs(item.Y-y)<=5))
            {
                item.selected = !item.selected;
                item.Color = item.selected ? Brushes.Green : Brushes.Red;
            }
        }
	}

	// Select (or deselect) features inside a box.
	public void select_box(double xMin, double xMax, double yMin, double yMax)
	{
        foreach (var item in this)
        {
            if ((item.X >= xMin) && (item.X <= xMax) && (item.Y >= yMin) && (item.Y <= yMax))
            {
                item.selected = (!item.selected);
                item.Color = item.selected ? Brushes.Green : Brushes.Red;
            }
        }
	}

	// Select all features.
	public void select_all()
	{
	    
	}

	// Deselect all features.
	public void deselect_all()
	{
	    
	}

	// Take only the selected features.
	public void get_selected_features(/*FeatureSet &f*/)
	{
	    
	}
};
    
}
