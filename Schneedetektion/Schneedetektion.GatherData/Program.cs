using Schneedetektion.GatherData.Properties;
using System;
using System.IO;

namespace Schneedetektion.GatherData
{
    public class Program
    {
	   private static string folderName = Settings.Default.WorkingFolder;
	   private static string[] cameraNames = { "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131" };
	   private static StrassenbilderMetaDataSetTableAdapters.ImagesTableAdapter imagesTableAdapter =
		  new StrassenbilderMetaDataSetTableAdapters.ImagesTableAdapter();

	   static void Main(string[] args)
	   {
		  foreach (var cameraName in cameraNames)
		  {
			 Console.WriteLine(cameraName);

			 string folder = folderName + "\\" + cameraName;

			 foreach (var subFolder in Directory.GetDirectories(folder))
			 {
				Console.WriteLine(subFolder);

				foreach (var image in Directory.GetFiles(subFolder))
				{
				    string fileName = Path.GetFileNameWithoutExtension(image);
				    int year = Int32.Parse(fileName.Substring(7, 4));
				    int month = Int32.Parse(fileName.Substring(11, 2));
				    int day = Int32.Parse(fileName.Substring(13, 2));
				    int hour = Int32.Parse(fileName.Substring(16, 2));
				    int minutes = Int32.Parse(fileName.Substring(18, 2));
				    int seconds = Int32.Parse(fileName.Substring(20, 2));
				    imagesTableAdapter.Insert(fileName, cameraName, new DateTime(year, month, day, hour, minutes, seconds));

                    }
			 }
		  }

		  Console.WriteLine("Finished!");
	   }
    }
}
