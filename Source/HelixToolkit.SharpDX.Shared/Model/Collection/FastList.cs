// Modified from xenko fast list
// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// Similar to <see cref="List{T}"/>, with direct access to underlying array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class FastList<T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        // Fields
        private const int _defaultCapacity = 4;

        /// <summary>
        /// Gets the items from internal array. Make sure to access this array using <see cref="Count"/> instead of Array Length
        /// </summary>
        internal T[] Items { get; private set; }
        private static readonly T[] empty = new T[0];
        private int _size;

        public FastList()
        {
            Items = empty;
        }

        public FastList(IEnumerable<T> collection)
        {
            if (collection is ICollection<T> is2)
            {
                var count = is2.Count;
                Items = new T[count];
                is2.CopyTo(Items, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                Items = new T[_defaultCapacity];
                using (var enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        public FastList(int capacity)
        {
            Items = new T[capacity];
        }

        public int Capacity
        {
            get { return Items.Length; }
            set
            {
                if (value != Items.Length)
                {
                    if (value > 0)
                    {
                        var destinationArray = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(Items, 0, destinationArray, 0, _size);
                        }
                        Items = destinationArray;
                    }
                    else
                    {
                        Items = empty;
                    }
                }
            }
        }

        #region IList<T> Members

        public void Add(T item)
        {
            if (_size == Items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            Items[_size++] = item;
        }

        public void IncreaseCapacity(int index)
        {
            EnsureCapacity(_size + index);
            _size += index;
        }

        public void Clear()
        {
            Clear(false);
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                for (var j = 0; j < _size; j++)
                {
                    if (Items[j] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            var comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < _size; i++)
            {
                if (comparer.Equals(Items[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(Items, 0, array, arrayIndex, _size);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(Items, item, 0, _size);
        }

        public void Insert(int index, T item)
        {
            if (_size == Items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            if (index < _size)
            {
                Array.Copy(Items, index, Items, index + 1, _size - index);
            }
            Items[index] = item;
            _size++;
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size) throw new ArgumentOutOfRangeException(nameof(index));
            _size--;
            if (index < _size)
            {
                Array.Copy(Items, index + 1, Items, index, _size - index);
            }
            Items[_size] = default(T);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int Count => _size;

        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }

        bool ICollection<T>.IsReadOnly => false;

        #endregion

        /// <summary>
        /// Clears this list with a fast-clear option.
        /// </summary>
        /// <param name="fastClear">if set to <c>true</c> this method only resets the count elements but doesn't clear items referenced already stored in the list.</param>
        public void Clear(bool fastClear)
        {
            Resize(0, fastClear);
        }

        public void Resize(int newSize, bool fastClear)
        {
            if (_size < newSize)
            {
                EnsureCapacity(newSize);
            }
            else if (!fastClear && _size - newSize > 0)
            {
                Array.Clear(Items, newSize, _size - newSize);
            }

            _size = newSize;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_size, collection);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return Array.BinarySearch(Items, index, count, item, comparer);
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            Array.Copy(Items, index, array, arrayIndex, count);
        }

        public void EnsureCapacity(int min)
        {
            if (Items.Length < min)
            {
                var num = (Items.Length == 0) ? _defaultCapacity : (Items.Length*2);
                if (num < min)
                {
                    num = min;
                }
                Capacity = num;
            }
        }

        public bool Exists(Predicate<T> match)
        {
            return (FindIndex(match) != -1);
        }

        public T Find(Predicate<T> match)
        {
            for (var i = 0; i < _size; i++)
            {
                if (match(Items[i]))
                {
                    return Items[i];
                }
            }
            return default(T);
        }

        
        public FastList<T> FindAll(Predicate<T> match)
        {
            var list = new FastList<T>();
            for (var i = 0; i < _size; i++)
            {
                if (match(Items[i]))
                {
                    list.Add(Items[i]);
                }
            }
            return list;
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, _size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, _size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            var num = startIndex + count;
            for (var i = startIndex; i < num; i++)
            {
                if (match(Items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public T FindLast(Predicate<T> match)
        {
            for (var i = _size - 1; i >= 0; i--)
            {
                if (match(Items[i]))
                {
                    return Items[i];
                }
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(_size - 1, _size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            var num = startIndex - count;
            for (var i = startIndex; i > num; i--)
            {
                if (match(Items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void ForEach(Action<T> action)
        {
            for (var i = 0; i < _size; i++)
            {
                action(Items[i]);
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        
        public FastList<T> GetRange(int index, int count)
        {
            var list = new FastList<T>(count);
            Array.Copy(Items, index, list.Items, 0, count);
            list._size = count;
            return list;
        }

        public int IndexOf(T item, int index)
        {
            return Array.IndexOf(Items, item, index, _size - index);
        }

        public int IndexOf(T item, int index, int count)
        {
            return Array.IndexOf(Items, item, index, count);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection is ICollection<T> is2)
            {
                var count = is2.Count;
                if (count > 0)
                {
                    EnsureCapacity(_size + count);
                    if (index < _size)
                    {
                        Array.Copy(Items, index, Items, index + count, _size - index);
                    }
                    if (this == is2)
                    {
                        Array.Copy(Items, 0, Items, index, index);
                        Array.Copy(Items, (index + count), Items, (index * 2), (_size - index));
                    }
                    else
                    {
                        is2.CopyTo(Items, index);
                    }
                    _size += count;
                }
            }
            else
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Insert(index++, enumerator.Current);
                    }
                }
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            return value is T || value == null && default(T) == null;
        }

        public int LastIndexOf(T item)
        {
            if (_size == 0)
            {
                return -1;
            }
            return LastIndexOf(item, _size - 1, _size);
        }

        public int LastIndexOf(T item, int index)
        {
            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            if (_size == 0)
            {
                return -1;
            }
            return Array.LastIndexOf(Items, item, index, count);
        }

        public int RemoveAll(Predicate<T> match)
        {
            var index = 0;
            while ((index < _size) && !match(Items[index]))
            {
                index++;
            }
            if (index >= _size)
            {
                return 0;
            }
            var num2 = index + 1;
            while (num2 < _size)
            {
                while ((num2 < _size) && match(Items[num2]))
                {
                    num2++;
                }
                if (num2 < _size)
                {
                    Items[index++] = Items[num2++];
                }
            }
            Array.Clear(Items, index, _size - index);
            var num3 = _size - index;
            _size = index;
            return num3;
        }

        public void RemoveRange(int index, int count)
        {
            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(Items, index + count, Items, index, _size - index);
                }
                Array.Clear(Items, _size, count);
            }
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            Array.Reverse(Items, index, count);
        }

        public void Sort()
        {
            Array.Sort(Items, 0, Count);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        //public void Sort(Comparison<T> comparison)
        //{
        //    if (this._size > 0)
        //    {
        //        IComparer<T> comparer = new Array.FunctorComparer<T>(comparison);
        //        Array.Sort<T>(this.Items, 0, this._size, comparer);
        //    }
        //}

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Array.Sort(Items, index, count, comparer);
        }

        
        public T[] ToArray()
        {
            var destinationArray = new T[_size];
            Array.Copy(Items, 0, destinationArray, 0, _size);
            return destinationArray;
        }

        public void TrimExcess()
        {
            if (Count == Capacity)
            {
                return;
            }
            var curr = Items;
            Items = Count == 0 ? empty : new T[Count];
            if (Count > 0)
            {
                Array.Copy(curr, 0, Items, 0, Count);
            }
            Capacity = Count;
        }

        public bool TrueForAll(Predicate<T> match)
        {
            for (var i = 0; i < _size; i++)
            {
                if (!match(Items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        // Properties

        // Nested Types

        #region Nested type: Enumerator

        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private readonly FastList<T> list;
            private int index;
            private T current;

            internal Enumerator(FastList<T> list)
            {
                this.list = list;
                index = 0;
                current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var list = this.list;
                if (index < list._size)
                {
                    current = list.Items[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                index = list._size + 1;
                current = default(T);
                return false;
            }

            public T Current => current;

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                index = 0;
                current = default(T);
            }
        }
        #endregion

        /// <summary>
        /// Fast add all from another <see cref="FastList{T}"/>.
        /// </summary>
        /// <param name="list">The list.</param>
        public void AddAll(FastList<T> list)
        {
            EnsureCapacity(_size + list.Count);
            Array.Copy(list.Items, 0, Items, Count, list.Count);
            _size += list.Count;
        }

        /// <summary>
        /// Gets the internal array used to hold data.
        /// </summary>
        /// <returns></returns>
        public T[] GetInternalArray()
        {
            return Items;
        }
    }
}
