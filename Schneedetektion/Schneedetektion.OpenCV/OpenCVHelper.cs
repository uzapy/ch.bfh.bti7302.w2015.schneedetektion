﻿using Emgu.CV;
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
        public double BlackPercentage;

        public BitmapImage GetMaskedImage(string imagePath, IList<Point> pointCollection)
        {
            Drawing.Bitmap bitmap = GetMaskedBitmap(imagePath, pointCollection);

            return BitmapToBitmapImage(bitmap);
        }

        public void GetMaskedImage(string imagePath, IList<Point> pointCollection, string resultPath)
        {
            Drawing.Bitmap bitmap = GetMaskedBitmap(imagePath, pointCollection);

            bitmap.Save(resultPath);
        }

        public BitmapImage GetMaskedImage(BitmapImage bitmapMask, BitmapImage bitmapImage)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage));
            Image<Bgr, byte> mask = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapMask));
            Image<Bgr, byte> result = new Image<Bgr, byte>(new byte[288, 352, 3]);

            CvInvoke.cvCopy(image, result, mask);

            return BitmapToBitmapImage(result.Bitmap);
        }

        public void GetMaskedImage(string imagePath, string maskPath, string resultPath)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath);
            Image<Bgr, byte> mask = new Image<Bgr, byte>(maskPath);
            Image<Bgr, byte> result = new Image<Bgr, byte>(new byte[288, 352, 3]);

            CvInvoke.cvCopy(image, result, mask);

            result.Save(resultPath);
        }

        public BitmapImage FillMaskHoles(BitmapImage imageMask0, BitmapImage imageMask1, BitmapImage newImage, BitmapImage incompleteImage)
        {
            Image<Bgr, byte> mask0 = new Image<Bgr, byte>(BitmapImageToBitmap(imageMask0));
            Image<Bgr, byte> mask1 = new Image<Bgr, byte>(BitmapImageToBitmap(imageMask1));
            Image<Bgr, byte> new0 = new Image<Bgr, byte>(BitmapImageToBitmap(newImage));
            Image<Bgr, byte> incomplete = new Image<Bgr, byte>(BitmapImageToBitmap(incompleteImage));

            Image<Bgr, byte> resultMask = new Image<Bgr, byte>(new byte[288, 352, 3]);

            for (int i = 0; i < mask0.Cols; i++)
            {
                for (int j = 0; j < mask0.Rows; j++)
                {
                    if (mask0.Data[j, i, 0] == 0 && mask0.Data[j, i, 1] == 0 && mask0.Data[j, i, 2] == 0 &&
                        mask1.Data[j, i, 0] == 255 && mask1.Data[j, i, 1] == 255 && mask1.Data[j, i, 2] == 255)
                    {
                        resultMask.Data[j, i, 0] = 0;
                        resultMask.Data[j, i, 1] = 0;
                        resultMask.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        resultMask.Data[j, i, 0] = 255;
                        resultMask.Data[j, i, 1] = 255;
                        resultMask.Data[j, i, 2] = 255;
                    }
                }
            }
            resultMask._Not();

            CvInvoke.cvCopy(new0, incomplete, resultMask);

            return BitmapToBitmapImage(incomplete.Bitmap);
        }

        public void FillMaskHoles(string maskPath0, string maskPath1, string newImagePath, string incompleteImagePath, string resultPath)
        {
            Image<Bgr, byte> mask0 = new Image<Bgr, byte>(maskPath0);
            Image<Bgr, byte> mask1 = new Image<Bgr, byte>(maskPath1);
            Image<Bgr, byte> new0 = new Image<Bgr, byte>(newImagePath);
            Image<Bgr, byte> incomplete = new Image<Bgr, byte>(incompleteImagePath);

            Image<Bgr, byte> resultMask = new Image<Bgr, byte>(new byte[288, 352, 3]);

            // Neue Maske: Die unterschiede der beiden Input-Masken
            for (int i = 0; i < mask0.Cols; i++)
            {
                for (int j = 0; j < mask0.Rows; j++)
                {
                    if (mask0.Data[j, i, 0] == 0 && mask0.Data[j, i, 1] == 0 && mask0.Data[j, i, 2] == 0 &&
                        mask1.Data[j, i, 0] == 255 && mask1.Data[j, i, 1] == 255 && mask1.Data[j, i, 2] == 255)
                    {
                        resultMask.Data[j, i, 0] = 0;
                        resultMask.Data[j, i, 1] = 0;
                        resultMask.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        resultMask.Data[j, i, 0] = 255;
                        resultMask.Data[j, i, 1] = 255;
                        resultMask.Data[j, i, 2] = 255;
                    }
                }
            }
            resultMask._Not();

            CvInvoke.cvCopy(new0, incomplete, resultMask);

            incomplete.Save(resultPath);
        }

        public BitmapImage GetBlackArea(BitmapImage bitmap0, BitmapImage bitmap1)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmap0));
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmap1));

            Image<Bgr, byte> resultMask = new Image<Bgr, byte>(new byte[288, 352, 3]);

            for (int i = 0; i < image0.Cols; i++)
            {
                for (int j = 0; j < image0.Rows; j++)
                {
                    if (image0.Data[j, i, 0] == 0 && image0.Data[j, i, 1] == 0 && image0.Data[j, i, 2] == 0 && 
                        !(image1.Data[j, i, 0] == 255 && image1.Data[j, i, 1] == 255 && image1.Data[j, i, 2] == 255))
                    {
                        resultMask.Data[j, i, 0] = 0;
                        resultMask.Data[j, i, 1] = 0;
                        resultMask.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        resultMask.Data[j, i, 0] = 255;
                        resultMask.Data[j, i, 1] = 255;
                        resultMask.Data[j, i, 2] = 255;
                    }
                }
            }

            return BitmapToBitmapImage(resultMask.Bitmap);
        }

        public void GetBlackArea(string imagePath, string maskPath, string resultPath)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath);
            Image<Bgr, byte> mask = new Image<Bgr, byte>(maskPath);

            Image<Bgr, byte> resultMask = new Image<Bgr, byte>(new byte[288, 352, 3]);

            for (int i = 0; i < image.Cols; i++)
            {
                for (int j = 0; j < image.Rows; j++)
                {
                    if (image.Data[j, i, 0] == 0 && image.Data[j, i, 1] == 0 && image.Data[j, i, 2] == 0 &&
                        !(mask.Data[j, i, 0] == 255 && mask.Data[j, i, 1] == 255 && mask.Data[j, i, 2] == 255))
                    {
                        resultMask.Data[j, i, 0] = 0;
                        resultMask.Data[j, i, 1] = 0;
                        resultMask.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        resultMask.Data[j, i, 0] = 255;
                        resultMask.Data[j, i, 1] = 255;
                        resultMask.Data[j, i, 2] = 255;
                    }
                }
            }

            resultMask.Save(resultPath);
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
                bitmap.Save(stream, ImageFormat.Bmp);

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
            result1._ThresholdBinaryInv(new Bgr(50, 50, 50), new Bgr(255, 255, 255));
            CvInvoke.CvtColor(result1, result1, ColorConversion.Bgr2Gray);

            return BitmapToBitmapImage(result1.Bitmap);
        }

        public BitmapImage CalculateAbsoluteDifference(BitmapImage bitmapImage0, BitmapImage bitmapImage1)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage0));
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage1));

            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            result1 = image0.AbsDiff(image1);
            result1._ThresholdBinaryInv(new Bgr(50, 50, 50), new Bgr(255, 255, 255));
            // CvInvoke.CvtColor(result1, result1, ColorConversion.Bgr2Gray);

            //result1._Dilate(1); // Macht die schwarze Fläche kleiner
            result1._Erode(3); // Macht die schwarze Fläche grösser

            for (int i = 0; i < result1.Cols; i++)
            {
                for (int j = 0; j < result1.Rows; j++)
                {
                    if (result1.Data[j, i, 0] < 50 && result1.Data[j, i, 1] < 50 && result1.Data[j, i, 2] < 50)
                    {
                        result1.Data[j, i, 0] = 0;
                        result1.Data[j, i, 1] = 0;
                        result1.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        result1.Data[j, i, 0] = 255;
                        result1.Data[j, i, 1] = 255;
                        result1.Data[j, i, 2] = 255;
                    }
                }
            }

            return BitmapToBitmapImage(result1.Bitmap);
        }

        public void CalculateAbsoluteDifference(string imagePath0, string imagePath1, string resultPath)
        {
            // Bilder einlesen
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(imagePath0);
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(imagePath1);

            // Neues, leeres Bild erzeugen
            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            // Absolute Differenz der beiden Bilder berechnen
            result1 = image0.AbsDiff(image1);
            // Threshholden und Invertieren
            result1._ThresholdBinaryInv(new Bgr(50, 50, 50), new Bgr(255, 255, 255));
            // Schwarze Fläche grösser machen per Erosions-Operation
            result1._Erode(3);

            // Für alle Farbkanäle gleich
            for (int i = 0; i < result1.Cols; i++)
            {
                for (int j = 0; j < result1.Rows; j++)
                {
                    if (result1.Data[j, i, 0] < 50 && result1.Data[j, i, 1] < 50 && result1.Data[j, i, 2] < 50)
                    {
                        result1.Data[j, i, 0] = 0;
                        result1.Data[j, i, 1] = 0;
                        result1.Data[j, i, 2] = 0;
                    }
                    else
                    {
                        result1.Data[j, i, 0] = 255;
                        result1.Data[j, i, 1] = 255;
                        result1.Data[j, i, 2] = 255;
                    }
                }
            }

            // Differenz der Bilder in neuem Bild abspeichern
            result1.Save(resultPath);
        }

        public BitmapImage CalculateIntesection(BitmapImage bitmapImage0, BitmapImage bitmapImage1)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage0));
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage1));

            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            CvInvoke.BitwiseOr(image0, image1, result1);

            BlackPercentage = CountBlackArea(result1);

            return BitmapToBitmapImage(result1.Bitmap);
        }

        public BitmapImage CalculateAverage(string imagePath0, string imagePath1)
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>(imagePath0);
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(imagePath1);

            Image<Bgr, byte> result1 = new Image<Bgr, byte>(new byte[288, 352, 3]);

            CvInvoke.AddWeighted(image0, 0.5, image1, 0.5, 0.0, result1);

            return BitmapToBitmapImage(result1.Bitmap);
        }

        public BitmapImage IntersectMasks(BitmapImage bitmapImage0, BitmapImage bitmapImage1)
        {
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage0));
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage1));

            Image<Bgr, byte> result = new Image<Bgr, byte>(new byte[288, 352, 3]);

            CvInvoke.BitwiseOr(image1, image2, result);

            return BitmapToBitmapImage(result.Bitmap);
        }

        public double CountBlackArea(BitmapImage bitmapImage)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage));
            return CountBlackArea(image);
        }

        private double CountBlackArea(Image<Bgr, byte> image)
        {
            int total = image.Cols * image.Rows;
            int black = 0;
            for (int i = 0; i < image.Cols; i++)
            {
                for (int j = 0; j < image.Rows; j++)
                {
                    if (image.Data[j, i, 0] < 10 && image.Data[j, i, 1] < 10 && image.Data[j, i, 2] < 10) black++;
                }
            }
            return 100d / (double)total * (double)black;
        }

        public double CountBlackArea(string filePath)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(filePath);
            
            int total = image.Cols * image.Rows;
            int black = 0;
            for (int i = 0; i < image.Cols; i++)
            {
                for (int j = 0; j < image.Rows; j++)
                {
                    if (image.Data[j, i, 0] < 10 && image.Data[j, i, 1] < 10 && image.Data[j, i, 2] < 10) black++;
                }
            }
            return 100d / (double)total * (double)black;
        }

        public BitmapImage CopyEmptyAreasToBase(BitmapImage bitmapImage0, BitmapImage bitmapImage1, BitmapImage bitmapImageMask)
        {
            Image<Bgr, byte> image1 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage0));
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImage1));
            Image<Bgr, byte> mask = new Image<Bgr, byte>(BitmapImageToBitmap(bitmapImageMask));
            mask._Not();

            CvInvoke.cvCopy(image2, image1, mask);

            return BitmapToBitmapImage(image1.Bitmap);
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
            // Drawing.Bitmap maskedBitmap = GetMaskedBitmap(imageFilePath, pointCollection);

            Image<Bgr, byte> cvImage = new Image<Bgr, byte>(imageFilePath);

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

            double distanceToSnow = Math.Sqrt(
                Math.Pow(snow.Blue - result.Blue, 2) +
                Math.Pow(snow.Green - result.Green, 2) +
                Math.Pow(snow.Red - result.Red, 2));
            double distanceToNotSnow = Math.Sqrt(
                Math.Pow(normal.Blue - result.Blue, 2) +
                Math.Pow(normal.Green - result.Green, 2) +
                Math.Pow(normal.Red - result.Red, 2));

            if (Math.Abs(distanceToSnow - distanceToNotSnow) < 10)
            {
                return 0;
            }
            else if (distanceToSnow < distanceToNotSnow)
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

