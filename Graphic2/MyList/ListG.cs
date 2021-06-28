using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphic.MyList
{

    class EnumeratorG<T> : IEnumerator<T>
    {
        private T[] _list;
        private int count;
        private int position = -1;

        public EnumeratorG(T[] _list, int count)
        {
            this._list = _list;
            this.count = count;
        }
            
        public T Current
        {
            get
            {
                try
                {
                    return _list[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if (position < count - 1)
            {
                position++;
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            
        }
    }

    public class ListG<T> : IList<T>
    {

        private T[] _list;
        private int count;
        public ListG()
        {
            _list = new T[0];
            count = 0;
        }

        #region Implementation of IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorG<T>(_list, count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public int Count
        {
            get { return count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public void Add(T item)
        {
            T[] buffer = _list;
            _list = new T[count + 1];
            for (int i = 0; i < count; i++)
                _list[i] = buffer[i];
            _list[count] = item;
            count++;
        }

        public void Clear()
        {
            _list = new T[0];
            count = 0;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Implementation of IList<T>
        public T this[int index] 
        {
            get
            {
                try
                {
                    return _list[index];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
            set
            {
                try
                {
                    _list[index] = value;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
      

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }  

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            return "List: "+ count.ToString() + " elements";
        }

    }
}
