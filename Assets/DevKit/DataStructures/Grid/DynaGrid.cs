using System;
using System.Collections.Generic;

namespace DataStructures
{
    /// <summary>
    /// 动态grid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynaGrid<T>
    {
        private Dictionary<Tuple<int, int>, T> mGrid = null;

        public DynaGrid()
        {
            mGrid = new Dictionary<Tuple<int, int>, T>();
        }

        public void ForEach(Action<int, int, T> each)
        {
            foreach (var kvp in mGrid)
            {
                each(kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
            }
        }

        public void ForEach(Action<T> each)
        {
            foreach (var kvp in mGrid)
            {
                each(kvp.Value);
            }
        }
        
        public T this[int xIndex, int yIndex]
        {
            get
            {
                var key = new Tuple<int, int>(xIndex, yIndex);
                return mGrid.TryGetValue(key, out var value) ? value : default;
            }
            set
            {
                var key = new Tuple<int, int>(xIndex, yIndex);
                mGrid[key] = value;
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            mGrid.Clear();
        }
    }
}