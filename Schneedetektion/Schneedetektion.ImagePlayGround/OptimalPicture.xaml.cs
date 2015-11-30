using Schneedetektion.ImagePlayGround.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using Schneedetektion.Data;
using System;
using System.Linq;

namespace Schneedetektion.ImagePlayGround
{
    public partial class OptimalPicture : UserControl
    {
        private static string burstFolder = Settings.Default.BurstFolder;
        private ObservableCollection<string> folders = new ObservableCollection<string>();
        private string selectedFolder;
        private ObservableCollection<DateTime> days = new ObservableCollection<DateTime>();
        private DateTime selectedDay;
        private List<Data.Image> images = new List<Data.Image>();

        public OptimalPicture()
        {
            InitializeComponent();

            Cameras.ItemsSource = folders;
            Days.ItemsSource = days;

            string[] fullFolderNames = Directory.GetDirectories(burstFolder);
            foreach (string f in fullFolderNames)
            {
                folders.Add(new DirectoryInfo(f).Name);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Camera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cameras.SelectedItem != null)
            {
                selectedFolder = burstFolder + "\\" + Cameras.SelectedItem;
                
                foreach (string imageFileName in Directory.GetFiles(selectedFolder))
                {
                    images.Add(new Data.Image(imageFileName));
                }

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
            }
        }
    }
}
