using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using Schneedetektion.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Media = System.Windows.Media;

namespace Schneedetektion.OpenCV
{
    public class OpenCVHelper
    {
        private List<Drawing.Point> polygonPoints;

        public BitmapImage GetMaskedImage(string imagePath, IList<Point> pointCollection)
        {
            Drawing.Bitmap bitmap = GetMaskedBitmap(imagePath, pointCollection);

            return BitmapToBitmapImage(bitmap);
        }

        private Drawing.Bitmap GetMaskedBitmap(string imagePath, IList<Point> pointCollection)
        {
            Mat matrix = new Mat(imagePath, LoadImageType.AnyColor);
            UMat uMatrix = matrix.ToUMat(AccessType.ReadWrite);

            // Scale Polygon
            List<Point> scaledPoints = GetScaledPoints(pointCollection, uMatrix.Rows, uMatrix.Cols);

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

            return image.Bitmap;
        }

        private List<Point> GetScaledPoints(IEnumerable<Point> polygonPoints, int numberOfRows, int numberOfCols)
        {
            List<Point> scaledPoints = new List<Point>();
            foreach (var point in polygonPoints)
            {
                scaledPoints.Add(new Point()
                {
                    X = point.X * numberOfRows * 1.21,
                    Y = point.Y * numberOfCols * 0.81
                });
            }
            return scaledPoints;
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

        private BitmapImage BitmapToBitmapImage(Drawing.Bitmap bitmap)
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

        private Drawing.Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            Drawing.Bitmap bitmap;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                bitmap = new Drawing.Bitmap(stream);
            }
            return bitmap;
        }

        public BitmapImage CalculateAbsoluteDifference(string imagePath0, string imagePath1)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(imagePath0);
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(imagePath1);

            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            result1 = image0.AbsDiff(image1);
            result1._ThresholdBinaryInv(new Bgr(75, 75, 75), new Bgr(255, 255, 255));

            //CvInvoke.cvCopy(image1, result1, result1);

            return BitmapToBitmapImage(result1.Bitmap);
        }

        public BitmapImage IntersectMasks(BitmapImage bitmapImage1, BitmapImage bitmapImage2)
        {

            Image<Bgr, byte> result = new Image<Bgr, byte>(new byte[288, 352, 3]);
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage1));
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage2));
            CvInvoke.BitwiseOr(image1, image2, result);
            return BitmapToBitmapImage(result.Bitmap);
        }

        public void CalculateAverageBrightessForArea(string reference0, string reference1, StrassenbilderMetaDataContext dataContext)
        {
            // Image-Meta-Daten laden
            string name0 = Path.GetFileNameWithoutExtension(reference0);
            string name1 = Path.GetFileNameWithoutExtension(reference1);
            Image image0 = dataContext.Images.Where(i => i.Name == name0).FirstOrDefault();
            Image image1 = dataContext.Images.Where(i => i.Name == name1).FirstOrDefault();

            // Polygone Laden
            IEnumerable<Polygon> polygons = dataContext.Polygons.Where(p => p.CameraName == image0.Place);

            // Pro Maske anwenden
            foreach (var polygon in polygons)
            {
                IList<Point> polygonPoints = JsonConvert.DeserializeObject<Media.PointCollection>(polygon.PolygonPointCollection);

                // Maskiertes Bild laden
                Drawing.Bitmap bitmap0 = GetMaskedBitmap(reference0, polygonPoints);
                Drawing.Bitmap bitmap1 = GetMaskedBitmap(reference1, polygonPoints);

                Image<Bgr, byte> cvImage0 = new Image<Bgr, byte>(bitmap0);
                Image<Bgr, byte> cvImage1 = new Image<Bgr, byte>(bitmap1);

                // Maske generieren aus Polygon
                Mat matMask = new Mat(new Drawing.Size(cvImage0.Cols, cvImage0.Rows), DepthType.Cv8U, 3);
                // Polygone skalieren und generieren
                List<Point> scaledPoints = GetScaledPoints(polygonPoints, cvImage0.Rows, cvImage0.Cols);
                List<Drawing.Point> scaledDrawingPoints = GetPolygonPoints(scaledPoints, cvImage0.Rows, cvImage0.Cols);
                // Polygon weiss zeichnen
                using (VectorOfPoint vPoint = new VectorOfPoint(scaledDrawingPoints.ToArray()))
                using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
                {
                    CvInvoke.FillPoly(matMask, vvPoint, new Bgr(255, 255, 255).MCvScalar);
                }
                Image<Gray, byte> imageMask = new Image<Gray, byte>(matMask.Bitmap);

                // Durchschnittsfarbe rechnen mit Maske
                Bgr result0 = cvImage0.GetAverage(imageMask);
                Bgr result1 = cvImage1.GetAverage(imageMask);
                // Resultat abspeichern
                polygon.BgrSnow = JsonConvert.SerializeObject(result0);
                polygon.BgrNormal = JsonConvert.SerializeObject(result1);
                dataContext.SubmitChanges();
            }
        }

        public short Calculate(string imageFilePath, Polygon polygon, Media.PointCollection pointCollection)
        {
            // Maskiertes Bild laden
            Drawing.Bitmap maskedBitmap = GetMaskedBitmap(imageFilePath, pointCollection);

            Image<Bgr, byte> cvImage = new Image<Bgr, byte>(maskedBitmap);

            // Maske generieren aus Polygon
            Mat matMask = new Mat(new Drawing.Size(cvImage.Cols, cvImage.Rows), DepthType.Cv8U, 3);
            // Polygone skalieren und generieren
            List<Point> scaledPoints = GetScaledPoints(pointCollection, cvImage.Rows, cvImage.Cols);
            List<Drawing.Point> scaledDrawingPoints = GetPolygonPoints(scaledPoints, cvImage.Rows, cvImage.Cols);
            // Polygon weiss zeichnen
            using (VectorOfPoint vPoint = new VectorOfPoint(scaledDrawingPoints.ToArray()))
            using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
            {
                CvInvoke.FillPoly(matMask, vvPoint, new Bgr(255, 255, 255).MCvScalar);
            }
            Image<Gray, byte> imageMask = new Image<Gray, byte>(matMask.Bitmap);

            // Durchschnittsfarbe rechnen mit Maske
            Bgr result = cvImage.GetAverage(imageMask);
            // Vergleichen mit Referenzbildern
            Bgr snow = JsonConvert.DeserializeObject<Bgr>(polygon.BgrSnow);
            Bgr normal = JsonConvert.DeserializeObject<Bgr>(polygon.BgrNormal);

            double resultSnow = Math.Abs(snow.Blue - result.Blue) + Math.Abs(snow.Green - result.Green) + Math.Abs(snow.Red - result.Red);
            double resultNormal = Math.Abs(normal.Blue - result.Blue) + Math.Abs(normal.Green - result.Green) + Math.Abs(normal.Red - result.Red);

            if (Math.Abs(resultSnow - resultNormal) < 10)
            {
                return 0;
            }
            else if (resultSnow < resultNormal)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
