﻿using Schneedetektion.Data.Properties;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Schneedetektion.Data
{
    public partial class Image
    {
        private static string folderName = Settings.Default.WorkingFolder;

        private BitmapImage bitmap;
        private string imageFileName;

        public Image(string imageFileName, string burstFolder = null)
        {
            this.imageFileName = imageFileName;
            Name = Path.GetFileNameWithoutExtension(imageFileName);
            Place = Path.GetDirectoryName(imageFileName).Split(Path.DirectorySeparatorChar).Last();
            int year = Int32.Parse(Name.Substring(7, 4));
            int month = Int32.Parse(Name.Substring(11, 2));
            int day = Int32.Parse(Name.Substring(13, 2));
            int hour = Int32.Parse(Name.Substring(16, 2));
            int minutes = Int32.Parse(Name.Substring(18, 2));
            int seconds = Int32.Parse(Name.Substring(20, 2));
            DateTime = new DateTime(year, month, day, hour, minutes, seconds);

            if (!String.IsNullOrEmpty(burstFolder))
            {
                folderName = burstFolder;
            }
        }

        public Image(BitmapImage bitmap, string name, string burstFolder = null)
        {
            Bitmap = bitmap;
            Name = name;

            if (!String.IsNullOrEmpty(burstFolder))
            {
                folderName = burstFolder;
            }
        }

        public BitmapImage Bitmap
        {
            get
            {
                if (bitmap == null)
                {
                    try
                    {
                        if (Directory.Exists(folderName + "\\" + Place + "\\" + Name.Substring(7, 8)))
                        {
                            bitmap = new BitmapImage(new Uri(folderName + "\\" + Place + "\\" + Name.Substring(7, 8) + "\\" + Name + ".jpg"));
                        }
                        else
                        {
                            bitmap = new BitmapImage(new Uri(folderName + "\\" + Place + "\\" + Name + ".jpg"));
                        }
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
