using Schneedetektion.ImagePlayGround.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;

namespace Schneedetektion.ImagePlayGround
{
    internal class ImageMask
    {
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
                // TODO: Nach Kamera filtern
            }
        }

        internal BitmapImage ApplyMasks(Image imageName)
        {
            Drawing.Image image = Drawing.Image.FromFile(folderName + "\\" + imageName.Place + "\\" + imageName.Name.Substring(7, 8) + "\\" + imageName.Name + ".jpg");
            //byte[] pixels;
            BitmapImage bitmap = new BitmapImage();

            //using (MemoryStream stream = new MemoryStream())
            //{
            //	image.Save(stream, Drawing.Imaging.ImageFormat.Jpeg);
            //	pixels = stream.ToArray();
            //}

            //         if (pixels != null && pixels.Length > 0)
            //         {
            //             for (int i = 0; i < pixels.Length - 1; i++)
            //             {
            //                 pixels[i] = 55;
            //             }
            //         }

            //         using (MemoryStream stream = new MemoryStream(pixels))
            //         {
            //             bitmap.BeginInit();
            //             bitmap.CacheOption = BitmapCacheOption.OnLoad;
            //             bitmap.StreamSource = stream;
            //             bitmap.EndInit();
            //         }

            return bitmap;
        }
    }
}