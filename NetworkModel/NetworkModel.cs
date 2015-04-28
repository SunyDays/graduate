using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using Helpers;
using Types;
using System.Diagnostics.SymbolStore;
using System.IO.Pipes;

namespace Modeling
{
    public class NetworkModel
    {
        #region PARAMETERS
        private Matrix<double> RoutingMatrix;
        public List<Vector<double>> Lambda          { get; private set; }
        public List<Vector<double>> Mu              { get; private set; }
        public Vector<double> Lambda0               { get; private set; }
        public int StreamsCount                     { get; private set; }
        public int NodesCount                       { get; private set; }
        public List<Vector<double>> InputIntensity  { get; private set; }
        public List<Vector<double>> E               { get; private set; }
        public List<Vector<double>> LambdaBar       { get; private set; }
        public List<Vector<double>> Ro              { get; private set; }
        public List<Vector<double>> RoBar           { get; private set; }
        public Vector<double> RoTotal               { get; private set; }
        public List<Vector<int>> Paths              { get; private set; }
        public List<double> TransitionProbabilities { get; private set; }

        public int StartNode                        { get; private set; }
        public int TargetNode                       { get; private set; }
        #endregion

        #region PROBABILITY-TIME CHARACTERISTICS
        public Vector<double> Ws                    { get; private set; }
        public List<Vector<double>> Us              { get; private set; }
        public List<Vector<double>> Ls              { get; private set; }
        public List<Vector<double>> Ns              { get; private set; }

        public double Wi                            { get; private set; }
        public List<double> Ui                      { get; private set; }
        public List<double> Li                      { get; private set; }
        public List<double> Ni                      { get; private set; }
        #endregion

        private NetworkModel(int startNode, int targetNode)
        {
            StartNode = startNode;
            TargetNode = targetNode;

            Lambda = new List<Vector<double>>();
            Mu = new List<Vector<double>>();
            Lambda0 = new Vector<double>();
            InputIntensity = new List<Vector<double>>();
            E = new List<Vector<double>>();
            LambdaBar = new List<Vector<double>>();
            Ro = new List<Vector<double>>();
            RoBar = new List<Vector<double>>();
            RoTotal = new Vector<double>();

            Ws = new Vector<double>();
            Us = new List<Vector<double>>();
            Ls = new List<Vector<double>>();
            Ns = new List<Vector<double>>();
            Ui = new List<double>();
            Li = new List<double>();
            Ni = new List<double>();
        }

        public NetworkModel(String path, int startNode, int targetNode)
            : this(startNode, targetNode)
        {
            ParseConfig(path);

            ComputeParameters();
            ComputeStationaryPTC();
            ComputeIntegratedPTC();
        }

        public Matrix<double> GetRoutingMatrix(int streamIndex)
        {
            if (streamIndex > StreamsCount)
                throw new ArgumentException();


            var streamRoutingMatrix = RoutingMatrix.Clone();
            streamRoutingMatrix.InsertRow(0, InputIntensity[streamIndex]);

            return streamRoutingMatrix;
        }

        private void ComputeParameters()
        {
            Lambda.ForEach(lambdas => Lambda0.AddElement(lambdas.Sum()));
            RoTotal = new Vector<double>(NodesCount);

            for (int stream = 0; stream < StreamsCount; stream++)
            {
                // input intensity
                InputIntensity.Add(Lambda[stream].Clone().InsertElement(0, 0).Divide(Lambda0[stream]));

                // E
                E.Add(GaussMethod.Solve(GetExtendedMatrix(stream)));

                // lambda bar
                LambdaBar.Add(E[stream].Multiply(Lambda0[stream]));

                // ro bar & ro total
                RoBar.Add(LambdaBar[stream].DivideElementWise(Mu[stream]));
                RoTotal = RoTotal.AddElementWise(RoBar.Last());

                if(RoBar[stream].Any(roBar => roBar > 1))
                    throw new ArgumentOutOfRangeException(string.Format("RoBar, stream {0}", stream), "Some Ro is greater than zero.");
            }

            if (RoTotal.Any(roTotal => roTotal > 1))
                throw new ArgumentOutOfRangeException("RoTotal", "Some RoTotal is greater than zero.");
        }

        private Matrix<double> GetExtendedMatrix(int streamIndex)
        {
            var matrix = GetRoutingMatrix(streamIndex);
            matrix.Transpose();
            matrix.RemoveRow(0);
            matrix.AddColumn(matrix.GetColumn(0).Negate());
            matrix.RemoveColumn(0);
            matrix = matrix.ReplaceDiagonalElements(-1);

            return matrix;
         }

        #region COMPUTE PROBABILITY-TIME CHARACTERISTICS
        private void ComputeStationaryPTC()
        {
            for (int i = 0; i < NodesCount; i++)
            {
                double sum = 0.0;
                for (int stream = 0; stream < StreamsCount; stream++)
                    sum += RoBar[stream][i] / Mu[stream][i];

                Ws.AddElement(sum / (1 - RoTotal[i]));
            }

            for (int stream = 0; stream < StreamsCount; stream++)
            {
                Us.Add( Ws.AddElementWise(Mu[stream].Pow( -1 )) );
                Ls.Add( LambdaBar[stream].MultiplyElementWise( Ws ) );
                Ns.Add( LambdaBar[stream].MultiplyElementWise( Us[stream]) );
            }
        }

