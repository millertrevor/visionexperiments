using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionAndMatching.UI.Models
{
    
// A DatabaseItem holds the name of an image, and the corresponding
// feature set.  The images themselves are not stored in memory.
struct DatabaseItem {
	string name;
	FeatureSet features;
};

// The ImageDatabase class is a vector of database items.
class ImageDatabase : List<DatabaseItem> {

	// Create a new database.
	public ImageDatabase()
	{
	    
	}

	// Load a database from file.
	public bool load(/*const char *name,*/ bool sift)
	{
	    return false;
	}
}

}
