using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;

namespace Schneedetektion.OpenCV
{
    public class OpenCVHelper
    {
        private List<Drawing.Point> polygonPoints;

        public BitmapImage GetMaskedImage(string imagePath, IList<Point> pointCollection)
        {
            Mat matrix = new Mat(imagePath, LoadImageType.AnyColor);
            UMat uMatrix = matrix.ToUMat(AccessType.ReadWrite);

            polygonPoints = new List<Drawing.Point>();

            polygonPoints.Add(new Drawing.Point(0, 0));
            polygonPoints.Add(new Drawing.Point(0, uMatrix.Rows));
            polygonPoints.Add(new Drawing.Point(uMatrix.Cols, uMatrix.Rows));
            polygonPoints.Add(new Drawing.Point(uMatrix.Cols, 0));
            polygonPoints.Add(new Drawing.Point(0, 0));


            // Element finden, das am nächsten zum Nullpunkt ist
            Point p0 = pointCollection.OrderBy(p => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2))).First();

            // Punkte in der richtigen Reihenfolge laden und herunterskalieren
            int element = pointCollection.IndexOf(p0);
            int i = element;
            while (i < pointCollection.Count)
            {
                polygonPoints.Add(
                    new Drawing.Point(
                        (int)(pointCollection[i].X * uMatrix.Rows * 1.21),
                        (int)(pointCollection[i].Y * uMatrix.Cols * 0.81)
                    )
                );
                i++;
            }
            int j = 0;
            while (j < element)
            {
                polygonPoints.Add(
                     new Drawing.Point(
                         (int)(pointCollection[j].X * uMatrix.Rows * 1.21),
                         (int)(pointCollection[j].Y * uMatrix.Cols * 0.81)
                     )
                 );
                j++;
            }

            // Noch einmal zu Ursprung zurück
            polygonPoints.Add(new Drawing.Point((int)(p0.X * uMatrix.Rows * 1.21), (int)(p0.Y * uMatrix.Cols * 0.81)));

            polygonPoints.Add(new Drawing.Point(0, 0));

            using (VectorOfPoint vPoint = new VectorOfPoint(polygonPoints.ToArray()))
            using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
            {
                CvInvoke.FillPoly(uMatrix, vvPoint, new Bgr(0, 0, 0).MCvScalar);
            }

            //return Imaging.CreateBitmapSourceFromHBitmap(uMatrix.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            BitmapImage resultImage;

            using (MemoryStream stream = new MemoryStream())
            {
                uMatrix.Bitmap.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                resultImage = new BitmapImage();
                resultImage.BeginInit();
                resultImage.StreamSource = stream;
                resultImage.CacheOption = BitmapCacheOption.OnLoad;
                resultImage.EndInit();
            }
            return resultImage;
        }
    }
}
