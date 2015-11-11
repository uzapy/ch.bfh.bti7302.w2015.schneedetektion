using System;
using System.Windows.Media.Imaging;

namespace Schneedetektion.ImagePlayGround
{
    public partial class Image
    {
        private BitmapImage bitmap;
        public BitmapImage Bitmap
        {
            get
            {
                if (bitmap == null)
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(MainWindow.folderName + "\\" + Place + "\\" + Name.Substring(7, 8) + "\\" + Name + ".jpg"));
                    }
                    catch (Exception)
                    {
                        bitmap = new BitmapImage();
                    }
                }
                return bitmap;
            }
            internal set
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
