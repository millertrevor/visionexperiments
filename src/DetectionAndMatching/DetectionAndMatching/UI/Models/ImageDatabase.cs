using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    // Load a database from file.  The database file contains a list of
    // image file names and feature file names.  Each image file name is
    // followed by the corresponding feature file name.  The file names in
    // the database must be relative to the database path or it won't work.
    // I apologize for this annoyance.
	public bool load(string fileName, bool sift)
	{
        DatabaseItem d;
        string s;
        // Clear all entries from the database.
        this.Clear();

        using (TextReader reader = File.OpenText(fileName))
        {
        }
	    return false;
	}
}

}
