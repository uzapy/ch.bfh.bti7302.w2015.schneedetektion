using Schneedetektion.GatherData.Properties;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace Schneedetektion.GatherData
{
    public class Program
    {
        private static string folderName = Settings.Default.WorkingFolder;
        private static List<string> cameraNames = new List<string>(); // "mvk021", "mvk101", "mvk105", "mvk107", "mvk110", "mvk120", "mvk122", "mvk131"
        private static StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();

        static void Main(string[] args)
        {
            cameraNames = dataContext.Cameras.Select(c => c.Name).ToList();

            // RegisterImagesInDB();
            UpdateDateTime();
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

        private static void UpdateDateTime()
        {
            foreach (string cameraName in cameraNames)
            {
                Console.WriteLine(cameraName);

                string folder = folderName + "\\" + cameraName;

                foreach (var subFolder in Directory.GetDirectories(folder))
                {
                    Console.WriteLine(subFolder);

                    foreach (var imageName in Directory.GetFiles(subFolder))
                    {
                        Console.WriteLine(imageName);
                        string fileName = Path.GetFileNameWithoutExtension(imageName);
                        Image image = dataContext.Images.Where(i => i.Name == fileName).FirstOrDefault();

                        string fileContent = Encoding.ASCII.GetString(File.ReadAllBytes(imageName));


                        if (!string.IsNullOrEmpty(fileContent))
                        {
                            string[] splitFileContent = fileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                            string dat = splitFileContent.Where(sfc => sfc.StartsWith("DAT")).FirstOrDefault().Split('=').Last();
                            string tim = splitFileContent.Where(sfc => sfc.StartsWith("TIM")).FirstOrDefault().Split('=').Last();
                            string tzn = splitFileContent.Where(sfc => sfc.StartsWith("TZN")).FirstOrDefault().Split('=').Last();
                            string tit = splitFileContent.Where(sfc => sfc.StartsWith("TIT")).FirstOrDefault().Split('=').Last();

                            try
                            {
                                DateTime dateTime = DateTime.ParseExact((dat + " " + tim), "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                image.DateTime = dateTime;
                                image.TimeZone = tzn;
                                image.UnixTime = double.Parse(tit, CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine(dat + " " + tim + " " + tzn + " " + tit);
                            }
                        }
                        else
                        {
                            Console.WriteLine("File deleted: " + imageName);
                            //File.Delete(imageName);
                            File.Move(imageName, @"C:\Users\uzapy\Desktop\astra\meta\delete candidates\" + Path.GetFileName(imageName));
                            dataContext.Images.DeleteOnSubmit(image);
                        }

                    }
                }

                dataContext.SubmitChanges();

                Console.WriteLine("Finished!");
            }
        }
    }
}
