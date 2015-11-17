using Newtonsoft.Json;
using Schneedetektion.Data;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schneedetektion.ImagePlayGround
{
    internal class ImageHelper
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
            string imageFilePath = GetFilePath(image);
            Polygon polygon = savedPolygons.Where(p => p.CameraName == image.Place).FirstOrDefault();

            if (polygon != null)
            {
                PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
                image.Bitmap = openCVHelper.GetMaskedImage(imageFilePath, pointCollection);

                if (!string.IsNullOrEmpty(polygon.BgrSnow) && !string.IsNullOrEmpty(polygon.BgrNormal))
                {
                    image.Snow = openCVHelper.Calculate(imageFilePath, polygon, pointCollection);
                }
            }
            else
            {
                image.Bitmap = new BitmapImage(new Uri(imageFilePath));
            }

            return image;
        }

        internal ImageSource ApplyNext(ObservableCollection<Image> removeCarsGroup, ImageSource currentMask)
        {
            string imageFilePath0 = GetFilePath(removeCarsGroup.Last());
            IList<string> files = Directory.GetFiles(GetDirectory(removeCarsGroup.Last()));
            string imageFilePath1 = files.ElementAt(files.IndexOf(imageFilePath0) + 1);

            Image image1 = dataContext.Images.Where(i => i.Name == Path.GetFileNameWithoutExtension(imageFilePath1)).FirstOrDefault();
            removeCarsGroup.Add(image1);

            // TODO: Maske aktualisieren

            return openCVHelper.CalculateAbsoluteDifference(imageFilePath0, imageFilePath1);
        }

        private string GetFilePath(Image image)
        {
            return folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8) + "\\" + image.Name + ".jpg";
        }

        private string GetDirectory(Image image)
        {
            return folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8);
        }
    }
}