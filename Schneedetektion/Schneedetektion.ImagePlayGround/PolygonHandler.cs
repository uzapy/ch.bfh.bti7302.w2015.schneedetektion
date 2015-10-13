using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Schneedetektion.ImagePlayGround
{
    internal class PolygonHandler
    {
	   private StrassenbilderMetaDataContext dataContext;
	   private Polygon polygon;
	   private Canvas polygonCanvas;
	   private string selectedCamera = string.Empty;
	   private double imageWidth = 0;
	   private double imageHeight = 0;

	   private string[] imageAreas = new string[] { "Lane", "Emergency Lane", "Median", "Marking", "Grass", "Tree", "Background", "Sky" };
	   private Brush[] fillBrushes = { Brushes.Blue , Brushes.Red, Brushes.Yellow, Brushes.Brown, Brushes.Violet, Brushes.Orange, Brushes.Magenta, Brushes.Gold };
	   private Brush[] strokeBrushes = { Brushes.LightBlue , Brushes.OrangeRed, Brushes.Khaki, Brushes.Sienna, Brushes.Pink, Brushes.SandyBrown, Brushes.Orchid, Brushes.Wheat };

	   public PolygonHandler(StrassenbilderMetaDataContext dataContext, Canvas polygonCanvas)
	   {
		  this.dataContext = dataContext;
		  this.polygonCanvas = polygonCanvas;
	   }

	   public string[] ImageAreas { get { return imageAreas; } }
	   public Brush[] ImageAreaBrushes { get { return fillBrushes; } }

	   internal void newPolygon(Image selectedImage, int selectedAreaIndex, double width, double height)
	   {
		  polygonCanvas.Children.Clear();

		  selectedCamera = selectedImage.Place;
		  imageWidth = width;
		  imageHeight = height;

		  polygon = new Polygon();
		  polygon.Stroke = fillBrushes[selectedAreaIndex];
		  polygon.Fill = strokeBrushes[selectedAreaIndex];
		  polygon.Opacity = 0.33d;
		  polygonCanvas.Children.Add(polygon);
	   }

	   internal void setSelectedArea(int selectedAreaIndex)
	   {
		  if (polygon != null)
		  {
			 polygon.Stroke = fillBrushes[selectedAreaIndex];
			 polygon.Fill = strokeBrushes[selectedAreaIndex]; 
		  }
	   }

	   internal void setPoint(Point point)
	   {
		  polygon.Points.Add(point);
        }

	   internal void deleteLastPoint()
	   {
		  if (polygon.Points.Count > 0)
		  {
			 polygon.Points.RemoveAt(polygon.Points.Count - 1); 
		  }
	   }

	   internal void savePolygon()
	   {
		  throw new NotImplementedException();
	   }
    }
}