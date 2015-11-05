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

            // Scale Polygon
            List<Point> scaledPoints = new List<Point>();
            foreach (var point in pointCollection)
            {
                scaledPoints.Add(new Point()
                {
                    X = point.X * uMatrix.Rows * 1.21,
                    Y = point.Y * uMatrix.Cols * 0.81
                });
            }

            // Element finden, das am nächsten zum Nullpunkt ist
            Point p0 = scaledPoints.OrderBy(p => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2))).First();

            // Create Polygon
            polygonPoints = new List<Drawing.Point>();

            polygonPoints.Add(new Drawing.Point(0, 0));
            polygonPoints.Add(new Drawing.Point(0, uMatrix.Rows));
            polygonPoints.Add(new Drawing.Point(uMatrix.Cols, uMatrix.Rows));
            polygonPoints.Add(new Drawing.Point(uMatrix.Cols, 0));
            polygonPoints.Add(new Drawing.Point(0, 0));

            // Punkte in der richtigen Reihenfolge laden
            int element = scaledPoints.IndexOf(p0);
            int i = element;
            while (i < scaledPoints.Count)
            {
                polygonPoints.Add(new Drawing.Point((int)(scaledPoints[i].X),(int)(scaledPoints[i].Y)));
                i++;
            }
            int j = 0;
            while (j < element)
            {
                polygonPoints.Add(new Drawing.Point((int)(scaledPoints[j].X), (int)(scaledPoints[j].Y)));
                j++;
            }

            // Noch einmal zu Ursprung zurück
            polygonPoints.Add(new Drawing.Point((int)(p0.X), (int)(p0.Y)));
            polygonPoints.Add(new Drawing.Point(0, 0));

            // Apply Polygon
            using (VectorOfPoint vPoint = new VectorOfPoint(polygonPoints.ToArray()))
            using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
            {
                CvInvoke.FillPoly(uMatrix, vvPoint, new Bgr(0, 0, 0).MCvScalar);
            }

            // Crop Bitmap
            int left = (int)scaledPoints.Min(p => p.X);
            int top = (int)scaledPoints.Min(p => p.Y);
            int width = (int)scaledPoints.Max(p => p.X) - left;
            int height = (int)scaledPoints.Max(p => p.Y) - top;

            Image<Bgr, byte> image = new Image<Bgr, byte>(uMatrix.Bitmap);
            image.ROI = new Drawing.Rectangle(left, top, width, height);

            return BitmapToBitmapImage(image.Bitmap);
        }

        public BitmapImage BitmapToBitmapImage(Drawing.Bitmap bitmap)
        {
            BitmapImage resultImage;

            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);

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
