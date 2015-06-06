using System;
using System.Collections.Specialized;
using System.Linq;
using Types;
using System.Threading.Tasks;

namespace Helpers
{
    public static class GaussMethod
    {
        public static Vector<double> Solve(double[,] A, double[] b)
        {
            return Solve(new Matrix<double>(A), new Vector<double>(b));
        }

        public static Vector<double> Solve(double[,] extendedMatrix)
        {
            return Solve(new Matrix<double>(extendedMatrix));
        }

        public static Vector<double> Solve(Matrix<double> A, Vector<double> b)
        {
            A.AddColumn(b);
            return Solve(A);
        }

        public static Vector<double> Solve(Matrix<double> extendedMatrix)
        {
            if(extendedMatrix.RowsCount < extendedMatrix.ColumnsCount - 1)
                throw new ArgumentException("The number of equations is less than the number of unknowns.");

            if (extendedMatrix.ColumnsCount - 1 < extendedMatrix.RowsCount)
                extendedMatrix.RemoveRows(extendedMatrix.ColumnsCount,
                    extendedMatrix.RowsCount - extendedMatrix.ColumnsCount);

            for (int baseR = 0; baseR < extendedMatrix.RowsCount - 1; baseR++)
            {
                var baseRow = extendedMatrix.GetRow(baseR);
                for (int additionR = baseR + 1; additionR < extendedMatrix.RowsCount; additionR++)
                {
                    var additionRow = extendedMatrix.GetRow(additionR);
                    if(!additionRow[baseR].Equals(0))
                        extendedMatrix.ReplaceRow( additionR,
                            additionRow.Multiply( baseRow[baseR])
                            .SubstractElementWise( baseRow.Multiply(additionRow[baseR]) ));
                }
            }

            var result = new Vector<double>(extendedMatrix.RowsCount);
            for (int r = extendedMatrix.RowsCount - 1; r >= 0; r--)
            {
                if (extendedMatrix[r, r].Equals(0))
                {
                    result = new Vector<double>(Enumerable.Repeat(double.NaN, extendedMatrix.RowsCount));
                    break;
                }

                result[r] = extendedMatrix[r, extendedMatrix.ColumnsCount - 1];
                for (int c = r + 1; c < extendedMatrix.RowsCount; c++)
                    result[r] -= extendedMatrix[r, c] * result[c];
                result[r] /= extendedMatrix[r, r];
            }

            return result;
        }
    }
}