        private void ComputeIntegratedPTC()
        {
            Paths = GraphHelper.GetAllPaths(RoutingMatrix.Clone().RemoveColumn(0), StartNode, TargetNode);
            ComputePathsProbabilities();

            Wi = ComputeIntegralChar(Ws);
            for (int stream = 0; stream < StreamsCount; stream++)
            {
                Ui.Add(ComputeIntegralChar(Us[stream]));
                Li.Add(ComputeIntegralChar(Ls[stream]));
                Ni.Add(ComputeIntegralChar(Ns[stream]));
            }
        }

        private double ComputeIntegralChar(Vector<double> staticChar)
        {
            var integralChar = staticChar[StartNode] + staticChar[TargetNode];
            for (int i = 0; i < Paths.Count; i++)
            {
                var temp = 0.0;
                for (int j = 1; j < Paths[i].Length - 1; j++)
                    temp += staticChar[j];

                integralChar += TransitionProbabilities[i] * temp;
            }

            return integralChar;
        }

        private void ComputePathsProbabilities()
        {
            var matrix = RoutingMatrix.Clone().RemoveColumn(0);

            var pathsProbabilities = 
                Paths.Select(path => 
                    path.Where((item, index) => index < path.Length -1).Select((item, index) => new {item, index})
                    .Aggregate(1.0, (accumulate, anon) => accumulate *= matrix[anon.item, path[anon.index + 1]]));

            var s = pathsProbabilities.Sum();

            TransitionProbabilities = pathsProbabilities.Select(prob => prob / s).ToList();
        }
        #endregion

        #region PARSE XML
        private void ParseConfig(String path)
        {
            var root = XDocument.Load(path).Root;
            ParseRoutingMatrix(root);
            ParseNodes(root);
        }

        private void ParseRoutingMatrix(XContainer root)
        {
            var rows = GetElements(root, "Row");
            RoutingMatrix = new Matrix<double>(rows.Count - 1, rows.Count);

            for (int r = 0; r < rows.Count - 1; r++)
            {
                var rowItems = rows[r + 1].Value.Split(new[] { ';' });
                var sum = 0.0;
                for (int c = 0; c < rowItems.Length; c++)
                {
                    var item = Double.Parse(rowItems[c]);
                    sum += item;
                    RoutingMatrix[r, c] = item;
                }

                if (!sum.Equals(1.0))
                    throw new ArgumentOutOfRangeException(String.Format("RoutingMatrix, row {0}", r),
                        "The sum of the row must be equal to 1");
            }
        }

        private void ParseNodes(XContainer root)
        {
            GetStreamsCount(root);
            GetNodesCount(root);

            for (int stream = 1; stream <= StreamsCount; stream++)
            {
                Lambda.Add(
                    new Vector<double>(
                        GetElements(root, "Lambda").Where(element => int.Parse(element.Attribute("Stream").Value) == stream)
                        .Select(element => double.Parse(element.Value))
                    ));
                Mu.Add(
                    new Vector<double>(
                        GetElements(root, "Mu").Where(element => int.Parse(element.Attribute("Stream").Value) == stream)
                        .Select(ParseMu)
                    ));

                Ro.Add(Lambda.Last().DivideElementWise(Mu.Last()));
            }
        }


        private void GetStreamsCount(XContainer root)
        {
            var elements = GetElements(root, "Lambda");
            StreamsCount = elements.Max(element => int.Parse(element.Attribute("Stream").Value));

            if(elements.Min(element => int.Parse(element.Attribute("Stream").Value)).Equals(0))
                StreamsCount ++;
        }

        private void GetNodesCount(XContainer root)
        {
            NodesCount = GetElements(root, "Node").Count();
        }

        private double ParseMu(XElement element)
        {
            if (!element.Descendants("Ethernet").Any())
                return double.Parse(element.Value);

            XElement ethernetElement = element.Descendants("Ethernet").First();
            var ethernetType = (ProtocolHelper.EthernetType)Enum.Parse(typeof(ProtocolHelper.EthernetType),
                                   ethernetElement.Attribute("Type").Value);

            int frameLength;
            try
            {
                frameLength = int.Parse(ethernetElement.Attribute("FrameLength").Value);
            }
            catch (FormatException)
            {
                frameLength = (int)Enum.Parse(typeof(ProtocolHelper.FrameRange), ethernetElement.Attribute("FrameLength").Value);
            }

            return ProtocolHelper.GetCapacity(ethernetType, frameLength);
        }

        private List<XElement> GetElements(XContainer element, String elementName)
        {
            return element.Descendants(elementName).ToList();
        }
        #endregion
    }
}