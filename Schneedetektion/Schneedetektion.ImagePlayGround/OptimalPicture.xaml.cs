using Schneedetektion.ImagePlayGround.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Schneedetektion.ImagePlayGround
{
    public partial class OptimalPicture : UserControl
    {
        private static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register("ImageSize", typeof(Double), typeof(OptimalPicture));
        private static string burstFolder = Settings.Default.BurstFolder;
        private ObservableCollection<string> folders = new ObservableCollection<string>();
        private string selectedFolder;
        private ObservableCollection<DateTime> days = new ObservableCollection<DateTime>();
        private DateTime selectedDay;
        private List<Data.Image> images = new List<Data.Image>();
        private ObservableCollection<Data.Image> selectedImages = new ObservableCollection<Data.Image>();
        private ImageHelper imageHelper = new ImageHelper();

        public OptimalPicture()
        {
            InitializeComponent();

            Cameras.ItemsSource = folders;
            Days.ItemsSource = days;
            ImageContainer.ItemsSource = selectedImages;

            if (Directory.Exists(burstFolder))
            {
                string[] fullFolderNames = Directory.GetDirectories(burstFolder);
                foreach (string f in fullFolderNames)
                {
                    folders.Add(new DirectoryInfo(f).Name);
                } 
            }
        }

        public Double ImageSize
        {
            get { return (Double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        private void Camera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cameras.SelectedItem != null)
            {
                selectedFolder = burstFolder + "\\" + Cameras.SelectedItem;

                images.Clear();
                foreach (string imageFileName in Directory.GetFiles(selectedFolder))
                {
                    images.Add(new Data.Image(imageFileName, burstFolder));
                }

                days.Clear();
                var dates = (from i in images select i.DateTime.Date).Distinct();
                foreach (var date in dates)
                {
                    days.Add(date);
                }
            }
        }

        private void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Days.SelectedItem != null)
            {
                selectedDay = (DateTime)Days.SelectedItem;

                selectedImages.Clear();
                var filteredImages = images.Where(i => i.DateTime.Year == selectedDay.Year &&
                    i.DateTime.Month == selectedDay.Month && i.DateTime.Day == selectedDay.Day);
                foreach (var image in filteredImages)
                {
                    selectedImages.Add(image);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Data.Image> parameter = new List<Data.Image>();
            foreach (Data.Image i in selectedImages)
            {
                parameter.Add(i);
            }
            IEnumerable<Data.Image> allDifferences = imageHelper.GetAllDifferences2(parameter);
            selectedImages.Clear();
            foreach (var difference in allDifferences)
            {
                selectedImages.Add(difference);
            }
        }
    }
}
