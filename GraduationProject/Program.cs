using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Helpers;
using Modeling;
using Types;

namespace Main
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("Need config file", "args");

            var networkModel = new NetworkModel(args[0]);
            ConsoleOutput(networkModel);

//            TestGuassMethod();

//            var A = new Matrix<double>(new double[,]
//                {
//                    { -1, 0, 0.1, 0, 0 },
//                    { 0, -1, 0, 0, 0 },
//                    { 0, 0.2, -1, 0.2, 0 },
//                    { 1, 0.2, 0, -1, 0 },
//                    { 0, 0.6, 0, 0, -1 }
//                });
//            var b = new Vector<double>(new double[]{ -0.3333, -0.6667, 0, 0, 0 });

//            var A = new Matrix<double>(new double[,]
//                {
//                    { 2, 1, -1},
//                    { -3, -1, 2 },
//                    { -2, 1, 2 }
//                });
//            var b = new Vector<double>(new double[]{ 8, -11, -3 });
//
//            var result = GaussMethod.Solve(A, b);
//            result.GetList().ForEach(Console.WriteLine);

//            var dv = new DoubleVector();

            Console.ReadLine();
        }

        public static void ConsoleOutput(NetworkModel networkModel)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            ColorWriteLine("==================== Network model parameters ====================", ConsoleColor.Blue);

            ColorWriteLine(String.Format("Number of streams = {0}", networkModel.StreamsCount), ConsoleColor.Green);

            for (int stream = 0; stream < networkModel.StreamsCount; stream++)
            {
                ColorWriteLine(String.Format("==================== Stream {0} ====================", stream + 1),
                    ConsoleColor.White);    

                ColorWriteLine("Routing matrix", ConsoleColor.Green);
                var routingMatrix = networkModel.GetRoutingMatrix(stream);
                for (int i = 0; i < routingMatrix.RowsCount; i++)
                {
                    for (int j = 0; j < routingMatrix.ColumnsCount; j++)
                        Console.Write("{0:0.000}\t", routingMatrix[i, j]);

                    Console.WriteLine ();
                }
                Console.WriteLine();

                ColorWriteLine("Lambda\t\tMu\t\tRo", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}\t\t{2:0.000}",
                        networkModel.Lambda[stream][i], networkModel.Mu[stream][i], networkModel.Ro[stream][i]);
                Console.WriteLine();

//                ColorWriteLine("Lambda", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Lambda[stream].Length; i++)
//                    Console.WriteLine("{0}\t", networkModel.Lambda[stream][i]);
//                Console.WriteLine ();

                ColorWriteLine("Lambda0", ConsoleColor.Green);
                Console.WriteLine(networkModel.Lambda0[stream]);
                Console.WriteLine ();

//                ColorWriteLine("Mu", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Mu[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.Mu[stream][i]);
//                Console.WriteLine ();
//
//                ColorWriteLine("Ro", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Ro[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.Ro[stream][i]);
//                Console.WriteLine();

                ColorWriteLine("E", ConsoleColor.Green);
                for (int i = 0; i < networkModel.E[stream].Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.E[stream][i]);
                Console.WriteLine();

                ColorWriteLine("LambdaBar\tRoBar", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}",
                        networkModel.LambdaBar[stream][i], networkModel.RoBar[stream][i]);
                Console.WriteLine();

//                ColorWriteLine("LambdaBar", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.LambdaBar[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.LambdaBar[stream][i]);
//                Console.WriteLine();
//
//                ColorWriteLine("RoBar", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.RoBar[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.RoBar[stream][i]);
//                Console.WriteLine();

                ColorWriteLine("RoTotal", ConsoleColor.Green);
                for (int i = 0; i < networkModel.RoTotal.Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.RoTotal[i]);
                Console.WriteLine();

                ColorWriteLine("Stationary probability-time characteristic", ConsoleColor.Green);
                ColorWriteLine("Ws", ConsoleColor.Green);
                for (int i = 0; i < networkModel.Ws.Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.Ws[i]);
                Console.WriteLine();

                ColorWriteLine("Us\t\tLs\t\tNs", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}\t\t{2:0.000}",
                        networkModel.Us[stream][i], networkModel.Ls[stream][i], networkModel.Ns[stream][i]);
                Console.WriteLine();

//                ColorWriteLine("Us", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Us[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.Us[stream][i]);
//                Console.WriteLine();
//
//                ColorWriteLine("Ls", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Ls[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.Ls[stream][i]);
//                Console.WriteLine();
//
//                ColorWriteLine("Ns", ConsoleColor.Green);
//                for (int i = 0; i < networkModel.Ns[stream].Length; i++)
//                    Console.WriteLine("{0:0.000}\t", networkModel.Ns[stream][i]);
//                Console.WriteLine();

                ColorWriteLine("==================================================", ConsoleColor.White);
            }

            ColorWriteLine("==================================================================", ConsoleColor.Blue);
        }

        public static void ColorWriteLine(String message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        private static void TestProtocolHelper()
        {
            foreach (ulong ethernetType in Enum.GetValues(typeof(ProtocolHelper.EthernetType)))
                foreach (int bound in Enum.GetValues(typeof(ProtocolHelper.FrameRange)))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Protocol = {0} Ethernet", (ProtocolHelper.EthernetType)ethernetType);
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("bitRate = {0} bit/s\nframeLength = {1} bytes\ncapacity = {2} frames/milisecond",
                        ProtocolHelper.GetBitRate((ProtocolHelper.EthernetType)ethernetType),
                        bound,
                        ProtocolHelper.GetCapacity((ProtocolHelper.EthernetType)ethernetType, bound));

                    Console.WriteLine();
                }
        }

        private static void TestGuassMethod()
        {
            var A = new double[,]
            {
                {-1, 0, 0.1, 0, 0},
                {0, -1, 0, 0, 0},
                {0, 0.2, -1, 0.2, 0},
                {1, 0.2, 0, -1, 0},
                {0, 0.6, 0, 0, -1}
            };
            var b = new double[]{ -0.3333, -0.6667, 0, 0, 0};
//            GaussMethod.Solve(A, b);
        }
    }
}