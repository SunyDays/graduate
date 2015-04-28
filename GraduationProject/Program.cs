using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Helpers;
using Modeling;
using Types;
using System.Text;

namespace Main
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var netconfigFullName = args.GetArgValue<string>("netconfig");
            var startNodeIndex = args.GetArgValue<int>("startnode");
            var targetNodeIndex = args.GetArgValue<int>("targetnode");

            var networkModel = new NetworkModel(netconfigFullName, startNodeIndex, targetNodeIndex);
            ConsoleOutput(networkModel);

            Console.ReadLine();
        }

        public static void ConsoleOutput(NetworkModel networkModel)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            ConsoleColorWrite("==================== Network model parameters ====================", ConsoleColor.Blue);

            ConsoleColorWrite(String.Format("Number of streams = {0}", networkModel.StreamsCount), ConsoleColor.Green);

            for (int stream = 0; stream < networkModel.StreamsCount; stream++)
            {
                ConsoleColorWrite(String.Format("==================== Stream {0} ====================", stream + 1),
                    ConsoleColor.White);    

                ConsoleColorWrite("Routing matrix", ConsoleColor.Green);
                var routingMatrix = networkModel.GetRoutingMatrix(stream);
                for (int i = 0; i < routingMatrix.RowsCount; i++)
                {
                    for (int j = 0; j < routingMatrix.ColumnsCount; j++)
                        ConsoleColorWrite(string.Format("{0:0.000}\t", routingMatrix[i, j]),
                            routingMatrix[i, j] == 0 ? Console.ForegroundColor : ConsoleColor.Yellow, false);

                    Console.WriteLine ();
                }
                Console.WriteLine();

                ConsoleColorWrite("Lambda\t\tMu\t\tRo", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}\t\t{2:0.000}",
                        networkModel.Lambda[stream][i], networkModel.Mu[stream][i], networkModel.Ro[stream][i]);
                Console.WriteLine();

                ConsoleColorWrite("Lambda0", ConsoleColor.Green);
                Console.WriteLine(networkModel.Lambda0[stream]);
                Console.WriteLine ();

                ConsoleColorWrite("E", ConsoleColor.Green);
                for (int i = 0; i < networkModel.E[stream].Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.E[stream][i]);
                Console.WriteLine();

                ConsoleColorWrite("LambdaBar\tRoBar", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}",
                        networkModel.LambdaBar[stream][i], networkModel.RoBar[stream][i]);
                Console.WriteLine();

                ConsoleColorWrite("RoTotal", ConsoleColor.Green);
                for (int i = 0; i < networkModel.RoTotal.Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.RoTotal[i]);
                Console.WriteLine();

                ConsoleColorWrite("Stationary probability-time characteristics", ConsoleColor.Green);
                ConsoleColorWrite("Ws", ConsoleColor.Green);
                for (int i = 0; i < networkModel.Ws.Length; i++)
                    Console.WriteLine("{0:0.000}\t", networkModel.Ws[i]);
                Console.WriteLine();

                ConsoleColorWrite("Us\t\tLs\t\tNs", ConsoleColor.Green);
                for (int i = 0; i < networkModel.NodesCount; i++)
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}\t\t{2:0.000}",
                        networkModel.Us[stream][i], networkModel.Ls[stream][i], networkModel.Ns[stream][i]);
                Console.WriteLine();

                ConsoleColorWrite("Integrated probability-time characteristics", ConsoleColor.Green);

                ConsoleColorWrite("Paths", ConsoleColor.Green);
                for (int i = 0; i < networkModel.Paths.Count; i++)
                    Console.WriteLine(
                        networkModel.Paths[i].Aggregate(string.Empty,
                            (accumulate, value) => accumulate +
                            (accumulate == string.Empty ? "" : " -> ") + value
                        )
                        + " : " + networkModel.TransitionProbabilities[i]
                    );
                Console.WriteLine();

                ConsoleColorWrite("Wi", ConsoleColor.Green);
                Console.WriteLine(networkModel.Wi);
                Console.WriteLine();

                ConsoleColorWrite("Ui\t\tLi\t\tNi", ConsoleColor.Green);
                    Console.WriteLine("{0:0.000}\t\t{1:0.000}\t\t{2:0.000}",
                        networkModel.Ui[stream], networkModel.Li[stream], networkModel.Ni[stream]);
                Console.WriteLine();

                ConsoleColorWrite("==================================================", ConsoleColor.White);
            }

            ConsoleColorWrite("==================================================================", ConsoleColor.Blue);
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

//        private static void TestProtocolHelper()
//        {
//            foreach (ulong ethernetType in Enum.GetValues(typeof(ProtocolHelper.EthernetType)))
//                foreach (int bound in Enum.GetValues(typeof(ProtocolHelper.FrameRange)))
//                {
//                    Console.ForegroundColor = ConsoleColor.Green;
//                    Console.WriteLine("Protocol = {0} Ethernet", (ProtocolHelper.EthernetType)ethernetType);
//                    Console.ForegroundColor = ConsoleColor.White;
//
//                    Console.WriteLine("bitRate = {0} bit/s\nframeLength = {1} bytes\ncapacity = {2} frames/milisecond",
//                        ProtocolHelper.GetBitRate((ProtocolHelper.EthernetType)ethernetType),
//                        bound,
//                        ProtocolHelper.GetCapacity((ProtocolHelper.EthernetType)ethernetType, bound));
//
//                    Console.WriteLine();
//                }
//        }
    }
}