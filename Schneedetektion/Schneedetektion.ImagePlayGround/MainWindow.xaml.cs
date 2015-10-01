using Schneedetektion.ImagePlayGround.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Schneedetektion.ImagePlayGround
{
    public partial class MainWindow : Window
    {
	   private static string folderName = Settings.Default.WorkingFolder;
	   private static string[] cameraNames = { "all", "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131" };
	   private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
	   private IEnumerable<Image> imageNames = new List<Image>();
	   private ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();

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
		  DataContext = this;
		  imageContainer.ItemsSource = images;
		  listBox.ItemsSource = cameraNames;

		  imageNames = dataContext.Images.Where(i => i.Place == cameraNames[1]).Take(265);

		  foreach (Image imageName in imageNames)
		  {
			 images.Add(GetBitmap(imageName));
		  }

        }

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
	   }
    }
}
