﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;


namespace KirWa.ImageComparison
{

    /// <summary>
    /// A class with extensionmethods for comparing images
    /// </summary>
    public static class ImageTool
    {
        //the font to use for the DifferenceImages
        private static readonly Font DefaultFont = new Font("Arial", 8);
        public static int divFactor = 10;
        //the brushes to use for the DifferenceImages
        private static Brush[] brushes = new Brush[256];

        //the colormatrix needed to grayscale an image
        
        static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][] 
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        //Create the brushes in varying intensities
        static ImageTool()
        {
            for (int i = 0; i < 256; i++)
            {
                brushes[i] = new SolidBrush(Color.FromArgb(255, i, i / 3, i / 2));
            }
        }

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float PercentageDifference(this Image img1, Image img2, byte threshold = 3)
        {
            byte[,] differences = img1.GetDifferences(img2);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / 256f;
        }

        /// <summary>
        /// Gets an image which displays the differences between two images
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <param name="adjustColorSchemeToMaxDifferenceFound">Whether to adjust the color indicating maximum difference (usually 255) to the maximum difference found in this case.
        /// E.g. if the maximum difference found is 12, then a true value in adjustColorSchemeToMaxDifferenceFound would result in 0 being black, 6 being dark pink, and 12 being bright pink.
        /// A false value would still have differences of 255 as bright pink resulting in the 12 difference still being very dark.</param>
        /// <param name="percentages">Whether to write percentages in each of the 255 squares (true) or the absolute value (false)</param>
        /// <returns>an image which displays the differences between two images</returns>
        public static Bitmap GetDifferenceImage(this Image img1, Image img2, bool adjustColorSchemeToMaxDifferenceFound = true, bool absoluteText = true)
        {
            //create a 16x16 tiles image with information about how much the two images differ
            int cellsize = 16;  //each tile is 16 pixels wide and high
            int width = img1.Width / divFactor, height = img1.Height / divFactor;

            Bitmap bmp = new Bitmap(width * cellsize + 1, height * cellsize + 1); //16 blocks * 16 pixels + a borderpixel at left/bottom

            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
            byte[,] differences = img1.GetDifferences(img2);
            byte maxDifference = 255;

            //if wanted - adjust the color scheme, by finding the new maximum difference
            if (adjustColorSchemeToMaxDifferenceFound)
            {
                maxDifference = 0;
                foreach (byte b in differences)
                {
                    if (b > maxDifference)
                    {
                        maxDifference = b;
                    }
                }

                if (maxDifference == 0)
                {
                    maxDifference = 1;
                }

            }

            for (int y = 0; y < differences.GetLength(1); y++)
            {
                for (int x = 0; x < differences.GetLength(0); x++)
                {
                    byte cellValue = differences[x, y];
                    string cellText = null;
                    
                    if (absoluteText)
                    {
                        cellText = cellValue.ToString();
                    }
                    else
                    {
                        cellText = string.Format("{0}%", (int)cellValue);
                    }

                    float percentageDifference = (float)differences[x, y] / maxDifference;
                    int colorIndex = (int)(255 * percentageDifference);

                    g.FillRectangle(brushes[colorIndex], x * cellsize, y * cellsize, cellsize, cellsize);
                    g.DrawRectangle(Pens.Blue, x * cellsize, y * cellsize, cellsize, cellsize);
                    SizeF size = g.MeasureString(cellText, DefaultFont);
                    g.DrawString(cellText, DefaultFont, Brushes.Black, x * cellsize + cellsize / 2 - size.Width / 2 + 1, y * cellsize + cellsize / 2 - size.Height / 2 + 1);
                    g.DrawString(cellText, DefaultFont, Brushes.White, x * cellsize + cellsize / 2 - size.Width / 2, y * cellsize + cellsize / 2 - size.Height / 2);
                }
            }

            return bmp;
        }


        /// <summary>
        /// Finds the differences between two images and returns them in a doublearray
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>the differences between the two images as a doublearray</returns>
        public static byte[,] GetDifferences(this Image img1, Image img2)
        {
            int width = img1.Width / divFactor, height = img1.Height / divFactor;
            Bitmap thisOne = (Bitmap)img1.Resize(width, height).GetGrayScaleVersion();
            Bitmap theOtherOne = (Bitmap)img2.Resize(width, height).GetGrayScaleVersion();
            
            byte[,] differences = new byte[width, height];

            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    differences[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R - theOtherOne.GetPixel(x, y).R);
                }
            }
            //differences.ToConsole();
            return differences;
        }



        
        /// <summary>
        /// Converts an image to grayscale
        /// </summary>
        /// <param name="original">The image to grayscale</param>
        /// <returns>A grayscale version of the image</returns>
        public static Image GetGrayScaleVersion(this Image original)
        {
          
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(ColorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();

            return newBitmap;

        }

        /// <summary>
        /// Resizes an image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static Image Resize(this Image originalImage, int newWidth, int newHeight)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return smallVersion;
        }

        /// <summary>
        /// Helpermethod to print a doublearray of 
        /// </summary>
        /// <typeparam name="T">The type of doublearray</typeparam>
        /// <param name="doubleArray">The doublearray to print</param>
        public static void ToConsole<T>(this T[,] doubleArray)
        {
            for (int y = 0; y < doubleArray.GetLength(0); y++)
            {
                Console.Write("[");
                for (int x = 0; x < doubleArray.GetLength(1); x++)
                {
                    Console.Write(string.Format("{0,3},", doubleArray[x, y]));
                }
                Console.WriteLine("]");
            }
        }

        /// <summary>
        /// Gets a bitmap with the RGB histograms of a bitmap
        /// </summary>
        /// <param name="bmp">The bitmap to get the histogram for</param>
        /// <returns>A bitmap with the histogram for R, G and B values</returns>
        public static Bitmap GetRgbHistogramBitmap(this Bitmap bmp)
        {
            return new Histogram(bmp).Visualize();
        }

        /// <summary>
        /// Get a histogram for a bitmap
        /// </summary>
        /// <param name="bmp">The bitmap to get the histogram for</param>
        /// <returns>A histogram for the bitmap</returns>
        public static Histogram GetRgbHistogram(this Bitmap bmp)
        {
            return new Histogram(bmp);
        }


        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <returns>The difference between the two images as a percentage</returns>
        /// <param name="image1Path">The path to the first image</param>
        /// <param name="image2Path">The path to the second image</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float GetPercentageDifference(string image1Path, string image2Path, byte threshold = 3)
        {
            if (CheckFile(image1Path) && CheckFile(image2Path))
            {
                Image img1 = Image.FromFile(image1Path);
                Image img2 = Image.FromFile(image2Path);

                return img1.PercentageDifference(img2, threshold);
            }
            else return -1;
            
        }

        private static bool CheckFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File '" + filePath + "' not found!");
            }
            return true;
        }

    }
}
