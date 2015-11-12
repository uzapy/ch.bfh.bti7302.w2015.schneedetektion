using Schneedetektion.Data.Properties;
using System;
using System.Windows.Media.Imaging;

namespace Schneedetektion.Data
{
    public partial class Image
    {
        private static string folderName = Settings.Default.WorkingFolder;

        private BitmapImage bitmap;
        public BitmapImage Bitmap
        {
            get
            {
                if (bitmap == null)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(folderName + "\\" + Place + "\\" + Name.Substring(7, 8) + "\\" + Name + ".jpg"));
                    }
                    catch (Exception)
                    {
                        bitmap = new BitmapImage();
                    }
                }
                return bitmap;
            }
            set
            {
                bitmap = value;
            }
        }

        public string ResultIconSource
        {
            get
            {
                switch (Snow)
                {
                    case -1:
                        return "resources/cross.png";
                    case 1:
                        return "resources/tick.png";
                    case 0:
                    default:
                        return "resources/minus.png";
                }
            }
        }
    }
}
