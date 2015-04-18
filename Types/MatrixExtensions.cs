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
            var m = matrix.Clone();
            for (int i = 0; i < new[] { m.RowsCount, m.ColumnsCount }.Min(); i++)
                m[i, i] = value;

            return m;
        }
    }
}

