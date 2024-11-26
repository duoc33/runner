using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataStructures
{
    /// <summary>
    /// Grid 网格 ， 
    /// Grid[x,y] = T ,  T = Grid[x,y];
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T>
    {
        private T[,] mGrid;
        private int mWidth;
        private int mHeight;

        public int Width => mWidth;
        public int Height => mHeight;

        public Grid(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mGrid = new T[width, height];
        }

        
        public void Fill(T t)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = t;
                }
            }
        }

        public void Fill(Func<int, int, T> onFill)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = onFill(x, y);
                }
            }
        }

        public void Resize(int width, int height, Func<int, int, T> onAdd)
        {
            var newGrid = new T[width, height];
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    newGrid[x, y] = mGrid[x, y];
                }

                // x addition
                for (var y = mHeight; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            for (var x = mWidth;  x < width; x++)
            {
                // y addition
                for (var y = 0; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            // 清空之前的
            Fill(default(T));

            mWidth = width;
            mHeight = height;
            mGrid = newGrid;
        }

        public void ForEach(Action<int, int, T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(x, y, mGrid[x, y]);
                }
            }
        }

        public void ForEach(Action<T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(mGrid[x, y]);
                }
            }
        }

        public T this[int xIndex, int yIndex]
        {
            get
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    return mGrid[xIndex, yIndex];
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                    return default;
                }
            }
            set
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    mGrid[xIndex, yIndex] = value;
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                }
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    cleanupItem?.Invoke(mGrid[x, y]);
                    mGrid[x, y] = default;
                }
            }

            mGrid = null;
        }
    }
}