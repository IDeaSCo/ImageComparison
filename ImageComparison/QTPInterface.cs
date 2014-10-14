using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace KirWa.ImageComparison
{
    public class QTPInterface
    {
        /// <summary>
        /// Class to us while accesing the Image comparison through QTP
        /// </summary>
        public QTPInterface()
        {
            ImageTool.divFactor = 1;
        }
       /// <summary>
       /// Constructour for Interface
       /// </summary>
       /// <param name="divRation">Image divison number(bigger the number smaller the array)</param>
        public QTPInterface(int divRation)
        {
            ImageTool.divFactor = divRation;
        }
        /// <summary>
        /// Get diffrance Image Grid
        /// </summary>
        /// <param name="Path1">Pathe of First image to compare</param>
        /// <param name="Path2">Pathe of Second image to compare</param>
        /// <param name="resultPath">Pathe of Result grid image</param>
        public void getDiifImage(String Path1, String Path2,String resultPath)
        {
            Bitmap firstBmp = (Bitmap)Image.FromFile(Path1);
            Bitmap secondBmp = (Bitmap)Image.FromFile(Path2);
            firstBmp.GetDifferenceImage(secondBmp).Save(resultPath + "_diff.png");
        }

        public void getDiifImage(String Path1, String Path2)
        {
            Bitmap firstBmp = (Bitmap)Image.FromFile(Path1);
            Bitmap secondBmp = (Bitmap)Image.FromFile(Path2);
            firstBmp.GetDifferenceImage(secondBmp).Save(Path1 + "_diff.png");
        }
        
        public byte[,] getDiifMatrx(String Path1, String Path2)
        {
            Bitmap firstBmp = (Bitmap)Image.FromFile(Path1);
            Bitmap secondBmp = (Bitmap)Image.FromFile(Path2);
            byte[,] x = firstBmp.GetDifferences(secondBmp);
            return x;
        }
    }
}
