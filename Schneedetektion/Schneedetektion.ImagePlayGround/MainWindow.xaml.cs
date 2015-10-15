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
	   private static string folderName = Settings.Default.WorkingFolder;
	   private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();

	   private ObservableCollection<string> cameraNames = new ObservableCollection<string>() { "all" };
	   private List<string> selectedCameras = new List<string>() { "all" };
	   private List<string> selectedMasks = new List<string>();
	   private IEnumerable<Image> imageNames = new List<Image>();
	   private ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();
	   private Image selectedImage;

	   private DispatcherTimer timer = new DispatcherTimer();

	   private PolygonHandler polygonHandler;
	   private ImageMask imageMask = new ImageMask();

	   private int year = 2014;
	   private int month = 12;
	   private int day = 2;
	   private bool hasDate = false;
	   private int hour = 0;
	   private int minute = 0;
	   private bool hasTime = false;

	   public MainWindow()
	   {
		  InitializeComponent();
		  listBox1.ItemsSource = cameraNames;
		  imageContainer.ItemsSource = images;

		  #region Initial Load
		  foreach (Camera camera in dataContext.Cameras)
		  {
			 cameraNames.Add(camera.Name);
		  }

		  imageNames = dataContext.Images.Where(i => i.Place == cameraNames[1]).Take(265);

		  foreach (Image imageName in imageNames)
		  {
			 images.Add(GetBitmap(imageName));
		  }

		  timeLapesImage.Source = images.First();
		  slider1.Maximum = images.Count - 1;
		  #endregion

		  timer.Tick += Timer_Tick;
		  timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100 Milisekunden => 10fps

		  polygonHandler = new PolygonHandler(dataContext, polygonCanvas);
		  selectedArea.ItemsSource = polygonHandler.ImageAreas;
		  selectedArea.SelectedItem = selectedArea.Items[0];

		  listBox2.ItemsSource = polygonHandler.ImageAreas;
	   }

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

		  ReloadImages();
	   }

	   private void timePicker_LostFocus(object sender, RoutedEventArgs e)
	   {
		  hasTime = timePicker.Value.HasValue;

		  if (timePicker.Value.HasValue)
		  {
			 hour = timePicker.Value.Value.Hour;
			 minute = timePicker.Value.Value.Minute - (timePicker.Value.Value.Minute % 10);
		  }

		  ReloadImages();
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

		  ReloadImages();
	   }

	   private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	   {
		  timeLapesImage.Source = images[(int)slider1.Value];
	   }

	   private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
	   {
		  selectedMasks.Clear();
		  selectedMasks.AddRange(listBox2.SelectedItems.OfType<string>().ToList());
		  imageMask.ActiveMasks = selectedMasks;
		  ReloadImages();
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
	   }

	   private void Timer_Tick(object sender, EventArgs e)
	   {
		  slider1.Value++;
		  if ((slider1.Value+1) >= images.Count)
		  {
			 slider1.Value = 0;
		  }
		  CommandManager.InvalidateRequerySuggested();
	   }

	   private void imageContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
	   {
		  if (imageContainer.SelectedIndex >= 0)
		  {
			 if (selectedImage?.Place != imageNames.ElementAt(imageContainer.SelectedIndex).Place)
			 {
				selectedImage = imageNames.ElementAt(imageContainer.SelectedIndex);
				polygonHandler.loadSavedPolygons(selectedImage);
				selectedCameraName.Text = "Camera: " + selectedImage.Place;
			 }
		  }
	   }

	   private void SelectedArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
	   {
		  polygonHandler.setSelectedArea(selectedArea.SelectedIndex);
	   }

	   private void newPolygon_Click(object sender, RoutedEventArgs e)
	   {
		  polygonHandler.newPolygon(selectedImage, selectedArea.SelectedIndex);
        }

	   private void savePolygon_Click(object sender, RoutedEventArgs e)
	   {
		  polygonHandler.savePolygon(maskToolImage.ActualWidth, maskToolImage.ActualHeight);
	   }

	   private void deletePoint_Click(object sender, RoutedEventArgs e)
	   {
		  polygonHandler.deleteLastPoint();
	   }

	   private void polygonCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	   {
		  polygonHandler.setPoint(e.GetPosition(polygonCanvas));
	   }
	   #endregion

	   #region Helper Methods
	   private void ReloadImages()
	   {
		  imageNames = (from i in dataContext.Images
					 where i.DateTime.Year == year || !hasDate
					 where i.DateTime.Month == month || !hasDate
					 where i.DateTime.Day == day || !hasDate
					 where i.DateTime.Hour == hour || !hasTime
					 where i.DateTime.Minute == minute || !hasTime
					 where selectedCameras.Contains(i.Place) || selectedCameras.Contains("all")
					 select i).Take(512);

		  images.Clear();
		  if (selectedMasks.Count < 1)
		  {
			 foreach (Image imageName in imageNames)
			 {
				images.Add(GetBitmap(imageName));
			 }
		  }
		  else
		  {
			 foreach (Image imageName in imageNames)
			 {
				images.Add(imageMask.ApplyMasks(imageName));
			 }
		  }

		  slider1.Maximum = images.Count - 1;
		  timeLapesImage.Source = images[(int)slider1.Value];
	   }

	   private BitmapImage GetBitmap(Image image)
	   {
		  try
		  {
			 return new BitmapImage(new Uri(folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8) + "\\" + image.Name + ".jpg"));
		  }
		  catch (Exception)
		  {
			 return new BitmapImage();
		  }
	   }
	   #endregion
    }
}
