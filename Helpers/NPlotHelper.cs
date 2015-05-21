using System;
using NPlot;
using Helpers;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using System.Linq;

namespace Helpers
{
    public static class NPlotHelper
    {
		public static void PlotChart(IEnumerable<double> y, string label)
		{
			Application.Init();

			var window = new Window("");
			window.Resize(1366, 768);
			window.Add(CreatePlotSurface(y, label));
			window.ShowAll();
			Application.Run();
		}

		private static NPlot.Gtk.PlotSurface2D CreatePlotSurface(IEnumerable<double> y, string label)
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

			plotSurface.Add(new LinePlot{ DataSource = y.ToArray(), Label = label });
			plotSurface.Refresh();
			plotSurface.Show();

			return plotSurface;
		}
    }
}