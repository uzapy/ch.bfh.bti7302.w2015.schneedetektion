using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Media = System.Windows.Media;

namespace Schneedetektion.OpenCV
{
    public class OpenCVHelper
    {
        private Drawing.Point[] polygonPoints;

        public BitmapImage GetMaskedImage(string imagePath, Media.PointCollection pointCollection)
        {
            Mat matrix = new Mat(imagePath, LoadImageType.AnyColor);
            UMat uMatrix = matrix.ToUMat(AccessType.ReadWrite);

            polygonPoints = new Drawing.Point[pointCollection.Count + 4];

            // Punkte laden und herunterskalieren
            for (int i = 0; i < pointCollection.Count; i++)
            {
                double x = pointCollection[i].X / 1200 * uMatrix.Rows;
                double y = pointCollection[i].Y / 1000 * uMatrix.Cols;

                polygonPoints[i] = new Drawing.Point((int)x, (int)y);
            }

            polygonPoints[pointCollection.Count + 0] = new Drawing.Point(0, uMatrix.Rows);
            polygonPoints[pointCollection.Count + 1] = new Drawing.Point(0, 0);
            polygonPoints[pointCollection.Count + 2] = new Drawing.Point(uMatrix.Cols, 0);
            polygonPoints[pointCollection.Count + 3] = new Drawing.Point(uMatrix.Cols, uMatrix.Rows);

            using (VectorOfPoint vPoint = new VectorOfPoint(polygonPoints))
            using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
            {
                CvInvoke.FillPoly(uMatrix, vvPoint, new Bgr(128, 0, 0).MCvScalar);
            }

            //return Imaging.CreateBitmapSourceFromHBitmap(uMatrix.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            using (MemoryStream stream = new MemoryStream())
            {
                uMatrix.Bitmap.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
