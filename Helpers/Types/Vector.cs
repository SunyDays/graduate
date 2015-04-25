using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Types;
using System.Threading.Tasks;

namespace Types
{
    [Serializable]
    public class Vector<T> : IEnumerable<T>
    {
        private List<T> _vector;
        public int Length{ get; private set;}

        public Vector()
        {
            InitializeVector();
        }

        public Vector(int length)
        {
            Length = length;

            InitializeVector();
        }

        public Vector(IEnumerable<T> collection)
        {
            Length = collection.Count();
            _vector = new List<T>(collection);
        }

        private void InitializeVector()
        {
            _vector = new List<T>();

            if(Length != 0)
                Enumerable.Range(0, Length).ToList().ForEach(index => _vector.Add(default(T)));
        }

        public T this[int index]
        {
            get
            {
                CheckIndex(index);
                return _vector[index];
            }
            set
            {
                CheckIndex(index);
                _vector[index] = value;
            }
        }

        private void CheckIndex(int index)
        {
            if (0 <= index && index < Length) return;

            throw new IndexOutOfRangeException();
        }

        public Vector<T> GetSubVector(int index, int length)
        {
            CheckIndex(index);
            CheckIndex(index + length);

            return new Vector<T>(_vector.Skip(index - 1).Take(length));
        }

        public Vector<T> AddElement(T element)
        {
            _vector.Add(element);
            Length++;

            return this;
        }

        public Vector<T> AddElements(IEnumerable<T> collection)
        {
            _vector.AddRange(collection);
            Length += collection.Count();

            return this;
        }

        public Vector<T> Concat(IEnumerable<T> collection)
        {
            return AddElements(collection);
        }

        public Vector<T> InsertElement(int index, T element)
        {
            CheckIndex(index == 0 ? index : index - 1);

            _vector.Insert(index, element);
            Length++;

            return this;
        }

        public Vector<T> RemoveElement(int index)
        {
            CheckIndex(index);

            _vector.RemoveAt(index);
            Length--;

            return this;
        }

        public Vector<T> RemoveElements(int index, int count)
        {
            CheckIndex(index);
            CheckIndex(index + count);

            _vector.RemoveRange(index, count);
            Length -= count;

            return this;
        }

        private void CheckVectorLength(int vectorLength)
        {
            if (vectorLength != Length)
                throw new ArgumentException("Invalid vector length.");
        }

        // TODO: test foreach
        public Vector<T> ForEach(Action<T> action)
        {
            foreach (var item in _vector)
                action(item);

            return this;
        }

        public T[] GetArray()
        {
            return _vector.ToArray();
        }

        public List<T> GetList()
        {
            return _vector;
        }

        #region IEnumerable implementation
        public IEnumerator<T> GetEnumerator()
        {
            return _vector.GetEnumerator();
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