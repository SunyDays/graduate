using System.Collections.Generic;
using Types;
using System.Linq;
using System.Collections;
using System;
using System.Diagnostics;
using GLib;

namespace Helpers
{
    public static class GraphHelper
    {
		public static List<Vector<int>> GetPathsBetween(Matrix<double> matrix, int startNode, int targetNode)
        {
			if (startNode == targetNode)
				throw new ArgumentException("stratNode and targetNode must be different.");

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
					List<Vector<int>> localPaths = GetPathsBetween(matrix.Clone(), intermediateNode, targetNode);
                    if (localPaths.Any())
						localPaths.ForEach(path =>
							{
								path.InsertElement(0, startNode);
								paths.Add(path);
							});
                }
            }

            return paths;
        }
    }
}