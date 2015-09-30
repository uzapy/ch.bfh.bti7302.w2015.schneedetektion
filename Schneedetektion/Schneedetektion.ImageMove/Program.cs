using System;
using System.IO;

namespace Schneedetektion.FileMove
{
    public class Program
    {
        private static string folderName = @"C:\Users\uzapy\Desktop\astra\mvk131";
        private static string folderNameLog = @"C:\Users\uzapy\Desktop\astra\log_mvk131";
        private static string fileName = String.Empty;
        private static string[] fileNames;

        static void Main(string[] args)
        {
            if (!Directory.Exists(folderNameLog))
            {
                Directory.CreateDirectory(folderNameLog);
            }

            fileNames = Directory.GetFiles(folderName);
            for (int i = 0; i < fileNames.Length-1; i++)
            {
                if (fileNames[i].EndsWith(".log"))
                {
                    fileName = Path.GetFileName(fileNames[i]);
                    File.Move(fileNames[i], folderNameLog + "\\" + fileName);
                    Console.WriteLine(folderNameLog + "\\" + fileName);
                }
            }
            Console.WriteLine("Finished!");
        }
    }
}
