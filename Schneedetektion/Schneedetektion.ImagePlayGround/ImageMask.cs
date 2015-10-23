using Newtonsoft.Json;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schneedetektion.ImagePlayGround
{
    internal class ImageMask
    {
        OpenCVHelper openCVHelper = new OpenCVHelper();
        private static string folderName = Settings.Default.WorkingFolder;
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        public List<string> activeMasks = new List<string>();
        private List<Polygon> savedPolygons = new List<Polygon>();

        public List<string> ActiveMasks
        {
            get { return activeMasks; }
            internal set
            {
                activeMasks = value;
                savedPolygons = dataContext.Polygons.Where(p => activeMasks.Contains(p.ImageArea)).ToList();
            }
        }

        internal BitmapImage ApplyMasks(Image imageName)
        {
            string imageFilePath = folderName + "\\" + imageName.Place + "\\" + imageName.Name.Substring(7, 8) + "\\" + imageName.Name + ".jpg";
            Polygon polygon = savedPolygons.Where(p => p.CameraName == imageName.Place).FirstOrDefault();
            PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);

            return openCVHelper.GetMaskedImage(imageFilePath, pointCollection);
        }
    }
}