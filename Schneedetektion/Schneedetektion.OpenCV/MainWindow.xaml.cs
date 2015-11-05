using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System;

namespace Schneedetektion.OpenCV
{
    public partial class MainWindow : Window
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        OpenCVHelper openCVHelper = new OpenCVHelper();
        string imagePath = @"C:\Users\uzapy\Desktop\astra\mvk120\20141204\mvk120_20141204_084001.jpg";

        public MainWindow()
        {
            InitializeComponent();

            ApplyMask();
        }

        private void ApplyMask()
        {
            Polygon polygon = dataContext.Polygons.Where(p => p.CameraName == "mvk120" && p.ImageArea == "Lane").FirstOrDefault();
            PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);

            maskedImage.Source = openCVHelper.GetMaskedImage(imagePath, pointCollection);
        }
    }
}
