using System;

namespace Runtime.Navigation
{
    public interface IHeapItem<T> : IComparable<T>
    {
        #region Properties

        int HeapIndex { get; set; }

        #endregion Properties
    }

    public class Heap<T> where T : IHeapItem<T>
    {
        #region Members

        private T[] _elements;
        private int _elementsCount;

        #endregion Members

        #region Properties

        public int Count => _elementsCount;

        #endregion Properties

        #region Class Methods

        public Heap(int maxHeapSize) => _elements = new T[maxHeapSize];
        public bool Contains(T item) => Equals(_elements[item.HeapIndex], item);

        public void Add(T item)
        {
            item.HeapIndex = _elementsCount;
            _elements[_elementsCount] = item;
            SortUp(item);
            _elementsCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = _elements[0];
            _elementsCount--;
            _elements[0] = _elements[_elementsCount];
            _elements[0].HeapIndex = 0;
            SortDown(_elements[0]);
            return firstItem;
        }

        private void SortUp(T item)
        {
            int parentIndex = ( item.HeapIndex - 1 ) / 2;
            while (true)
            {
                T parentItem = _elements[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = item.HeapIndex * 2 + 1;
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = 0;
                if (childIndexLeft < _elementsCount)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < _elementsCount)
                    {
                        if (_elements[childIndexLeft].CompareTo(_elements[childIndexRight]) < 0)
                            swapIndex = childIndexRight;
                    }

                    if (item.CompareTo(_elements[swapIndex]) < 0)
                        Swap(item, _elements[swapIndex]);
                    else
                        return;
                }
                else return;
            }
        }

        private void Swap(T itemA, T itemB)
        {
            _elements[itemA.HeapIndex] = itemB;
            _elements[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }

        #endregion Class Methods
    }
}