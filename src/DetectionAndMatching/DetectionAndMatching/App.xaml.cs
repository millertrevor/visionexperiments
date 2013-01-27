using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionAndMatching
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //e.Args is the string[] of command line argruments
           
            if (e.Args.Length > 0)
            {
                ProcessCommandLineParameters(e.Args);
            }
            else
            {
                this.StartupUri = new System.Uri(@"UI\Views\MainWindow.xaml", System.UriKind.Relative);
            }
        }
        private void ProcessCommandLineParameters(string[] argv)
        {
            int argc = argv.Length;
            if ((argv[0].Equals( "computeFeatures")) == true)
            {
               // return mainComputeFeatures(argc, argv);
            }
            else if ((argv[0].Equals( "matchFeatures")) == true)
            {
               // return mainMatchFeatures(argc, argv);
            }
            else if ((argv[0].Equals( "matchSIFTFeatures")) == true)
            {
              //  return mainMatchSIFTFeatures(argc, argv);
            }
            else if ((argv[0].Equals( "testMatch")) == true)
            {
               // return mainTestMatch(argc, argv);
            }
            else if ((argv[0].Equals( "testSIFTMatch")) == true)
            {
                //return mainTestSIFTMatch(argc, argv);
            }
            else if ((argv[0].Equals( "benchmark")) == true)
            {
               // return mainBenchmark(argc, argv);
            }
            else if ((argv[0].Equals( "rocSIFT")) == true)
            {
                //return saveRoc(argc,argv);
               // mainRocTestSIFTMatch(argc, argv);
            }
            else if ((argv[0].Equals( "roc")) == true)
            {
                //return saveRoc(argc,argv);
               // mainRocTestMatch(argc, argv);
            }

            else
            {
                var fullName = Environment.GetCommandLineArgs()[0];
                System.IO.FileInfo fi = new System.IO.FileInfo(fullName);
                var shortName = fi.Name;
                Console.WriteLine("usage:\n");
                Console.WriteLine("\t{0}\n", shortName);
                Console.WriteLine("\t{0} computeFeatures imagefile featurefile [featuretype]\n", shortName);
                Console.WriteLine("\t{0} matchFeatures featurefile1 featurefile2 matchfile [matchtype]\n", shortName);
                Console.WriteLine("\t{0} matchSIFTFeatures featurefile1 featurefile2 matchfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} testMatch featurefile1 featurefile2 homographyfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} testSIFTMatch featurefile1 featurefile2 homographyfile [matchtype]\n", shortName);
                // Console.WriteLine("\t{0} benchmark imagedir [featuretype matchtype]\n", shortName);
                Console.WriteLine("\t{0} rocSIFT featurefile1 featurefile2 homographyfile [matchtype] rocfilename aucfilename\n", shortName);
                Console.WriteLine("\t{0} roc featurefile1 featurefile2 homographyfile [matchtype] rocfilename aucfilename\n", shortName);

                Application.Current.Shutdown(-1);
            }
        }

    }
}
