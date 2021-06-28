using System;
using System.Collections;
using System.Collections.Generic;

namespace Graphic.MyList
{
    public class MyList<T> : IList<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemChanged;
        public event Action<T> ItemDeleted;
        public event Action<int> CollectionChanged;
        
        private readonly IList<T> _list = new List<T>();

        #region Implementation of IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public void Add(T item)
        {
            if(ItemAdded!=null)
                ItemAdded.Invoke(item);
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (ItemDeleted != null) 
                ItemDeleted.Invoke(item);
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            CollectionChanged.Invoke(index);
        }

        public void RemoveAt(int index)
        {
            if (ItemDeleted != null)
                ItemDeleted.Invoke(_list[index]);
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get 
            {
                return _list[index]; 
            }
            set 
            {
                _list[index] = value;
                if (ItemChanged != null)
                    ItemChanged.Invoke(_list[index]);
            }
        }

        #endregion

        #region Your Added Stuff

        public void Update()
        {
            foreach(T _obj in _list)
            {
                if (ItemChanged != null)
                    ItemChanged.Invoke(_obj);
            }
        }

        #endregion
    }
}
