using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MathChart.ListG
{
    public delegate T ItemAction<T>(T obj);
    public class EnumeratorG<T> : IEnumerator<T>
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
            if(typeof(IDisposable).IsSubclassOf(typeof(T)))
                foreach (var el in _list)
                    if ((el as IDisposable) != null)
                        (el as IDisposable).Dispose();
        }
    }

    public class ListG<T> : IList<T>
    {

        public event Action ItemAdded;
        public event Action<T,int>  ItemChanged;
        public event Action<T, int> ItemTaken;
        public event Action ItemDeleted;
        public event Action CollectionChanged;

        private T[] _list;
        private int count;
        public ListG()
        {
            _list = new T[0];
            count = 0;
        }

        public ListG(T[] array) : this()
        {
            _list = new T[array.Length];
            array.CopyTo(_list, 0);
            count = array.Length;
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
            if (ItemAdded != null)
                ItemAdded.Invoke();
            if (CollectionChanged != null)
                CollectionChanged.Invoke();
        }

        public void Clear()
        {
            if (typeof(IDisposable).IsSubclassOf(typeof(T)))
                foreach (var el in _list)
                    if((el as IDisposable) != null)
                        (el as IDisposable).Dispose();
            if (ItemDeleted != null)
                for (int i = 0; i < count; i++)
                    ItemDeleted.Invoke();
            _list = new T[0];
            count = 0;
            if (CollectionChanged != null)
                CollectionChanged.Invoke();
        }

        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < count; i++)
                    if ((Object)_list[i] == null)
                        return true;
                return false;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (c.Equals(_list[i], item)) return true;
                }
                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex + count < array.Length)
                throw new IndexOutOfRangeException("CopyTo: array index bigger then list size");
            try
            {
                for (int i = arrayIndex; i < count + arrayIndex; i++)
                {
                    if (typeof(ICloneable).IsSubclassOf(typeof(T)))
                        array[i] = (T)(_list[i - arrayIndex] as ICloneable).Clone();
                    else
                        array[i] = _list[i - arrayIndex];
                }
            }
            catch (Exception e) { throw new Exception("CopyTo error: " + e.Message); }

        }

        public bool Remove(T item)
        {
            if (ItemDeleted != null)
                ItemDeleted.Invoke();

            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                CollectionChanged.Invoke();
            }
            else return false;

            return true;

        }

        #endregion

        #region Implementation of IList<T>
        public T this[int index]
        {
            get
            {
                try
                {
                    if (ItemTaken != null)
                        ItemTaken.Invoke(_list[index], index);
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
                    if (ItemChanged != null)
                        ItemChanged.Invoke(_list[index], index);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public int IndexOf(T item)
        {
            EqualityComparer<T> c = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++)
            {
                if (c.Equals(_list[i], item)) return i;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException("ListG.Insert");
            try
            {
                if (ItemAdded != null)
                    ItemAdded.Invoke();
                T[] array = new T[count + 1];
                for (int i = 0; i < index; i++)
                {
                    if (typeof(ICloneable).IsSubclassOf(typeof(T)))
                        array[i] = (T)(_list[i] as ICloneable).Clone();
                    else
                        array[i] = _list[i];
                }

                if (typeof(ICloneable).IsSubclassOf(typeof(T)))
                    array[index] = (T)(item as ICloneable).Clone();
                else
                    array[index] = item;

                for (int i = index + 1; i < count; i++)
                {
                    array[i + 1] = _list[i];
                }
                if (typeof(IDisposable).IsSubclassOf(typeof(T)))
                    foreach (var el in _list)
                        (el as IDisposable).Dispose();

                _list = array;
                if (CollectionChanged != null)
                    CollectionChanged.Invoke();
            }
            catch (Exception e) { throw new Exception("InsertAt error: " + e.Message); }

        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("RemoveAt.index");

            try
            {
                if (ItemDeleted != null)
                    ItemDeleted.Invoke();
                T[] array = new T[count - 1];
                for (int i = 0; i < index; i++)
                {
                    if (typeof(ICloneable).IsSubclassOf(typeof(T)))
                        array[i] = (T)(_list[i] as ICloneable).Clone();
                    else
                        array[i] = _list[i];
                }
                for (int i = index + 1; i < count; i++)
                {
                    array[i - 1] = _list[i];
                }
                if (typeof(IDisposable).IsSubclassOf(typeof(T)))
                    foreach (var el in _list)
                        (el as IDisposable).Dispose();

                _list = array;
                if (CollectionChanged != null)
                    CollectionChanged.Invoke();
            }
            catch (Exception e) { throw new Exception("RemoveAt error: " + e.Message); }
        }

        #endregion

        #region Other things

        public void Refresh()
        {
            if(CollectionChanged!=null)
                CollectionChanged.Invoke();
        }

        #endregion

        public override string ToString()
        {
            return "ListG: " + count.ToString() + " elements";
        }

    }
}
