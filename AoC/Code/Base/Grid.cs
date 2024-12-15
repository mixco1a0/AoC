using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Base
{
    #region Grid2
    public class Grid2Char : Grid2<char>
    {
        public Grid2Char(List<string> rawGrid)
        {
            m_array = new char[rawGrid.Count, rawGrid.First().Length];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    m_array[_r, _c] = rawGrid[_r][_c];
                }
            }
        }

        public Grid2Char(int maxCol, int maxRow, char defaultValue)
        {
            m_array = new char[maxRow, maxCol];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    m_array[_r, _c] = defaultValue;
                }
            }
        }

        public override void PrintNextArrow(Core.Log.ELevel level, Base.Vec2 next = null, Util.Grid2.Dir dir = Util.Grid2.Dir.None)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(Core.Log.ELevel.Spam, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                for (int _c = 0; _c < MaxCol; ++_c)
                {
                    if (next != null && dir != Util.Grid2.Dir.None && next.X == _c && next.Y == _r)
                    {
                        sb.Append(Util.Grid2.Map.Arrow[dir]);
                    }
                    else
                    {
                        sb.Append(m_array[_r, _c]);
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Spam, sb.ToString());
            }
        }
    }

    public class Grid2Bool : Grid2<bool>
    {
        public Grid2Bool(List<string> rawGrid)
        {
            m_array = new bool[rawGrid.Count, rawGrid.First().Length];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    m_array[_r, _c] = false;
                }
            }
        }

        public override void Print(Core.Log.ELevel level)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(Core.Log.ELevel.Spam, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                for (int _c = 0; _c < MaxCol; ++_c)
                {
                    sb.Append(m_array[_r, _c] ? '#' : '.');
                }
                Core.Log.WriteLine(Core.Log.ELevel.Spam, sb.ToString());
            }
        }
    }
    
    public class Grid2Int : Grid2<int>
    {
        public Grid2Int(int maxCol, int maxRow)
        {
            m_array = new int[maxRow, maxCol];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    m_array[_r, _c] = 0;
                }
            }
        }

        public Grid2Int(List<string> rawGrid)
        {
            m_array = new int[rawGrid.Count, rawGrid.First().Length];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    m_array[_r, _c] = 0;
                }
            }
        }

        public override void Print(Core.Log.ELevel level)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(Core.Log.ELevel.Spam, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                for (int _c = 0; _c < MaxCol; ++_c)
                {
                    sb.Append(m_array[_r, _c] == 0 ? '.' : m_array[_r, _c].ToString());
                }
                Core.Log.WriteLine(Core.Log.ELevel.Spam, sb.ToString());
            }
        }
    }

    public class Grid2<T> : IEnumerable
    {
        protected T[,] m_array;
        public int MaxCol => m_array.GetLength(1);
        public int MaxRow => m_array.GetLength(0);

        public Grid2()
        {
            m_array = default;
        }

        public Grid2(T[,] array)
        {
            m_array = array;
        }

        public bool Contains(Base.Vec2 vec2)
        {
            return Contains(vec2.X, vec2.Y);
        }

        public bool Contains(int col, int row)
        {
            return col >= 0 && col < MaxCol && row >= 0 && row < MaxRow;
        }

        public T this[int col, int row]
        {
            get => m_array[row, col];
            set => m_array[row, col] = value;
        }

        public T this[Base.Vec2 vec2]
        {
            get => m_array[vec2.Y, vec2.X];
            set => m_array[vec2.Y, vec2.X] = value;
        }

        public virtual void Print(Core.Log.ELevel level)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, MaxCol).Select(_c => m_array[_r, _c])));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        public virtual void PrintNextArrow(Core.Log.ELevel level, Base.Vec2 next = null, Util.Grid2.Dir dir = Util.Grid2.Dir.None)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, MaxCol).Select(_c => m_array[_r, _c])));
                Core.Log.WriteLine(level, sb.ToString());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<Vec2> GetEnumerator()
        {
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    yield return new(_c, _r);
                }
            }
        }
    }
    #endregion
}