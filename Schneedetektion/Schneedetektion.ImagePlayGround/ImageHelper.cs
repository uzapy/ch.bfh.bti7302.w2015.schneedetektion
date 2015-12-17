using Newtonsoft.Json;
using Schneedetektion.Data;
using Schneedetektion.ImagePlayGround.Properties;
using Schneedetektion.OpenCV;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schneedetektion.ImagePlayGround
{
    internal class ImageHelper
    {
        OpenCVHelper openCVHelper = new OpenCVHelper();
        private static string folderName = Settings.Default.WorkingFolder;
        private static string burstFolderName = Settings.Default.BurstFolder;
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        private List<Polygon> savedPolygons = new List<Polygon>();
        public List<string> activeMasks = new List<string>();
        private List<Candidate> diffCandidates = new List<Candidate>();
        private Image result = new Image();
        private Image resultMask = new Image();
        private static int counter = 0;

        #region Masking
        public List<string> ActiveMasks
        {
            get
            {
                return activeMasks;
            }
            internal set
            {
                activeMasks = value;
                savedPolygons = dataContext.Polygons.Where(p => activeMasks.Contains(p.ImageArea)).ToList();
            }
        }

        public double BlackPercentage { get { return openCVHelper.BlackPercentage; } }

        internal Image ApplyMask(Image image)
        {
            string imageFilePath = GetFilePath(image);
            Polygon polygon = savedPolygons.Where(p => p.CameraName == image.Place).FirstOrDefault();

            if (polygon != null)
            {
                PointCollection pointCollection = JsonConvert.DeserializeObject<PointCollection>(polygon.PolygonPointCollection);
                image.Bitmap = openCVHelper.GetMaskedImage(imageFilePath, pointCollection);

                if (!string.IsNullOrEmpty(polygon.BgrSnow) && !string.IsNullOrEmpty(polygon.BgrNormal))
                {
                    image.Snow = openCVHelper.Calculate(imageFilePath, polygon, pointCollection);
                }
            }
            else
            {
                image.Bitmap = new BitmapImage(new Uri(imageFilePath));
            }

            return image;
        }
        #endregion

        #region Car clean up
        internal ImageSource ApplyNext(ObservableCollection<Image> removeCarsGroup, List<BitmapImage> removeCarsMasks)
        {
            string imageFilePath0 = GetFilePath(removeCarsGroup.Last());
            IList<string> files = Directory.GetFiles(GetDirectory(removeCarsGroup.Last()));
            string imageFilePath1 = files.ElementAt(files.IndexOf(imageFilePath0) + 1);

            Image image1 = dataContext.Images.Where(i => i.Name == Path.GetFileNameWithoutExtension(imageFilePath1)).FirstOrDefault();
            removeCarsGroup.Add(image1);

            removeCarsMasks.Add(openCVHelper.CalculateAbsoluteDifference(imageFilePath0, imageFilePath1));

            BitmapImage intersectedMask = new BitmapImage();

            if (removeCarsMasks.Count > 1)
            {
                int lastMask = removeCarsMasks.Count - 1;
                intersectedMask = openCVHelper.IntersectMasks(removeCarsMasks[lastMask], removeCarsMasks[lastMask - 1]);
                removeCarsMasks.Add(intersectedMask);
            }

            return removeCarsMasks.Last();
        }

        internal IEnumerable<BitmapImage> ApplyNext(ObservableCollection<Image> removeCarsGroup)
        {
            List<BitmapImage> results = new List<BitmapImage>();

            if (removeCarsGroup.Count() > 2 && (removeCarsGroup.Count() + 1) % 2 == 0)
            {
                // select base image
                string baseImageFilePath = GetFilePath(removeCarsGroup.FirstOrDefault());
                BitmapImage baseImage = removeCarsGroup.FirstOrDefault().Bitmap;
                // select second to last image
                string secondToLastImageFilePath = GetFilePath(removeCarsGroup.ElementAt(removeCarsGroup.Count - 2));
                BitmapImage secondToLastImage = removeCarsGroup.ElementAt(removeCarsGroup.Count - 2).Bitmap;
                // select lat image
                string lastImageFilePath = GetFilePath(removeCarsGroup.ElementAt(removeCarsGroup.Count - 1));
                BitmapImage lastImage = removeCarsGroup.ElementAt(removeCarsGroup.Count - 1).Bitmap;

                // absolute difference between base and second to last
                results.Add(openCVHelper.CalculateAbsoluteDifference(baseImage, secondToLastImage));
                // absolute difference between base and last
                results.Add(openCVHelper.CalculateAbsoluteDifference(baseImage, lastImage));

                // intersection between the two differences
                results.Add(openCVHelper.CalculateIntesection(results.ElementAt(results.Count - 2), results.ElementAt(results.Count - 1)));
                // blend last and second to last images
                results.Add(openCVHelper.CalculateAverage(secondToLastImageFilePath, lastImageFilePath));

                // replace empty areas on base image
                results.Add(openCVHelper.CopyEmptyAreasToBase(removeCarsGroup.FirstOrDefault().Bitmap, results.ElementAt(results.Count - 1), results.ElementAt(results.Count - 2)));
            }

            return results;
        }

        internal IEnumerable<Image> GetAllDifferences(IEnumerable<Image> selectedImages)
        {
            List<Image> results = new List<Image>();

            List<Tuple<Image, Image>> crossJoin = new List<Tuple<Image, Image>>();
            for (int i = 0; i < selectedImages.Count() - 1; i++)
            {
                crossJoin.Add(new Tuple<Image, Image>(selectedImages.ElementAt(i), selectedImages.ElementAt(i + 1)));
            }
            //for (int i = 0; i < selectedImages.Count() - 2; i++)
            //{
            //    crossJoin.Add(new Tuple<Image, Image>(selectedImages.ElementAt(i), selectedImages.ElementAt(i + 2)));
            //}
            //for (int i = 0; i < selectedImages.Count() - 3; i++)
            //{
            //    crossJoin.Add(new Tuple<Image, Image>(selectedImages.ElementAt(i), selectedImages.ElementAt(i + 3)));
            //}

            diffCandidates.Clear();
            foreach (var pair in crossJoin)
            {
                Image differenceImage = new Image(openCVHelper.CalculateAbsoluteDifference(pair.Item1.Bitmap, pair.Item2.Bitmap));
                differenceImage.Name = pair.Item1.Name + "\r\n" + pair.Item2.Name;
                differenceImage.Coverage = openCVHelper.CountBlackArea(differenceImage.Bitmap);
                diffCandidates.Add(new Candidate(pair.Item1, pair.Item2, differenceImage));
            }

            // Beste 10% auswählen
            int tenPercent = (int)Math.Ceiling(diffCandidates.Count * 0.5);
            diffCandidates = diffCandidates.OrderBy(c => c.DifferenceImage.Coverage).Take(tenPercent).ToList();

            // 1. Resultat schreiben
            result.Bitmap = openCVHelper.GetMaskedImage(diffCandidates.First().DifferenceImage.Bitmap, diffCandidates.First().Image0.Bitmap);
            resultMask.Bitmap = diffCandidates.First().DifferenceImage.Bitmap;
            WriteResult(result.Bitmap);

            Directory.GetFiles(@"C:\Users\uzapy\Desktop\astra\result\").ToList().ForEach(f => File.Delete(f));

            // Löcher auffüllen mit der nächsten Maske
            for (int i = 1; i < diffCandidates.Count; i++)
            {
                result.Bitmap = openCVHelper.FillMaskHoles(
                    resultMask.Bitmap, diffCandidates[i].DifferenceImage.Bitmap, diffCandidates[i].Image0.Bitmap, result.Bitmap);
                WriteResult(result.Bitmap);

                resultMask.Bitmap = openCVHelper.GetBlackArea(result.Bitmap);
                //WriteResult(resultMask.Bitmap);

                if (openCVHelper.CountBlackArea(result.Bitmap) < 3) break;
            }

            result.Name = "Result";
            result.Coverage = openCVHelper.CountBlackArea(result.Bitmap);
            results.Add(result);
            results.AddRange(diffCandidates.Select(dc => dc.DifferenceImage));

            return results;
        }
        #endregion

        #region Helper Methodes
        private string GetFilePath(Image image)
        {
            return folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8) + "\\" + image.Name + ".jpg";
        }

        private string GetDirectory(Image image)
        {
            return folderName + "\\" + image.Place + "\\" + image.Name.Substring(7, 8);
        }

        private void WriteResult(BitmapImage bitmapImage)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (FileStream fileStream = new FileStream(@"C:\Users\uzapy\Desktop\astra\result\" + counter++ + ".png", FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        #endregion
    }
}