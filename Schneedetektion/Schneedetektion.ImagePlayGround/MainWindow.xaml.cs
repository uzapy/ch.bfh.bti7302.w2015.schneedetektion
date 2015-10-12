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
	   private IEnumerable<Image> imageNames = new List<Image>();
	   private ObservableCollection<string> cameraNames = new ObservableCollection<string>() { "all" };//, "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131" };
	   private ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();
	   private DispatcherTimer timer = new DispatcherTimer();

	   private int year = 2014;
	   private int month = 12;
	   private int day = 2;
	   private bool hasDate = false;
	   private int hour = 0;
	   private int minute = 0;
	   private bool hasTime = false;
	   private List<string> selectedCameras = new List<string>() { "all" };

	   public MainWindow()
	   {
		  InitializeComponent();
		  listBox.ItemsSource = cameraNames;
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

	   private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	   {
		  selectedCameras.Clear();

		  if (listBox.SelectedItems.Count == 0 || listBox.SelectedItems.OfType<string>().Contains("all"))
		  {
			 selectedCameras.Add("all");
		  }
		  else
		  {
			 selectedCameras = listBox.SelectedItems.OfType<string>().ToList();
		  }

		  ReloadImages();
	   }

	   private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	   {
		  timeLapesImage.Source = images[(int)slider1.Value];
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
		  foreach (Image imageName in imageNames)
		  {
			 images.Add(GetBitmap(imageName));
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
