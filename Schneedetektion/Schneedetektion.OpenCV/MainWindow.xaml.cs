using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Schneedetektion.OpenCV
{
    public partial class MainWindow : Window
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        OpenCVHelper openCVHelper = new OpenCVHelper();
        string imagePath = @"C:\Users\uzapy\Desktop\astra\mvk120\20150307\mvk120_20150307_104002.jpg";

        public MainWindow()
        {
            InitializeComponent();

            // Polygon Laden
            Polygon polygon = dataContext.Polygons.Where(p => p.CameraName == "mvk120" && p.ImageArea == "Lane").FirstOrDefault();
            PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);

            maskedImage.Source = openCVHelper.GetMaskedImage(imagePath, pointCollection);
        }
    }
}
