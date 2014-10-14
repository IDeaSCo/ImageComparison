using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KirWa.ImageComparison
{
    public static class ConsoleInterface
    {
        public static void getDiifImage(String Path1, String Path2, String resultPath)
        {
            Bitmap firstBmp = (Bitmap)Image.FromFile(Path1);
            Bitmap secondBmp = (Bitmap)Image.FromFile(Path2);
            firstBmp.GetDifferenceImage(secondBmp).Save(resultPath + "_diff.png");
        }
    }
}
