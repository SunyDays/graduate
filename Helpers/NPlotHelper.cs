using System;
using NPlot;
using Helpers;

namespace Helpers
{
    public static class NPlotHelper
    {
//        public static LinePlot CreatePlot(double[] y)
//        {
//            return new LinePlot()
//            {
////                AbscissaData = x,
//                OrdinateData = y
//            };
//        }
//
//        public static NPlot.Gtk.PlotSurface2D CreatePlotSurface(IDrawable plot)
//        {
//            var plotSurface = new NPlot.Gtk.PlotSurface2D();
//            plotSurface.Clear();
//
//            plotSurface.Padding = 40;
//            plotSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
//            plotSurface.Add(new Grid { HorizontalGridType = Grid.GridType.None, VerticalGridType = Grid.GridType.Fine});
//            plotSurface.Add(plot);
////            plotSurface.YAxis1.Label = "Frequency";
//            plotSurface.Refresh();
//            plotSurface.Show();
//
//            return plotSurface;
//        }

        public static HistogramPlot CreateHistogramPlot(double[] frequency)
        {
            return new HistogramPlot()
            {
                OrdinateData = frequency,
                RectangleBrush = RectangleBrushes.Solid.Black,
                Color = System.Drawing.Color.Black,
                Filled = true,
                BaseWidth = 0.5f,
            };
        }

        public static HistogramPlot CreateHistogramPlot(double[] randArray, int min, int max, int n)
        {
            return CreateHistogramPlot(HistogramHelper.ComputeFrequency(randArray, min, max, n));
        }

        public static NPlot.Gtk.PlotSurface2D CreatePlotSurface(IDrawable plot)
        {
            var plotSurface = new NPlot.Gtk.PlotSurface2D();
            plotSurface.Clear();

            plotSurface.Padding = 40;
            plotSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            plotSurface.Add(new Grid { HorizontalGridType = Grid.GridType.None, VerticalGridType = Grid.GridType.Fine});
            plotSurface.Add(plot);
            plotSurface.YAxis1.Label = "Frequency";
            plotSurface.Refresh();
            plotSurface.Show();

            return plotSurface;
        }
    }
}