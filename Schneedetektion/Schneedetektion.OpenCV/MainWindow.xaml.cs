using Newtonsoft.Json;
using Schneedetektion.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Schneedetektion.OpenCV
{
    public partial class MainWindow : Window
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        OpenCVHelper openCVHelper = new OpenCVHelper();
        string imagePath = @"C:\Users\uzapy\Desktop\astra\mvk120\20141204\mvk120_20141204_084001.jpg";

        string imagePath0 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_080002.jpg";
        string imagePath1 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_081001.jpg";
        //string imagePath2 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_082002.jpg";
        //string imagePath3 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_083001.jpg";
        //string imagePath4 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_084001.jpg";
        //string imagePath5 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141204\mvk101_20141204_085001.jpg";

        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk021\20141229\mvk021_20141229_142002.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk021\20150316\mvk021_20150316_142001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk101\20141227\mvk101_20141227_083001.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk101\20150520\mvk101_20150520_073001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk105\20141227\mvk105_20141227_090001.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk105\20150513\mvk105_20150513_080001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk107\20150201\mvk107_20150201_082002.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk107\20150419\mvk107_20150419_070001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk110\20141228\mvk110_20141228_080001.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk110\20150522\mvk110_20150522_071001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk120\20150201\mvk120_20150201_080001.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk120\20150604\mvk120_20150604_071001.jpg"; // Not Snow
        //string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk122\20141228\mvk122_20141228_082001.jpg"; // Snow
        //string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk122\20150422\mvk122_20150422_120001.jpg"; // Not Snow
        string reference0 = @"C:\Users\uzapy\Desktop\astra\mvk131\20141228\mvk131_20141228_153001.jpg"; // Snow
        string reference1 = @"C:\Users\uzapy\Desktop\astra\mvk131\20150608\mvk131_20150608_114001.jpg"; // Not Snow


        public MainWindow()
        {
            InitializeComponent();

            //ApplyMask();

            //CalculateAverage();

            CalculateAverageBrightessForArea();
        }

        private void ApplyMask()
        {
            Polygon polygon = dataContext.Polygons.Where(p => p.CameraName == "mvk120" && p.ImageArea == "Lane").FirstOrDefault();
            PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
            maskedImage.Source = openCVHelper.GetMaskedImage(imagePath, pointCollection);
        }

        private void CalculateAverage()
        {
            //IList<string> images = new List<string>() { imagePath0, imagePath1, imagePath2, imagePath3, imagePath4, imagePath5, };
            maskedImage.Source = openCVHelper.CalculateAbsoluteDifference(imagePath0, imagePath1);
        }

        private void CalculateAverageBrightessForArea()
        {
            openCVHelper.CalculateAverageBrightessForArea(reference0, reference1, dataContext);
        }
    }
}
