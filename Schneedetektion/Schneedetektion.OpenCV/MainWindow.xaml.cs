using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System;
using System.Collections.Generic;

namespace Schneedetektion.OpenCV
{
    public partial class MainWindow : Window
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        OpenCVHelper openCVHelper = new OpenCVHelper();
        string imagePath = @"C:\Users\uzapy\Desktop\astra\mvk120\20141204\mvk120_20141204_084001.jpg";

        string imagePath0 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_075001.jpg";
        string imagePath1 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_080002.jpg";
        string imagePath2 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_081001.jpg";
        string imagePath3 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_082002.jpg";
        string imagePath4 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_083001.jpg";
        string imagePath5 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_084001.jpg";

        public MainWindow()
        {
            InitializeComponent();

            //ApplyMask();

            CalculateAverage();
        }

        private void ApplyMask()
        {
            Polygon polygon = dataContext.Polygons.Where(p => p.CameraName == "mvk120" && p.ImageArea == "Lane").FirstOrDefault();
            PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
            maskedImage.Source = openCVHelper.GetMaskedImage(imagePath, pointCollection);
        }

        private void CalculateAverage()
        {
            IList<string> images = new List<string>() { imagePath0, imagePath1, imagePath2, imagePath3, imagePath4, imagePath5, };
            maskedImage.Source = openCVHelper.CalculateAverage(images);
        }
    }
}
