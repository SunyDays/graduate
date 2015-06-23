﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Helpers;
using Modeling;
using Types;
using System.Text;
using System.IO;
using Gtk;
using System.Collections;
using NPlot;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GLib;

namespace NetModeling
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if(args.ContainsArg("ethernet"))
			{
				EthernetHelp();
				Console.WriteLine();
			}

			try
			{
				var netconfig = args.GetArgValue<string>("netconfig");
				var startNodeIndex = args.GetArgValue<int>("startnode");
				var targetNodeIndex = args.GetArgValue<int>("targetnode");

				var networkModel = new NetworkModel(netconfig, startNodeIndex, targetNodeIndex);

				if(args.ContainsArg("console"))
					ConsoleOutput(networkModel, args.ContainsArg("matrix"));
				else
					ConsoleColorWrite("OK", ConsoleColor.Green);
				if(args.ContainsArg("log"))
					LogOutput(networkModel,
						Path.Combine(Path.GetDirectoryName(netconfig), "logs",
						Path.GetFileNameWithoutExtension(netconfig) + ".log"),
						args.ContainsArg("matrix"));
				if(args.ContainsArg("graph"))
					PlotDensity(networkModel);
			}
			catch(Exception ex)
			{
				ConsoleColorWrite("ERROR:", ConsoleColor.Red);
				Console.WriteLine(ex);
			}

			#if DEBUG
				Console.ReadLine();
			#endif
		}

		public static void PlotDensity(NetworkModel networkModel)
		{
			var t = CreateRange(0, 0.0022, 0.00001);
			var data = new List<Vector<double>>();
			var labels = new List<string>();

			data.Add(new Vector<double>(networkModel.ComputeDensity(networkModel.MinPath, 0, t)));

			if(networkModel.MinPath.Length != networkModel.MaxPath.Length)
			{
				data.Add(new Vector<double>(networkModel.ComputeDensity(networkModel.MaxPath, 0, t)));

				labels.Add(string.Format("Shortest path: {0}",
					networkModel.MinPath.ToString(" -> ")));
				labels.Add(string.Format("Longest path: {0}",
					networkModel.MaxPath.ToString(" -> ")));
			}
			else
				labels.Add(string.Format("Path: {0}",
					networkModel.MinPath.ToString(" -> ")));

			NPlotHelper.PlotCharts(data, t, labels);
		}

		public static IEnumerable<double> CreateRange(double a, double b, double step)
		{
			for (double val = a; val <= b; val += step)
				yield return val;
		}

		public static void ConsoleOutput(NetworkModel networkModel, bool matrixOutput)
		{
			Console.ForegroundColor = ConsoleColor.White;

			ConsoleColorWrite(String.Format("NAME = {0}", networkModel.Name), ConsoleColor.Green);
			ConsoleColorWrite(String.Format("STREAMS COUNT = {0}", networkModel.StreamsCount), ConsoleColor.Green);

			Console.Write(Environment.NewLine);

			for (int stream = 0; stream < networkModel.StreamsCount; stream++)
			{
				if(networkModel.StreamsCount > 1)
					ConsoleColorWrite(String.Format("==================== STREAM {0} ====================", stream),
						ConsoleColor.White);

				if(matrixOutput)
				{
					ConsoleColorWrite("ROUTING MATRIX", ConsoleColor.Green);
					var routingMatrix = networkModel.GetRoutingMatrix(stream);
					for (int i = 0; i < routingMatrix.RowsCount; i++)
					{
						for (int j = 0; j < routingMatrix.ColumnsCount; j++)
							ConsoleColorWrite(string.Format("{0:0.000}\t", routingMatrix[i, j]),
								routingMatrix[i, j] == 0 ? Console.ForegroundColor : ConsoleColor.Yellow, false);

						Console.Write(Environment.NewLine);
					}

					Console.Write(Environment.NewLine);	
				}

				ConsoleColorWrite("LAMBDA\t\t\t\tMU\t\t\t\tRO", ConsoleColor.Green);
				for (int i = 0; i < networkModel.NodesCount; i++)
					Console.WriteLine("{0:0.000000}\t\t\t{1:0.000000}\t\t\t{2:0.000000}",
						networkModel.Lambda[stream][i], networkModel.Mu[stream][i], networkModel.Ro[stream][i]);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("LAMBDA0", ConsoleColor.Green);
				Console.WriteLine(networkModel.Lambda0[stream]);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("E", ConsoleColor.Green);
				for (int i = 0; i < networkModel.E[stream].Length; i++)
					Console.WriteLine("{0:0.000000}", networkModel.E[stream][i]);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("LAMBDA'\t\t\t\tRO'", ConsoleColor.Green);
				for (int i = 0; i < networkModel.NodesCount; i++)
					Console.WriteLine("{0:0.000000}\t\t\t{1:0.000000}",
						networkModel.LambdaBar[stream][i], networkModel.RoBar[stream][i]);

				Console.Write(Environment.NewLine);

				if(networkModel.StreamsCount > 1)
				{
					ConsoleColorWrite("ROTOTAL", ConsoleColor.Green);
					for (int i = 0; i < networkModel.RoTotal.Length; i++)
						Console.WriteLine("{0:0.000000}", networkModel.RoTotal[i]);

					Console.Write(Environment.NewLine);
				}

				ConsoleColorWrite("THE AVERAGE NUMBER OF PATHS", ConsoleColor.Green);
				Console.WriteLine("{0:0.000000}", networkModel.AveragePaths);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("STATIONARY PROBABILITY-TIME CHARACTERISTICS", ConsoleColor.Green);
				ConsoleColorWrite("W", ConsoleColor.Green);
				for (int i = 0; i < networkModel.Ws.Length; i++)
					Console.WriteLine("{0:0.000000}", networkModel.Ws[i]);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("U\t\t\t\tL\t\t\t\tN", ConsoleColor.Green);
				for (int i = 0; i < networkModel.NodesCount; i++)
					Console.WriteLine("{0:0.000000}\t\t\t{1:0.000000}\t\t\t{2:0.000000}",
						networkModel.Us[stream][i], networkModel.Ls[stream][i], networkModel.Ns[stream][i]);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("INTEGRATED PROBABILITY-TIME CHARACTERISTICS", ConsoleColor.Green);

				ConsoleColorWrite(string.Format("PATHS, {0}", networkModel.Paths.Count), ConsoleColor.Green);
				networkModel.Paths.Select((path, index) => path.ToString(" -> ")
					+ " : " + networkModel.TransitionProbabilities[index]).ToList().ForEach(Console.WriteLine);

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("W", ConsoleColor.Green);
				Console.WriteLine(string.Format("{0:0.000000}", networkModel.Wi ));

				Console.Write(Environment.NewLine);

				ConsoleColorWrite("U\t\t\t\tL\t\t\t\tN", ConsoleColor.Green);
				Console.WriteLine("{0:0.000000}\t\t\t{1:0.000000}\t\t\t{2:0.000000}",
					networkModel.Ui[stream], networkModel.Li[stream], networkModel.Ni[stream]);

				Console.Write(Environment.NewLine);

				if(networkModel.StreamsCount > 1)
					ConsoleColorWrite("==================================================", ConsoleColor.White);
			}
		}

		private static void LogOutput(NetworkModel networkModel, string logFile, bool matrixOutput)
		{
			var data = new List<string>();

			data.Add("NAME = " + networkModel.Name);
			data.Add("STREAMS COUNT = " + networkModel.StreamsCount);

			data.Add(Environment.NewLine);

			for (int stream = 0; stream < networkModel.StreamsCount; stream++)
			{
				if(networkModel.StreamsCount > 1)
					data.Add(string.Format("==================== STREAM {0} ====================", stream));

				if(matrixOutput)
				{
					data.Add("ROUTING MATRIX");
					networkModel.GetRoutingMatrix(stream).ForEachRow(row => 
						data.Add(
							row.Aggregate("",
								(accumulate, item) => accumulate + string.Format("{0:0.000}\t", item)
							).Trim()));

					data.Add(Environment.NewLine);	
				}

				data.Add("LAMBDA");
				networkModel.Lambda[stream].ForEach(lambda => data.Add(string.Format("{0:0.000000}", lambda)));

				data.Add(Environment.NewLine);

				data.Add("MU");
				networkModel.Mu[stream].ForEach(mu => data.Add(string.Format("{0:0.000000}", mu)));

				data.Add(Environment.NewLine);

				data.Add("RO");
				networkModel.Ro[stream].ForEach(ro => data.Add(string.Format("{0:0.000000}", ro)));

				data.Add(Environment.NewLine);

				data.Add("LAMBDA0");
				data.Add(networkModel.Lambda0[stream].ToString());

				data.Add(Environment.NewLine);

				data.Add("E");
				networkModel.E[stream].ForEach(e => data.Add(string.Format("{0:0.000000}", e)));

				data.Add(Environment.NewLine);

				data.Add("LAMBDA'");
				networkModel.LambdaBar[stream].ForEach(lambdaBar => data.Add(string.Format("{0:0.000000}", lambdaBar)));

				data.Add(Environment.NewLine);

				data.Add("RO'");
				networkModel.RoBar[stream].ForEach(roBar => data.Add(string.Format("{0:0.000000}", roBar)));

				data.Add(Environment.NewLine);

				if(networkModel.StreamsCount > 1)
				{
					data.Add("ROTOTAL");
					for (int i = 0; i < networkModel.RoTotal.Length; i++)
						data.Add(string.Format("{0:0.000000}", networkModel.RoTotal[i]));

					data.Add(Environment.NewLine);
				}

				data.Add("THE AVERAGE NUMBER OF PATHS");
				data.Add(string.Format("{0:0.000000}", networkModel.AveragePaths));

				data.Add(Environment.NewLine);

				data.Add("STATIONARY PROBABILITY-TIME CHARACTERISTICS");
				data.Add("W");
				networkModel.Ws.ForEach(ws => data.Add(string.Format("{0:0.000000}", ws)));

				data.Add(Environment.NewLine);

				data.Add("U");
				networkModel.Us[stream].ForEach(us => data.Add(string.Format("{0:0.000000}", us)));

				data.Add(Environment.NewLine);

				data.Add("L");
				networkModel.Ls[stream].ForEach(ls => data.Add(string.Format("{0:0.000000}", ls)));

				data.Add(Environment.NewLine);

				data.Add("N");
				networkModel.Ns[stream].ForEach(ns => data.Add(string.Format("{0:0.000000}", ns)));

				data.Add(Environment.NewLine);

				data.Add("INTEGRATED PROBABILITY-TIME CHARACTERISTICS");
				data.Add(string.Format("PATHS, {0}", networkModel.Paths.Count));
				data.AddRange(
					networkModel.Paths.Select((path, index) => path.ToString(" -> ")
						+ " : " + networkModel.TransitionProbabilities[index]));

				data.Add(Environment.NewLine);

				data.Add("W");
				data.Add(string.Format("{0:0.000000}", networkModel.Wi));

				data.Add(Environment.NewLine);

				data.Add("U");
				data.Add(string.Format("{0:0.000000}", networkModel.Ui[stream]));

				data.Add(Environment.NewLine);

				data.Add("L");
				data.Add(string.Format("{0:0.000000}", networkModel.Li[stream]));

				data.Add(Environment.NewLine);

				data.Add("N");
				data.Add(string.Format("{0:0.000000}", networkModel.Ni[stream]));

				if(networkModel.StreamsCount > 1)
				{
					data.Add("==================================================");

					if(stream != networkModel.StreamsCount - 1)
						data.Add(Environment.NewLine);
				}
			}

			if (!Directory.Exists(Path.GetDirectoryName(logFile)))
				Directory.CreateDirectory(Path.GetDirectoryName(logFile));

			File.WriteAllLines(logFile, data, Encoding.UTF8);
		}

		public static void ConsoleColorWrite(String message, ConsoleColor color, bool writeLine = true)
		{
			var originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;

			if (writeLine)
				Console.WriteLine(message);
			else
				Console.Write(message);

			Console.ForegroundColor = originalColor;
		}

		private static void EthernetHelp()
		{
			foreach (var ethernet in Enum.GetValues(typeof(ProtocolHelper.EthernetType)))
			{
				foreach (var frame in Enum.GetValues(typeof(ProtocolHelper.FrameLength)))
				{
					ConsoleColorWrite(string.Format(
						"{0}, {1}: ", (ProtocolHelper.EthernetType)ethernet, frame), ConsoleColor.Green, false);
					Console.WriteLine(string.Format("{0} frame/sec",
						ProtocolHelper.GetCapacity((ProtocolHelper.EthernetType)ethernet,
							(ProtocolHelper.FrameLength)frame)));
				}				

				Console.WriteLine();
			}
		}
	}
}