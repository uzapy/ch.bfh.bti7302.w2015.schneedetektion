using Newtonsoft.Json;
using Schneedetektion.Data;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private static string sourcefolderName = Settings.Default.WorkingFolder;
        private static string folderName = Settings.Default.PresentationFolder;
        private static int counter = 0;

        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        private ObservableCollection<string> cameras = new ObservableCollection<string>();
        private List<Polygon> polygons = new List<Polygon>();
        private string selectedCamera = String.Empty;

        private WebClient webClient = new WebClient();

        private List<Data.Image> downloadedImages = new List<Data.Image>();
        private List<Data.Image> differenceImages = new List<Data.Image>();
        private List<Data.Image> maskedImages = new List<Data.Image>();
        private ObservableCollection<Data.Image> shownImages = new ObservableCollection<Data.Image>();
        private Data.Image candidate;
        private Data.Image candidateMask;

        private DispatcherTimer timer = new DispatcherTimer();
        private OpenCVHelper openCVHelper = new OpenCVHelper();
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
                polygons = dataContext.Polygons.Where(p => p.CameraName == selectedCamera).ToList();
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
            differenceImages.Clear();
            shownImages.Clear();
            candidate = null;
            candidateMask = null;
            polygons.Clear();
            maskedImages.Clear();

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
            string downloadedFilePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_" + counter + ".jpg";

            // Live
            //webClient.DownloadFile("http://www.astramobcam.ch/kamera/" + selectedCamera + "/live.jpg", downloadedFilePath);
            // Offline
            File.Copy(Directory.GetFiles(sourcefolderName + "\\live\\" + selectedCamera + "\\20160113\\")[counter], downloadedFilePath);

            // Heruntergeladenes Bild hinzufügen
            Data.Image downloadedImage = new Data.Image(downloadedFilePath);
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
                // Letzte 2 Bilder auswählen
                Data.Image image0 = downloadedImages[downloadedImages.Count - 2];
                Data.Image image1 = downloadedImages.Last();

                string differencePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_" +
                    image0.Counter + "+" + image1.Counter + ".png";

                // Differenz ausrechnen
                openCVHelper.CalculateAbsoluteDifference(image0.FilePath, image1.FilePath, differencePath);

                // Differenz speichern
                Data.Image difference = new Data.Image(differencePath);
                difference.Coverage = openCVHelper.CountBlackArea(differencePath);
                difference.Brush = Brushes.Yellow;
                differenceImages.Add(difference);
                shownImages.Add(difference);

                if (candidate == null || candidateMask == null)
                {
                    // Kandidat speichern
                    string candidatePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_C.png";
                    openCVHelper.GetMaskedImage(downloadedImages.First().FilePath, differenceImages.First().FilePath, candidatePath);
                    candidate = new Data.Image(candidatePath);
                    candidate.Coverage = openCVHelper.CountBlackArea(candidatePath);
                    candidate.Brush = Brushes.Cyan;
                    shownImages.Add(candidate);

                    // 1. Kandidaten Maske speichern
                    string candidateMaskPath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_M.png";
                    File.Copy(differenceImages.First().FilePath, candidateMaskPath);
                    candidateMask = new Data.Image(candidateMaskPath);
                    candidateMask.Coverage = openCVHelper.CountBlackArea(candidateMask.FilePath);
                    candidateMask.Brush = Brushes.Magenta;
                    shownImages.Add(candidateMask);
                }

                if (differenceImages.Count > 1)
                {
                    // Löcher im ersten Bild auffüllen
                    Data.Image lastDifference = differenceImages.Last();
                    string filledNewImagePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_C.png";

                    openCVHelper.FillMaskHoles(candidateMask.FilePath, lastDifference.FilePath,
                        downloadedImages.Last().FilePath, candidate.FilePath, filledNewImagePath);

                    candidate = new Data.Image(filledNewImagePath);
                    candidate.Coverage = openCVHelper.CountBlackArea(filledNewImagePath);
                    candidate.Brush = Brushes.Cyan;
                    shownImages.Add(candidate);

                    // Kandidaten Maske aktualisieren
                    string candidateMaskPath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_M.png";

                    openCVHelper.GetBlackArea(candidate.FilePath, candidateMask.FilePath, candidateMaskPath);

                    candidateMask = new Data.Image(candidateMaskPath);
                    candidateMask.Coverage = openCVHelper.CountBlackArea(candidateMask.FilePath);
                    candidateMask.Brush = Brushes.Magenta;
                    shownImages.Add(candidateMask);
                }

                if (candidateMask.Coverage < 1)
                {
                    ProgressBar.Value = 0;
                    timer.Stop();
                    DetectSnow();
                }
            }
        }

        private void DetectSnow()
        {
            // Bild in Regionen aueinanderschneiden
            foreach (Polygon polygon in polygons)
            {
                PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
                
                string maskedImagePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_C_" + polygon.ImageArea + ".png";
                openCVHelper.GetMaskedImage(candidate.FilePath, pointCollection, maskedImagePath);

                Data.Image maskedImage = new Data.Image(maskedImagePath);
                maskedImage.Brush = Brushes.RosyBrown;
                if (!string.IsNullOrEmpty(polygon.BgrSnow) && !string.IsNullOrEmpty(polygon.BgrNormal))
                {
                    maskedImage.Snow = openCVHelper.Calculate(maskedImage.FilePath, polygon, pointCollection);
                    maskedImage.ShowResult = true;
                }
                maskedImages.Add(maskedImage);
                shownImages.Add(maskedImage);
            }


            string resultFilePath = folderName + "\\" + selectedCamera + "_" + DateTime.Now.ToString("yyyMMdd_HHmmss") + "_result.jpg";
            File.Copy(candidate.FilePath, resultFilePath);
            Data.Image result = new Data.Image(resultFilePath);
            result.Brush = Brushes.Gold;

            int snow = maskedImages.Count(i => i.Snow == 1);
            int notSnow = maskedImages.Count(i => i.Snow == 0);
            int dontKnow = maskedImages.Count(i => i.Snow == -1);
            if (snow > notSnow && snow >= dontKnow) result.Snow = 1;
            if (notSnow > snow && notSnow >= dontKnow) result.Snow = -1;

            result.ShowResult = true;
            shownImages.Add(result);
        }
        #endregion
    }
}
