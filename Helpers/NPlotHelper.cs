using System;
using NPlot;
using Helpers;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using System.Linq;
using System.Security.Principal;
using System.Drawing;
using System.Reflection;
using System.Security.Cryptography;

namespace Helpers
{
    public static class NPlotHelper
    {
		public static void PlotCharts(IEnumerable<IEnumerable<double>> data, IEnumerable<double> t, IEnumerable<string> labels)
		{
			Application.Init();

			var window = new Window("");
			window.Resize(1366, 768);
			window.Add(CreatePlotSurface(data, t, labels));
			window.ShowAll();
			Application.Run();
		}

		private static NPlot.Gtk.PlotSurface2D CreatePlotSurface(IEnumerable<IEnumerable<double>> data, IEnumerable<double> t, IEnumerable<string> labels)
		{
			var plotSurface = new NPlot.Gtk.PlotSurface2D();
			plotSurface.Clear();

			plotSurface.Padding = 40;
			plotSurface.Add(new Grid { HorizontalGridType = Grid.GridType.Fine, VerticalGridType = Grid.GridType.Fine});

			plotSurface.Legend = new Legend
				{
					YOffset = 16,
					HorizontalEdgePlacement = Legend.Placement.Outside,
					VerticalEdgePlacement = Legend.Placement.Inside
				};
			plotSurface.Legend.AttachTo(PlotSurface2D.XAxisPosition.Bottom, PlotSurface2D.YAxisPosition.Right);

			for (int i = 0; i < data.Count(); i++)
				plotSurface.Add(new LinePlot
					{
						AbscissaData = t.ToArray(),
						OrdinateData = data.ElementAt(i).ToArray(),
						Label = labels.ElementAt(i),
						Pen = new Pen(GetRandomColor(), 2)
					});

			plotSurface.Refresh();
			plotSurface.Show();

			return plotSurface;
		}

		private static readonly Random random = new Random(DateTime.Now.Millisecond);

		private static Color GetRandomColor()
		{
			return Color.FromArgb(255, random.Next(0, 191), random.Next(0, 191), random.Next(0, 191));
		}
    }
}