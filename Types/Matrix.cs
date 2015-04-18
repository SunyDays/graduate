using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Types
{
    [Serializable]
    public class Matrix<T> : IEnumerable<Vector<T>>
    {
        private List<Vector<T>> _matrix;
        public int RowsCount { get;private set;}
        public int ColumnsCount { get;private set;}

        public Matrix()
        {
            InitializeMatrix();
        }

        public Matrix(int rowsCount, int columnsCount)
        {
            if (rowsCount <= 0 || columnsCount <= 0)
                throw new ArgumentException("Rows/columns count must be greater then 0.");

            RowsCount = rowsCount;
            ColumnsCount = columnsCount;

            InitializeMatrix();
        }

        public Matrix(T[,] array)
        {
            RowsCount = array.GetUpperBound(0) + 1;
            ColumnsCount = array.GetUpperBound(1) + 1;

            InitializeMatrix();

            for (int r = 0; r < RowsCount; r++)
                for (int c = 0; c < ColumnsCount; c++)
                    _matrix[r][c] = array[r, c];
        }

        public Matrix(IEnumerable<Vector<T>> rows)
        {
            RowsCount = rows.Count();
            ColumnsCount = rows.First().Length;

            _matrix = rows.ToList();
        }

        private void InitializeMatrix()
        {
            _matrix = new List<Vector<T>>();

            if(RowsCount != 0 && ColumnsCount != 0)
                Enumerable.Range(0, RowsCount).ToList().ForEach(rowIndex => _matrix.Add(new Vector<T>(ColumnsCount)));
        }

        public T this[int row, int column]
        {
            get
            {
                CheckIndices(row, column);
                return _matrix[row][column];
            }
            set
            {
                CheckIndices(row, column);
                _matrix[row][column] = value;
            }
        }

        private void CheckIndices(int rowIndex, int columnIndex)
        {
            CheckRowIndex(rowIndex);
            CheckColumnIndex(columnIndex);
        }

        private void CheckRowIndex(int rowIndex)
        {
            if (0 <= rowIndex && rowIndex < RowsCount) return;

            throw new IndexOutOfRangeException("Invalid row index.");
        }

        private void CheckColumnIndex(int columnIndex)
        {
            if (0 <= columnIndex && columnIndex < ColumnsCount) return;

            throw new IndexOutOfRangeException("Invalid column index.");
        }

        // TODO: test foreach
        public Matrix<T> ForEachRow(Action<Vector<T>> action)
        {
            foreach (var row in _matrix)
                action(row);

            return this;
        }

        public Matrix<T> ForEachElement(Action<T> action)
        {
            foreach (var row in _matrix)
                foreach (var item in row)
                    action(item);

            return this;
        }

        #region rows operations
        public Matrix<T> AddRow(IEnumerable<T> row)
        {
            CheckRowLength(row.Count());

            _matrix.Add(new Vector<T>(row));
            RowsCount++;

            return this;
        }

        public Vector<T> GetRow(int index)
        {
            CheckRowIndex(index);

            return _matrix[index];
        }

        public Matrix<T> GetRows(int index, int count)
        {
            CheckRowIndex(index);
            CheckRowIndex(index + count);

            return new Matrix<T>(_matrix.Skip(index - 1).Take(count));
        }

        public Matrix<T> InsertRow(int index, IEnumerable<T> row)
        {
            CheckRowIndex(index == 0 ? index : index - 1);
            CheckRowLength(row.Count());

            _matrix.Insert(index, new Vector<T>(row));
            RowsCount++;

            return this;
        }

        public Matrix<T> RemoveRow(int index)
        {
            CheckRowIndex(index);

            _matrix.RemoveAt(index);
            RowsCount--;

            return this;          
        }

        public Matrix<T> RemoveRows(int index, int count)
        {
            CheckRowIndex(index);
            CheckRowIndex(index + count);

            _matrix.RemoveRange(index, count);
            RowsCount -= count;

            return this;
        }

        public Matrix<T> ReplaceRow(int index, IEnumerable<T> row)
        {
            CheckRowIndex(index);
            CheckRowLength(row.Count());

            RemoveRow(index);
            InsertRow(index, row);

            return this;
        }

        private void CheckRowLength(int rowLength)
        {
            if (rowLength != ColumnsCount)
                throw new ArgumentException("Invalid row length.");
        }
        #endregion

        #region columns operations
        public Matrix<T> AddColumn(IEnumerable<T> column)
        {
            CheckColumnLength(column.Count());

//            var vector = new Vector<T>(column);
//            for (int r = 0; r < RowsCount; r++)
//                _matrix[r].AddElement(column[r]);

            _matrix.Zip(column, (row, item) => row.AddElement(item));

            ColumnsCount++;

            return this;
        }

        public Vector<T> GetColumn(int index)
        {
            CheckColumnIndex(index);

            return new Vector<T>(_matrix.Select(row => row[index]));
        }

        public Matrix<T> GetColumns(int index, int count)
        {
            CheckColumnIndex(index);
            CheckColumnIndex(index + count);

            return new Matrix<T>(_matrix.Select(row => row.GetSubVector(index, count)));
        }

        public Matrix<T> InsertColumn(int index, IEnumerable<T> column)
        {
            CheckColumnIndex(index == 0 ? index : index - 1);
            CheckColumnLength(column.Count());

            var vector = new Vector<T>(column);
            for (int r = 0; r < RowsCount; r++)
                _matrix[r].InsertElement(index, vector[r]);

            ColumnsCount++;

            return this;
        }

        public Matrix<T> RemoveColumn(int index)
        {
            CheckColumnIndex(index);

            _matrix.ForEach(row => row.RemoveElement(index));
            ColumnsCount--;

            return this;
        }

        public Matrix<T> RemoveColumns(int index, int count)
        {
            CheckColumnIndex(index);
            CheckColumnIndex(index + count);

            _matrix.ForEach(row => row.RemoveElements(index, count));

            ColumnsCount -= count;

            return this;
        }

        public Matrix<T> ReplaceColumn(int index, IEnumerable<T> column)
        {
            CheckColumnIndex(index);
            CheckColumnLength(column.Count());

            RemoveColumn(index);
            InsertColumn(index, column);

            return this;
        }

        private void CheckColumnLength(int columnLength)
        {
            if(columnLength != RowsCount)
                throw new ArgumentException("Invalid column length.");
        }
        #endregion

        public Matrix<T> Transpose()
        {
            var transposeMatrix = new Matrix<T>(ColumnsCount, 1);
            _matrix.ForEach(row => transposeMatrix.AddColumn(row));
            _matrix = transposeMatrix.RemoveColumn(0).GetList();

            return this;
        }

        public T[,] GetArray()
        {
            var array = new T[RowsCount, ColumnsCount];
            for (int r = 0; r < RowsCount; r++)
                for (int c = 0; c < ColumnsCount; c++)
                    array[r, c] = _matrix[r][c];

            return array;
        }

        public List<Vector<T>> GetList()
        {
            return _matrix;
        }

        #region IEnumerable implementation
        public IEnumerator<Vector<T>> GetEnumerator()
        {
            return _matrix.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}