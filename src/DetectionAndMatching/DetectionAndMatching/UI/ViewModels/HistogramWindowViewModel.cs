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
    public class HistogramWindowViewModel: INotifyPropertyChanged
    {
        private PointCollection luminanceHistogramPoints = null;
        private PointCollection redColorHistogramPoints = null;
        private PointCollection greenColorHistogramPoints = null;
        private PointCollection blueColorHistogramPoints = null;

        private PointCollection luminanceHistogramPointsR = null;
        private PointCollection redColorHistogramPointsR = null;
        private PointCollection greenColorHistogramPointsR = null;
        private PointCollection blueColorHistogramPointsR = null;

        public bool PerformHistogramSmoothing { get; set; }

        public PointCollection LuminanceHistogramPoints
        {
            get
            {
                return this.luminanceHistogramPoints;
            }
            set
            {
                if (this.luminanceHistogramPoints != value)
                {
                    this.luminanceHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LuminanceHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection RedColorHistogramPoints
        {
            get
            {
                return this.redColorHistogramPoints;
            }
            set
            {
                if (this.redColorHistogramPoints != value)
                {
                    this.redColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection GreenColorHistogramPoints
        {
            get
            {
                return this.greenColorHistogramPoints;
            }
            set
            {
                if (this.greenColorHistogramPoints != value)
                {
                    this.greenColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection BlueColorHistogramPoints
        {
            get
            {
                return this.blueColorHistogramPoints;
            }
            set
            {
                if (this.blueColorHistogramPoints != value)
                {
                    this.blueColorHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramPoints"));
                    }
                }
            }
        }

        public PointCollection LuminanceHistogramPointsR
        {
            get
            {
                return this.luminanceHistogramPointsR;
            }
            set
            {
                if (this.luminanceHistogramPointsR != value)
                {
                    this.luminanceHistogramPointsR = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LuminanceHistogramPointsR"));
                    }
                }
            }
        }

        public PointCollection RedColorHistogramPointsR
        {
            get
            {
                return this.redColorHistogramPointsR;
            }
            set
            {
                if (this.redColorHistogramPointsR != value)
                {
                    this.redColorHistogramPointsR = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RedColorHistogramPointsR"));
                    }
                }
            }
        }

        public PointCollection GreenColorHistogramPointsR
        {
            get
            {
                return this.greenColorHistogramPointsR;
            }
            set
            {
                if (this.greenColorHistogramPointsR != value)
                {
                    this.greenColorHistogramPointsR = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("GreenColorHistogramPointsR"));
                    }
                }
            }
        }

        public PointCollection BlueColorHistogramPointsR
        {
            get
            {
                return this.blueColorHistogramPointsR;
            }
            set
            {
                if (this.blueColorHistogramPointsR != value)
                {
                    this.blueColorHistogramPointsR = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("BlueColorHistogramPointsR"));
                    }
                }
            }
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
