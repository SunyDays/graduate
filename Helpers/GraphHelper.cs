using System.Collections.Generic;
using Types;
using System.Linq;

namespace Helpers
{
    public static class GraphHelper
    {
        public static List<Vector<int>> GetAllPaths(Matrix<double> matrix, int startNode, int targetNode)
        {
            var paths = new List<Vector<int>>();

            // exclude routing loops
            matrix.ForEachRow(row => row[startNode] = 0);

            for (int intermediateNode = 0; intermediateNode < matrix.RowsCount; intermediateNode++)
            {
                if (matrix[startNode, intermediateNode] <= 0)
                    continue;

                if (intermediateNode == targetNode)
                    paths.Add(new Vector<int>(new[] { startNode, targetNode }));
                else
                {
                    List<Vector<int>> localPaths = GetAllPaths(matrix.Clone(), intermediateNode, targetNode);
                    if (localPaths.Any())
                        localPaths.ForEach(path => paths.Add(path.InsertElement(0, startNode)));
                }
            }

            return paths;
        }
    }
}