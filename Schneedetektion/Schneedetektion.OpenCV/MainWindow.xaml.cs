using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Media = System.Windows.Media;

namespace Schneedetektion.OpenCV
{
    public partial class MainWindow : Window
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        Random random = new Random((int)(DateTime.Now.Ticks));

        public MainWindow()
        {
            InitializeComponent();

            Mat matrix = new Mat(@"C:\Users\uzapy\Desktop\astra\mvk120\20150307\mvk120_20150307_104002.jpg", LoadImageType.AnyColor);
            UMat uMatrix = matrix.ToUMat(AccessType.ReadWrite);

            // Polygon Laden
            Polygon polygon = dataContext.Polygons.Where(p => p.CameraName == "mvk120" && p.ImageArea == "Lane").FirstOrDefault();
            Media.PointCollection pointCollection = JsonConvert.DeserializeObject<Media.PointCollection>(polygon.PolygonPointCollection);
            Drawing.Point[] polygonPoints = new Drawing.Point[pointCollection.Count + 4];

            // Punkte laden und herunterskalieren
            for (int i = 0; i < pointCollection.Count; i++)
            {
                double x = pointCollection[i].X;
                double y = pointCollection[i].Y;

                x = x / polygon.ImageHeight.Value*0.77;
                y = y / polygon.ImageWidth.Value;

                x = x * uMatrix.Rows;
                y = y * uMatrix.Cols;

                polygonPoints[i] = new Drawing.Point((int)x, (int)y);
            }

            polygonPoints[pointCollection.Count + 0] = new Drawing.Point(0, uMatrix.Rows);
            polygonPoints[pointCollection.Count + 1] = new Drawing.Point(0, 0);
            polygonPoints[pointCollection.Count + 2] = new Drawing.Point(uMatrix.Cols, 0);
            polygonPoints[pointCollection.Count + 3] = new Drawing.Point(uMatrix.Cols, uMatrix.Rows);

            using (VectorOfPoint vPoint = new VectorOfPoint(polygonPoints))
            using (VectorOfVectorOfPoint vvPoint = new VectorOfVectorOfPoint(vPoint))
            {
                CvInvoke.FillPoly(uMatrix, vvPoint,
                    new Bgr(random.NextDouble() * 120, random.NextDouble() * 120, random.NextDouble() * 120).MCvScalar);
            }

            // Bitmap zu BitmapImage machen
            maskedImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                uMatrix.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    uMatrix.Bitmap.Save(stream, ImageFormat.Jpeg);
            //    stream.Position = 0;
            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.StreamSource = stream;
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.EndInit();
            //    maskedImage.Source = bitmapImage;
            //}
        }
    }
}
