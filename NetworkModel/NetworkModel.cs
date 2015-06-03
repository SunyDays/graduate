using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Helpers;
using Types;
using System.Threading.Tasks;

namespace Modeling
{
    public class NetworkModel
    {
        public string Name                          { get; private set;}

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
		public List<Vector<int>> AllPaths           { get; private set; }
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

                // lambda'
                LambdaBar.Add(E[stream].Multiply(Lambda0[stream]));

                // ro' & ro total
                RoBar.Add(LambdaBar[stream].DivideElementWise(Mu[stream]));
                RoTotal = RoTotal.AddElementWise(RoBar.Last());

                if(RoBar[stream].Any(roBar => roBar > 1))
                    throw new ArgumentOutOfRangeException(string.Format("RoBar, stream {0}", stream), "Some Ro' is greater than zero.");
            }

            if (RoTotal.Any(roTotal => roTotal > 1))
                throw new ArgumentOutOfRangeException("RoTotal", "Some RoTotal is greater than zero.");

			AllPaths = GraphHelper.GetAllPaths(RoutingMatrix.Clone().RemoveColumn(0))
				.OrderBy(path => path.Length).ToList();
			Paths = AllPaths.Where(path => path.First() == StartNode && path.Last() == TargetNode).ToList();
        }

        private Matrix<double> GetExtendedMatrix(int streamIndex)
        {
            var matrix = GetRoutingMatrix(streamIndex);
            matrix.Transpose();
            matrix.RemoveRow(0);
            matrix.AddColumn(matrix.GetColumn(0).Negate());
            matrix.RemoveColumn(0);
            matrix.ReplaceDiagonalElements(-1);

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
			ComputeTransitionProbabilities();

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

			for (int i = 0; i < Paths.Count(); i++)
            {
                var temp = 0.0;
				for (int j = 1; j < Paths.ElementAt(i).Count() - 1; j++)
                    temp += staticChar[j];

                integralChar += TransitionProbabilities[i] * temp;
            }

            return integralChar;
        }

		private void ComputeTransitionProbabilities()
        {
            var matrix = RoutingMatrix.Clone().RemoveColumn(0);

            var pathsProbabilities = 
                Paths.Select(path => 
					path.Where((item, index) => index < path.Length - 1).Select((item, index) => new {item, index})
                    .Aggregate(1.0, (accumulate, anon) => accumulate *= matrix[anon.item, path[anon.index + 1]]));

            var s = pathsProbabilities.Sum();

            TransitionProbabilities = pathsProbabilities.Select(prob => prob / s).ToList();
        }
        #endregion

        #region COMPUTE PROBABILITY DENSITY
		public IEnumerable<double> ComputeDensity(Vector<int> path, int stream, IEnumerable<double> t)
        {
            for (int i = 0; i < t.Count(); i++)
			{
				var result = 0.0;

				foreach (var node in path)
					result += ComputeHi(path, node, stream) *
					(Mu[stream][node] - LambdaBar[stream][node]) *
					Math.Pow(Math.E, -(Mu[stream][node] - LambdaBar[stream][node]) * t.ElementAt(i));

				yield return result;
			}
        }

		private double ComputeHi(Vector<int> path, int i, int stream)
        {
            var Hi = 1.0;

            foreach (var node in path)
                if (node != i)
                    Hi *= (Mu[stream][node] - LambdaBar[stream][node]) /
                    (Mu[stream][node] - Mu[stream][i] - LambdaBar[stream][node] + LambdaBar[stream][i]);

            return Hi;
        }
        #endregion

        #region PARSE XML
        private void ParseConfig(String path)
        {
            var root = XDocument.Load(path).Root;
            Name = root.Attribute("Name").Value;

            ParseRoutingMatrix(root);
            ParseNodes(root);
        }

        // TODO: don't touch
        private void ParseRoutingMatrix(XContainer root)
        {
            var rows = GetElements(root, "Row").Skip(1).Select(row => row.Value).ToList();
            for (int i = 0; i < rows.Count(); i++)
            {
                var elements = rows[i].Split(';').Select(element => element.Trim()).ToList();

                var n = elements.Count(element => element.Contains("-"));
                var value = 0.0;
                if(n != 0)
                    value = (1 - double.Parse(elements[0])) / n;

                var row = elements.Select(element => element.Contains("-") ?
                    value :
                    double.Parse(element));
                if (i == 0)
                    RoutingMatrix = new Matrix<double>(new[] { row });
                else
                    RoutingMatrix.AddRow(row);
            }

            var wrongRowsIndeces = RoutingMatrix.Select((row, index) => new {row, index})
                .Where(anon => !anon.row.Sum().Equals(1.0))
                .Aggregate(string.Empty,
                    (wrongRows, index) => wrongRows + string.Format("{0} ,", index));

            if(wrongRowsIndeces != string.Empty)
                throw new ArgumentOutOfRangeException(String.Format("RoutingMatrix, rows {0}", wrongRowsIndeces),
                    "The sum of the row must be equal to 1");
        }

        private void ParseNodes(XContainer root)
        {
            StreamsCount = GetElements(root, "Stream")
                .Max(element => int.Parse(element.Attribute("Index").Value)) + 1;

            NodesCount = GetAttributeValue<int>(GetElement(root, "Nodes"), "Count");

            for (int stream = 0; stream < StreamsCount; stream++)
            {
                Mu.Add(new Vector<double>(
                    ParseMu(GetElements(root, "Mu")
                        .Single(element => GetAttributeValue<int>(element.Parent, "Index") == stream))));

                    Lambda.Add(new Vector<double>(
                            ParseLambda(GetElements(root, "Lambda")
                        .Single(element => GetAttributeValue<int>(element.Parent, "Index") == stream), stream)));

                Ro.Add(Lambda.Last().DivideElementWise(Mu.Last()));
            }
        }

        private static Random random = new Random();

        private IEnumerable<double> ParseLambda(XElement element, int stream)
        {
            return element.Value.ToLower().Contains("rand") ?
            // FIXME: some RoBar > 1. random values must be under control
            Enumerable.Range(0, NodesCount).Select(index => (double)random.Next(1, (int)Math.Floor(Mu[stream][index]))) :

            element.Value.ToLower().Contains(";") ?
            element.Value.Split(';').Select(lambda => double.Parse(lambda.Trim())) :
            Enumerable.Repeat(double.Parse(element.Value), NodesCount);
        }

        private IEnumerable<double> ParseMu(XElement element)
        {
            var ethernetElements = GetElements(element, "Ethernet");

            return ethernetElements.Count == 1 ?
                Enumerable.Repeat(ParseEthernet(ethernetElements.First()), NodesCount) :
                ethernetElements.Select(ethernet => ParseEthernet(ethernet));
        }

        private double ParseEthernet(XElement element)
        {
            var ethernetType = (ProtocolHelper.EthernetType)Enum.Parse(typeof(ProtocolHelper.EthernetType),
                                   element.Attribute("Type").Value);

            int frameLength;
            try
            {
                frameLength = int.Parse(element.Attribute("FrameLength").Value);
            }
            catch (FormatException)
            {
                frameLength = (int)Enum.Parse(typeof(ProtocolHelper.FrameRange), element.Attribute("FrameLength").Value);
            }

            return ProtocolHelper.GetCapacity(ethernetType, frameLength);
        }

        private List<XElement> GetElements(XContainer element, String elementName)
        {
            return element.Descendants(elementName).ToList();
        }

        private XElement GetElement(XContainer element, string elementName)
        {
            return GetElements(element, elementName).SingleOrDefault();
        }

        private T GetElementValue<T>(XContainer element, string elementName)
        {
            return GetElement(element, elementName).Value.CastObject<T>();
        }

        private T GetAttributeValue<T>(XElement element, string attributeName)
        {
            return element.Attribute(attributeName).Value.CastObject<T>();
        }
        #endregion
    }
}