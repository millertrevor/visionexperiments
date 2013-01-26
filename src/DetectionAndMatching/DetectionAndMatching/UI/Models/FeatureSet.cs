using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

using DetectionAndMatching.UI.ViewModels;

namespace DetectionAndMatching.UI.Models
{
    // FeatureMatch is used by your feature matching routine to store the
    // ID of the matching feature in the other image, as well as the score
    // of the match.
    struct FeatureMatch
    {
        int id1, id2;
        double score;
    };



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

                // Read the total number of features.
                string totalNumberFeaturesAndFeatureLength = reader.ReadLine();
                var parsed = totalNumberFeaturesAndFeatureLength.Split(' ');
                n = int.Parse(parsed[0]);
                m = int.Parse(parsed[1]);

                if (m != 128)
                {
                    return false;
                }

                this.Capacity = n;

                // Read each of the features.
                int id = 1;
                for (int i = 0; i < n; i++)
                {
                    var feature = new Feature();
                    feature.read_sift(reader);
                    feature.id = id;
                    id++;
                    Add(feature);
                }
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
                if ((Math.Abs(item.X - x) <= 5) && (Math.Abs(item.Y - y) <= 5))
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
    }

}
