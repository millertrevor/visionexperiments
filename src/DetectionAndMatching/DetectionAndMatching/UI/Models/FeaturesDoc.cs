using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionAndMatching.UI.Models
{
    
// The FeaturesDoc class controls the functionality of the project, and
// has methods for all major operations, like loading image and
// features, and performing queries.
public class FeaturesDoc {

	//private Fl_Shared_Image *queryImage;
    private string _queryImage;
	public FeatureSet queryFeatures; //*

	private ImageDatabase db; //*

	//private Fl_Shared_Image *resultImage;

	private int matchType;


	//public FeaturesUI ui; //*


	// Create a new document.
	public FeaturesDoc()
	{
	    
	}

	// Destroy the document.
	~FeaturesDoc()
	{
	    
	}

	// Load an image, feature set, or database.
	public void load_query_image( string queryImageName)
	{
	    _queryImage = queryImageName;
	}
	public void load_query_features( string fileName, bool sift)
	{
        if (string.IsNullOrEmpty(_queryImage) )
        {
            //fl_alert("no query image loaded");
            MessageBox.Show("No query image loaded");
        }
        else
        {
         //   ui->set_images(queryImage, NULL);
           // ui->set_features(NULL, NULL);

            // Delete the current query image features.
            if (queryFeatures != null)
            {
                //delete queryFeatures;
                queryFeatures = null;
            }

            // Delete the current result image.
            //if (resultImage != null)
            //{
            //    resultImage->release();
            //    resultImage = NULL;
            //}

            queryFeatures = new FeatureSet();

            // Load the feature set.
            if (((!sift) && (queryFeatures.load(fileName))) || ((sift) && (queryFeatures.load_sift(fileName))))
            {
               // ui->set_features(queryFeatures, NULL);
            }
            else
            {
               // delete queryFeatures;
                queryFeatures = null;

               // fl_alert("couldn't load feature data file");
                MessageBox.Show("Couldn't load feature data file");
            }
        }

       // ui->refresh();
	}
	public void load_image_database( string fileName, bool sift)
	{
	    
	}

	// Perform a query on the currently loaded image and database.
	public void perform_query()
	{
	    
	}

	// Set the pointer to the UI.
	public void set_ui(/*FeaturesUI* ui*/)
	{
	    
	}

	// Select or deselect all query features.
	public void select_all_query_features()
	{
	    
	}
	public void deselect_all_query_features()
	{
	    
	}

	// Set the match algorithm.
    public int get_match_algorithm() { return matchType; }
    public void set_match_algorithm(int type)
    {
        
    }
};
}
