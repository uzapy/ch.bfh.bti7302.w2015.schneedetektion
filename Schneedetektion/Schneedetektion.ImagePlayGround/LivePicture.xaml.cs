using Schneedetektion.Data;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Schneedetektion.ImagePlayGround
{
    public partial class LivePicture : UserControl
    {
        #region Fields
        private static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register("ImageSize", typeof(Double), typeof(LivePicture));
        private static string folderName = Settings.Default.PresentationFolder;
        private static int counter = 0;

        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        private ObservableCollection<string> cameras = new ObservableCollection<string>();
        private string selectedCamera = String.Empty;

        private WebClient webClient = new WebClient();

        private List<Data.Image> downloadedImages = new List<Data.Image>();
        private List<Data.Image> differenceImages = new List<Data.Image>();
        private ObservableCollection<Data.Image> shownImages = new ObservableCollection<Data.Image>();
        private Data.Image result;
        private Data.Image resultMask;

        private DispatcherTimer timer = new DispatcherTimer();

        OpenCVHelper openCVHelper = new OpenCVHelper();
        #endregion

        public LivePicture()
        {
            InitializeComponent();
            
            webClient.Headers["Cookie"] = "PHPSESSID=6dook56psrptp83461mh3mpip4";

            Cameras.ItemsSource = cameras;
            foreach (var c in dataContext.Cameras.Select(c => c.Name))
            {
                cameras.Add(c);
            }

            ImageContainer.ItemsSource = shownImages;

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        public Double ImageSize
        {
            get { return (Double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        #region Event Handler
        private void Cameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cameras.SelectedItem != null)
            {
                selectedCamera = Cameras.SelectedItem as string;
            }

            ProcessButton.IsEnabled = Cameras.SelectedItem != null;
        }

        private void Process_Button_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                ProcessButton.Content = "Play";
                timer.Stop();
            }
            else
            {
                ProcessButton.Content = "Pause";
                if (downloadedImages.Count < 1)
                {
                    GetLivePicture();
                }
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value++;
            if (ProgressBar.Value == 10)
            {
                GetLivePicture();
                ProgressBar.Value = 0;
            }
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            // Bilder entfernen
            downloadedImages.Clear();
            shownImages.Clear();

            // Alle Dateien löschen
            //DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
            //foreach (FileInfo file in directoryInfo.GetFiles())
            //{
            //    file.Delete();
            //}

            // Counter zurücksetzen
            counter = 0;

            ProgressBar.Value = 0;
            ProcessButton.Content = "Play";
            timer.Stop();
        }
        #endregion

        #region Methods
        private void GetLivePicture()
        {
            // Livebild herunterladen
            string fileName = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_" + counter + ".jpg";
            webClient.DownloadFile("http://www.astramobcam.ch/kamera/" + selectedCamera + "/live.jpg", fileName);

            // Heruntergeladenes Bild hinzufügen
            Data.Image downloadedImage = new Data.Image(fileName);
            downloadedImage.Counter = counter;
            downloadedImage.Brush = Brushes.Green;
            downloadedImages.Add(downloadedImage);
            shownImages.Add(downloadedImage);

            counter++;
            ProcessPictures();
        }

        private void ProcessPictures()
        {
            if (downloadedImages.Count > 1)
            {
                //if (result == null || resultMask == null)
                //{
                //    result = new Data.Image()
                //}
                //result.Bitmap = openCVHelper.GetMaskedImage(differenceImage.Bitmap, image0.Bitmap);
                //resultMask.Bitmap = diffCandidates.First().DifferenceImage.Bitmap;

                // Letzte 2 Bilder auswählen
                Data.Image image0 = downloadedImages[downloadedImages.Count - 2];
                Data.Image image1 = downloadedImages.Last();

                string difference = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_" +
                    image0.Counter + "+" + image1.Counter + ".png";

                // Differenz ausrechnen
                openCVHelper.CalculateAbsoluteDifference(image0.FilePath, image1.FilePath, difference);

                // Differenz speichern
                Data.Image newImage = new Data.Image(difference);
                newImage.Coverage = openCVHelper.CountBlackArea(difference);
                newImage.Brush = Brushes.Yellow;
                differenceImages.Add(newImage);
                shownImages.Add(newImage);

                // Löcher im ersten Bild auffüllen
                if (differenceImages.Count > 1)
                {
                    // Get Masked Image (einmalig)
                    // Get First Mask

                    Data.Image difference0 = differenceImages[downloadedImages.Count - 2];
                    Data.Image difference1 = differenceImages.Last();
                    string filledNewImagePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_" + counter + ".png";

                    openCVHelper.FillMaskHoles(difference0.FilePath, difference1.FilePath, image1.FilePath, downloadedImages.First().FilePath, filledNewImagePath);

                    Data.Image filledNewImage = new Data.Image(filledNewImagePath);
                    filledNewImage.Counter = counter;
                    filledNewImage.Brush = Brushes.Orange;
                    // downloadedImages.Add(filledNewImage);
                    shownImages.Add(filledNewImage);
                    counter++;
                }
            }
        }
        #endregion
    }
}
