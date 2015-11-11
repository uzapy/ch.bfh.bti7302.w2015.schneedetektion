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

            polygonPoints = GetPolygonPoints(scaledPoints, uMatrix.Rows, uMatrix.Cols);

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

        private List<Drawing.Point> GetPolygonPoints(List<Point> scaledPoints, int numberOfRows, int numberOfCols)
        {
            // Element finden, das am nächsten zum Nullpunkt ist
            Point p0 = scaledPoints.OrderBy(p => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2))).First();

            // Create Polygon
            List<Drawing.Point> polygon = new List<Drawing.Point>();
            polygon.Add(new Drawing.Point(0, 0));
            polygon.Add(new Drawing.Point(0, numberOfRows));
            polygon.Add(new Drawing.Point(numberOfCols, numberOfRows));
            polygon.Add(new Drawing.Point(numberOfCols, 0));
            polygon.Add(new Drawing.Point(0, 0));

            // Punkte in der richtigen Reihenfolge laden
            int element = scaledPoints.IndexOf(p0);
            int i = element;
            while (i < scaledPoints.Count)
            {
                polygon.Add(new Drawing.Point((int)(scaledPoints[i].X), (int)(scaledPoints[i].Y)));
                i++;
            }
            int j = 0;
            while (j < element)
            {
                polygon.Add(new Drawing.Point((int)(scaledPoints[j].X), (int)(scaledPoints[j].Y)));
                j++;
            }

            // Noch einmal zu Ursprung zurück
            polygon.Add(new Drawing.Point((int)(p0.X), (int)(p0.Y)));
            polygon.Add(new Drawing.Point(0, 0));

            return polygon;
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

        public BitmapImage CalculateAverage(IList<string> images)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(images[0]);
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(images[1]);

            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            result1 = image0.AbsDiff(image1);
            result1._ThresholdBinaryInv(new Bgr(75, 75, 75), new Bgr(255, 255, 255));

            CvInvoke.cvCopy(image1, result1, result1);

            return BitmapToBitmapImage(result1.Bitmap);
            
            // ------------------------------------------------------------------------------------//

            //for (int i = 0; i < 288; i++)
            //{
            //    for (int j = 0; j < 352; j++)
            //    {
            //        result.Data[i, j, 0] = (byte)Math.Abs(image0.Data[i, j, 0] - image1.Data[i, j, 0]);
            //        result.Data[i, j, 1] = (byte)Math.Abs(image0.Data[i, j, 1] - image1.Data[i, j, 1]);
            //        result.Data[i, j, 2] = (byte)Math.Abs(image0.Data[i, j, 2] - image1.Data[i, j, 2]);
            //    }
            //}

            // ------------------------------------------------------------------------------------//

            //UMat matrix0 = new Mat(images[0], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix1 = new Mat(images[1], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrixResult1 = new UMat();
            //CvInvoke.AddWeighted(matrix0, 0.5, matrix1, 0.5, 0, matrixResult1);

            //UMat matrix2 = new Mat(images[2], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix3 = new Mat(images[3], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrixResult2 = new UMat();
            //CvInvoke.AddWeighted(matrix2, 0.5, matrix3, 0.5, 0, matrixResult2);

            //UMat matrix4 = new Mat(images[4], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix5 = new Mat(images[5], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrixResult3 = new UMat();
            //CvInvoke.AddWeighted(matrix4, 0.5, matrix5, 0.5, 0, matrixResult3);

            //UMat matrixResult4 = new UMat();
            //CvInvoke.AddWeighted(matrixResult1, 0.5, matrixResult2, 0.5, 0, matrixResult4);

            //UMat matrixResult5 = new UMat();
            //CvInvoke.AddWeighted(matrixResult3, 0.333, matrixResult4, 0.666, 0, matrixResult5);

            //return BitmapToBitmapImage(matrixResult5.Bitmap);

            // ------------------------------------------------------------------------------------//

            //UMat matrix0 = new Mat(images[0], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix1 = new Mat(images[1], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix2 = new Mat(images[2], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix3 = new Mat(images[3], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix4 = new Mat(images[4], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);
            //UMat matrix5 = new Mat(images[5], LoadImageType.AnyColor).ToUMat(AccessType.ReadWrite);

            //Mat matrix = new Mat(new Drawing.Size(352, 288), DepthType.Cv8U, 3);
            //return BitmapToBitmapImage(matrix.Bitmap);

            // ------------------------------------------------------------------------------------//
        }
    }
}
