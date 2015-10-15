using Schneedetektion.GatherData.Properties;
using System;
using System.IO;

namespace Schneedetektion.GatherData
{
    public class Program
    {
        private static string folderName = Settings.Default.WorkingFolder;
        private static string[] cameraNames = { "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131" };
        private static StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();

        static void Main(string[] args)
        {
            RegisterImagesInDB();
        }

        private static void RegisterImagesInDB()
        {
            foreach (var cameraName in cameraNames)
            {
                Console.WriteLine(cameraName);

                string folder = folderName + "\\" + cameraName;

                foreach (var subFolder in Directory.GetDirectories(folder))
                {
                    Console.WriteLine(subFolder);

                    foreach (var imageName in Directory.GetFiles(subFolder))
                    {
                        Image image = new Image();
                        image.Name = Path.GetFileNameWithoutExtension(imageName);
                        image.Place = cameraName;
                        int year = Int32.Parse(image.Name.Substring(7, 4));
                        int month = Int32.Parse(image.Name.Substring(11, 2));
                        int day = Int32.Parse(image.Name.Substring(13, 2));
                        int hour = Int32.Parse(image.Name.Substring(16, 2));
                        int minutes = Int32.Parse(image.Name.Substring(18, 2));
                        int seconds = Int32.Parse(image.Name.Substring(20, 2));
                        image.DateTime = new DateTime(year, month, day, hour, minutes, seconds);
                        dataContext.Images.InsertOnSubmit(image);
                    }
                }
                dataContext.SubmitChanges();

                Console.WriteLine("Finished!");
            }
        }
    }
}
