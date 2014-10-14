using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KirWa.ImageComparison;


namespace ConsoleComparison
{

    /// <summary>
    /// Console program which compares two iages and returns the difference in percentage as an errorlevel between zero and a hundred (both included).
    /// </summary>
    class Program
    {

        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("IMAGE COMPARISON CONSOLE APPLICATION");
                Console.WriteLine("  by Kirtesh Wani");
                Console.WriteLine("  Compares two images and returns the difference as an errorlevel (0 to 100)");
                Console.WriteLine();
                Console.WriteLine(@"  Usage: 'ImageComparisonConsole.exe [image1 path] [image2 path] [Result path]");
                Console.WriteLine(@"  Sample usage: 'ImageComparisonConsole.exe ""c:\image1.jpg"" ""c:\image2.bmp"" ""c:\result""");
                Console.Read();
                return -1;
              /*  String[] args2 ={"C:/1.png","C:/2.png","C:/1"};

                int difference = (int)(ImageTool.GetPercentageDifference(args2[0], args2[1]) * 100);

                //display the difference
                Console.WriteLine("Difference is {0}%", difference);

                //Save the Matrix Image File at the Location

                ConsoleInterface.getDiifImage(args2[0], args2[1], args2[2]);

                //return the difference as an errorlevel
                return difference; */

            }
            else
            {
                //get the difference
                int difference = (int)(ImageTool.GetPercentageDifference(args[0], args[1])*100);
                
                //display the difference
                Console.WriteLine("Difference is {0}%", difference);

                //Save the Matrix Image File at the Location

                ConsoleInterface.getDiifImage(args[0], args[1], args[2]);
                
                //return the difference as an errorlevel
                return difference;
            }
        }
    }
}
