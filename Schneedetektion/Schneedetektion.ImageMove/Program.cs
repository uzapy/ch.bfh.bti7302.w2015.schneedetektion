using Schneedetektion.ImageMove.Properties;
using System;
using System.IO;

namespace Schneedetektion.FileMove
{
    public class Program
    {
	   /* Bilder in Unterordner verschieben (Unterordner nach Tag) */
	   private static string folderName = Settings.Default.WorkingFolder;
	   private static string[] cameraNames = { "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131" };
	   private static string[] fileNames;

	   static void Main(string[] args)
	   {
		  foreach (string cameraName in cameraNames)
		  {
			 string folder = folderName + "\\" + cameraName;
			 fileNames = Directory.GetFiles(folder);

			 foreach (string file in fileNames)
			 {
				string fileName = Path.GetFileNameWithoutExtension(file);
				string subFolder = folderName + "\\" +  cameraName + "\\" + fileName.Substring(7, 8);

				if (!Directory.Exists(subFolder))
				{
				    Directory.CreateDirectory(subFolder);
				}

				File.Move(file, subFolder + "\\" + Path.GetFileName(file));
				Console.WriteLine(subFolder + "\\" + Path.GetFileName(file));

			 }

			 Console.WriteLine(cameraName);
		  }
		  Console.WriteLine("Finished!");
		  Console.ReadLine();
	   }

	   /* Bilder und Log-Dateien trennen */
	   //private static string folderName = @"C:\Users\uzapy\Desktop\astra";
	   //private static string folderNameLog = @"C:\Users\uzapy\Desktop\astra\log_mvk131";
	   //private static string fileName = String.Empty;
	   //private static string[] fileNames;

	   //static void Main(string[] args)
	   //{
	   //    if (!Directory.Exists(folderNameLog))
	   //    {
	   //        Directory.CreateDirectory(folderNameLog);
	   //    }

	   //    fileNames = Directory.GetFiles(folderName);
	   //    for (int i = 0; i < fileNames.Length-1; i++)
	   //    {
	   //        if (fileNames[i].EndsWith(".log"))
	   //        {
	   //            fileName = Path.GetFileName(fileNames[i]);
	   //            File.Move(fileNames[i], folderNameLog + "\\" + fileName);
	   //            Console.WriteLine(folderNameLog + "\\" + fileName);
	   //        }
	   //    }
	   //    Console.WriteLine("Finished!");
	   //}

    }
}
