using Newtonsoft.Json;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Schneedetektion.Data;

namespace Schneedetektion.ImagePlayGround
{
    internal class ImageMask
    {
        OpenCVHelper openCVHelper = new OpenCVHelper();
        private static string folderName = Settings.Default.WorkingFolder;
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        private List<Polygon> savedPolygons = new List<Polygon>();
        public List<string> activeMasks = new List<string>();

        public List<string> ActiveMasks
        {
            get
            {
                return activeMasks;
            }
            internal set
            {
                activeMasks = value;
                savedPolygons = dataContext.Polygons.Where(p => activeMasks.Contains(p.ImageArea)).ToList();
            }
        }

        internal Image ApplyMask(Image image)
        {
            string imageFilePath = folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8) + "\\" + image.Name + ".jpg";
            Polygon polygon = savedPolygons.Where(p => p.CameraName == image.Place).FirstOrDefault();

            if (polygon != null)
            {
                PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
                image.Bitmap = openCVHelper.GetMaskedImage(imageFilePath, pointCollection);
            }

            if (!string.IsNullOrEmpty(polygon.BgrSnow) && ! string.IsNullOrEmpty(polygon.BgrNormal))
            {
                PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
                image.Snow = openCVHelper.Calculate(imageFilePath, polygon, pointCollection);
            }

            return image;
        }
    }
}