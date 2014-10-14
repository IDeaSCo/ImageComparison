using System;
using KirWa.ImageComparison;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Diagnostics;


namespace TestingImageComparison_Console
{
    class Program
    {

        static DirectoryInfo exeFolder, projectFolder, sampleImagesFolder;

        public static void Main(string[] args)
        {
            //find the image folder
            string codebase = Assembly.GetExecutingAssembly().Location;
            exeFolder = new DirectoryInfo(Path.GetDirectoryName(codebase));
            projectFolder = exeFolder.Parent.Parent;
            sampleImagesFolder = new DirectoryInfo(Path.Combine(projectFolder.FullName, "SampleImages"));


            //compare different images
            Compare("1.png", "2.png");
           // Compare("img_one.png", "img_two.png");

            //show the images in the folder
            Process.Start(sampleImagesFolder.FullName);

            Console.WriteLine("Any key to end...");
            Console.ReadKey();
        }

        static void Compare(string bmp1, string bmp2, byte threshold = 5)
        {

            //get the full path of the images
            string image1Path = Path.Combine(sampleImagesFolder.FullName, bmp1);
            string image2Path = Path.Combine(sampleImagesFolder.FullName, bmp2);

            //compare the two
            Console.Write("Comparing: " + bmp1 + " and " + bmp2 + ", with a threshold of " + threshold);
            Bitmap firstBmp = (Bitmap)Image.FromFile(image1Path);
            Bitmap secondBmp = (Bitmap)Image.FromFile(image2Path);
            //get the difference as a bitmap
            firstBmp.GetDifferenceImage(secondBmp,true)
                .Save(image1Path + "_diff.png");
            Console.WriteLine(string.Format("Difference: {0:0.0} %", firstBmp.PercentageDifference(secondBmp, threshold) * 100));
            //Console.WriteLine("ENTER see histogram for " + bmp1);
            //Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Creating histogram for " + bmp1);
            Histogram hist = new Histogram(firstBmp);
           // hist.Visualize().Save(image1Path + "_hist.png");
            Console.WriteLine(hist.ToString());
            Console.WriteLine("ENTER to continue...");
            Console.ReadLine();

            
        }
    }
}
