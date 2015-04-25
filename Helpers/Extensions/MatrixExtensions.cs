using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Helpers;

namespace Types
{
    public static class MatrixExtensions
    {
        public static Matrix<double> ReplaceDiagonalElements(this Matrix<double> matrix, double value)
        {
            var cloneMatrix = matrix.Clone();
            for (int i = 0; i < new[] { cloneMatrix.RowsCount, cloneMatrix.ColumnsCount }.Min(); i++)
                cloneMatrix[i, i] = value;

            return cloneMatrix;
        }
    }
}

