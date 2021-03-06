﻿using Schneedetektion.Data;
using Schneedetektion.ImagePlayGround.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Schneedetektion.ImagePlayGround
{
    public partial class MainWindow : Window
    {
        #region Fields
        public static string folderName = Settings.Default.WorkingFolder;
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();

        private ObservableCollection<string> cameraNames = new ObservableCollection<string>() { "all" };
        private List<string> selectedCameras = new List<string>() { "all" };
        private List<string> selectedMasks = new List<string>();
        private ObservableCollection<Data.Image> images = new ObservableCollection<Data.Image>();
        private Data.Image selectedImage;
        private ObservableCollection<Data.Image> removeCarsGroup = new ObservableCollection<Data.Image>();
        private ObservableCollection<BitmapImage> resultsGroup = new ObservableCollection<BitmapImage>();

        private DispatcherTimer timer = new DispatcherTimer();

        private PolygonHelper polygonHelper;
        private ImageHelper imageHelper = new ImageHelper();

        private int year = 2014;
        private int month = 12;
        private int day = 2;
        private bool hasDate = false;
        private int hour = 0;
        private int minute = 0;
        private bool hasTime = false;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            listBox1.ItemsSource = cameraNames;
            imageContainer.ItemsSource = images;
            resultsContainer.ItemsSource = resultsGroup;
            removeCarsContainer.ItemsSource = removeCarsGroup;

            foreach (Camera camera in dataContext.Cameras)
            {
                cameraNames.Add(camera.Name);
            }

            foreach (Data.Image i in dataContext.Images.Where(i => i.Place == cameraNames[1]).Take(265))
            {
                images.Add(i);
            }

            timeLapesImage.Source = images.First().Bitmap;
            slider1.Maximum = images.Count - 1;

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100 Milisekunden => 10fps

            polygonHelper = new PolygonHelper(polygonCanvas);
            selectedArea.ItemsSource = polygonHelper.ImageAreas;
            selectedArea.SelectedItem = selectedArea.Items[0];

            listBox2.ItemsSource = polygonHelper.ImageAreas;
        }
        #endregion

        #region Properties
        public bool HasSelectedMasks { get { return selectedMasks.Count > 0; } }
        #endregion

        #region Event Handler
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            hasDate = datePicker.SelectedDate.HasValue;

            if (datePicker.SelectedDate.HasValue)
            {
                year = datePicker.SelectedDate.Value.Year;
                month = datePicker.SelectedDate.Value.Month;
                day = datePicker.SelectedDate.Value.Day;
            }
        }

        private void timePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            hasTime = timePicker.Value.HasValue;

            if (timePicker.Value.HasValue)
            {
                hour = timePicker.Value.Value.Hour;
                minute = timePicker.Value.Value.Minute;
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCameras.Clear();

            if (listBox1.SelectedItems.Count == 0 || listBox1.SelectedItems.OfType<string>().Contains("all"))
            {
                selectedCameras.Add("all");
            }
            else
            {
                selectedCameras = listBox1.SelectedItems.OfType<string>().ToList();
            }
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            timeLapesImage.Source = images[(int)slider1.Value].Bitmap;
        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedMasks.Clear();
            selectedMasks.AddRange(listBox2.SelectedItems.OfType<string>().ToList());
            imageHelper.ActiveMasks = selectedMasks;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ReloadImages();
        }

        private void imageContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageContainer.SelectedIndex >= 0)
            {
                if (selectedImage?.Place != images[imageContainer.SelectedIndex].Place)
                {
                    selectedImage = images[imageContainer.SelectedIndex];
                    polygonHelper.loadSavedPolygons(selectedImage, maskToolImage.ActualWidth, maskToolImage.ActualHeight);
                    selectedCameraName.Text = "Camera: " + selectedImage.Place;
                }

                removeCarsGroup.Clear();
                resultsGroup.Clear();
                removeCarsGroup.Add(images[imageContainer.SelectedIndex]);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                else
                {
                    timer.Start();
                }
            }
            else if (e.Key == Key.Left)
            {
                slider1.Value--;
            }
            else if (e.Key == Key.Right)
            {
                slider1.Value++;
            }
            else if (e.Key == Key.Enter)
            {
                ReloadImages();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            slider1.Value++;
            if ((slider1.Value + 1) >= images.Count)
            {
                slider1.Value = 0;
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private void SelectedArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            polygonHelper.setSelectedArea(selectedArea.SelectedIndex);
        }

        private void newPolygon_Click(object sender, RoutedEventArgs e)
        {
            polygonHelper.newPolygon(selectedImage, selectedArea.SelectedIndex);
        }

        private void savePolygon_Click(object sender, RoutedEventArgs e)
        {
            polygonHelper.savePolygon(maskToolImage.ActualWidth, maskToolImage.ActualHeight);
        }

        private void deletePoint_Click(object sender, RoutedEventArgs e)
        {
            polygonHelper.deleteLastPoint();
        }

        private void maskToolImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imageWidth.Text = "W: " + maskToolImage.ActualWidth.ToString("0.00");
            imageHeight.Text = "H: " + maskToolImage.ActualHeight.ToString("0.00");

            polygonHelper.loadSavedPolygons(selectedImage, maskToolImage.ActualWidth, maskToolImage.ActualHeight);
        }

        private void polygonCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            pointerXPosition.Text = "X: " + e.GetPosition(maskToolImage).X.ToString("0:00");
            pointerYPosition.Text = "Y: " + e.GetPosition(maskToolImage).Y.ToString("0:00");
        }

        private void polygonCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            polygonHelper.setPoint(e.GetPosition(maskToolImage));
        }

        private void step_Click(object sender, RoutedEventArgs e)
        {
            removeCarsGroup.Add(GetNextImage(removeCarsGroup.Last()));
            IEnumerable<BitmapImage> results = imageHelper.ApplyNext(removeCarsGroup);
            foreach (BitmapImage result in results)
            {
                resultsGroup.Add(result);
            }
            removeCarsGroup.FirstOrDefault().Bitmap = resultsGroup.LastOrDefault();
            blackPercentage.Text = "Mask: " + (imageHelper.BlackPercentage > 0 ? imageHelper.BlackPercentage.ToString("0.00") : "???") + "%";
        }
        #endregion

        #region Helper Methods
        private void ReloadImages()
        {
            images.Clear();

            TimeSpan minuteSpan = new TimeSpan(0, 16, 0);
            DateTime exactTime = new DateTime(year, month, day, hour, minute, 0);

            var loadedImages = (from i in dataContext.Images
                                where i.DateTime.Year == year || !hasDate
                                where i.DateTime.Month == month || !hasDate
                                where i.DateTime.Day == day || !hasDate
                                where i.DateTime.Hour == hour || !hasTime
                                where Math.Abs(i.DateTime.Minute - minute) < 6 || !hasTime
                                where selectedCameras.Contains(i.Place) || selectedCameras.Contains("all")
                                select i).Distinct().Take(512);

            foreach (Data.Image i in loadedImages)
            {
                images.Add(imageHelper.ApplyMask(i));
            }

            slider1.Maximum = images.Count - 1;
            timeLapesImage.Source = images[(int)slider1.Value].Bitmap;
        }

        private Data.Image GetNextImage(Data.Image image)
        {
            return (from i in dataContext.Images
                    where i.Place == image.Place
                    where i.UnixTime > image.UnixTime
                    orderby i.UnixTime ascending
                    select i).FirstOrDefault();
        }
        #endregion
    }
}
