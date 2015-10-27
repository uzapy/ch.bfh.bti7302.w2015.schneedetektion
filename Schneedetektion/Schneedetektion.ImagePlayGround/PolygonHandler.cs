using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shapes = System.Windows.Shapes;

namespace Schneedetektion.ImagePlayGround
{
    internal class PolygonHandler
    {
        private StrassenbilderMetaDataContext dataContext = new StrassenbilderMetaDataContext();
        private Shapes.Polygon polygon;
        private Canvas polygonCanvas;
        private string selectedCamera = string.Empty;
        private int selectedAreaIndex = 0;

        private string[] imageAreas = new string[] { "Lane", "Emergency Lane", "Median", "Marking", "Grass", "Tree", "Background", "Sky" };
        private Brush[] fillBrushes = { Brushes.Blue, Brushes.Red, Brushes.Yellow, Brushes.Brown, Brushes.Violet, Brushes.Orange, Brushes.Magenta, Brushes.Gold };
        private Brush[] strokeBrushes = { Brushes.LightBlue, Brushes.OrangeRed, Brushes.Khaki, Brushes.Sienna, Brushes.Pink, Brushes.SandyBrown, Brushes.Orchid, Brushes.Wheat };

        public PolygonHandler(Canvas polygonCanvas)
        {
            this.polygonCanvas = polygonCanvas;
        }

        public string[] ImageAreas { get { return imageAreas; } }

        internal void newPolygon(Image selectedImage, int areaIndex)
        {
            polygonCanvas.Children.Remove(polygon);
            selectedCamera = selectedImage.Place;
            selectedAreaIndex = areaIndex;

            polygon = new Shapes.Polygon();
            polygon.Stroke = fillBrushes[selectedAreaIndex];
            polygon.Fill = strokeBrushes[selectedAreaIndex];
            polygon.Opacity = 0.33d;
            polygonCanvas.Children.Add(polygon);
        }

        internal void loadSavedPolygons(Image selectedImage)
        {
            selectedCamera = selectedImage.Place;

            polygonCanvas.Children.Clear();
            var dbPolygons = dataContext.Polygons.Where(p => p.CameraName == selectedCamera);
            foreach (Polygon dbPolygon in dbPolygons)
            {
                Shapes.Polygon polygon = new Shapes.Polygon();
                polygon.Stroke = fillBrushes[Array.IndexOf(imageAreas, dbPolygon.ImageArea)];
                polygon.Fill = strokeBrushes[Array.IndexOf(imageAreas, dbPolygon.ImageArea)];
                polygon.Opacity = 0.33d;
                polygonCanvas.Children.Add(polygon);
                foreach (Point point in JsonConvert.DeserializeObject<PointCollection>(dbPolygon.PolygonPointCollection))
                {
                    polygon.Points.Add(new Point(point.X * dbPolygon.ImageWidth.Value, point.Y * dbPolygon.ImageHeight.Value));
                }
            }
        }

        internal void setSelectedArea(int areaIndex)
        {
            if (polygon != null)
            {
                selectedAreaIndex = areaIndex;
                polygon.Stroke = fillBrushes[selectedAreaIndex];
                polygon.Fill = strokeBrushes[selectedAreaIndex];
            }
        }

        internal void setPoint(Point point)
        {
            if (polygon != null)
            {
                polygon.Points.Add(point);
            }
        }

        internal void deleteLastPoint()
        {
            if (polygon.Points.Count > 0)
            {
                polygon.Points.RemoveAt(polygon.Points.Count - 1);
            }
        }

        internal void savePolygon(double imageWidth, double imageHeight)
        {
            Polygon p = new Polygon();
            p.CameraName = selectedCamera;
            p.ImageArea = imageAreas[selectedAreaIndex];
            p.ImageWidth = imageWidth;
            p.ImageHeight = imageHeight;

            PointCollection pointCollection = new PointCollection();

            foreach (Point point in polygon.Points)
            {
                pointCollection.Add(new Point(1 / imageWidth * point.X, 1 / imageHeight * point.Y));
            }

            p.PolygonPointCollection = JsonConvert.SerializeObject(pointCollection);

            dataContext.Polygons.InsertOnSubmit(p);
            dataContext.SubmitChanges();
        }
    }
}