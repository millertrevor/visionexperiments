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
	public string name;
	public FeatureSet features;
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
        var dirString = new System.IO.FileInfo(fileName).Directory.FullName + Path.DirectorySeparatorChar;
      
        // Clear all entries from the database.
        this.Clear();

        using (TextReader reader = File.OpenText(fileName))
        {
            string allFiles = reader.ReadToEnd();
            allFiles = allFiles.Trim();
            var parsed = allFiles.Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.None);
            for (int i = 0; i < parsed.Length; i++)
            {
                DatabaseItem d;
                d.features = new FeatureSet();
                string s;

                var imageFile = parsed[i];
                i++;
                var featureFile = parsed[i];

                d.name =dirString+ imageFile;
                s =dirString+ featureFile;
                if (((!sift) && (!d.features.load(s))) || ((sift) && (!d.features.load_sift(s))))
                {
                    Clear();
                   // f.close();
                    return false;
                }
                Add(d);
            }
           
        }
	    return true;
	}
}

}
