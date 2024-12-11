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
            MaxCol = rawGrid[0].Length;
            MaxRow = rawGrid.Count;

            Grid = new char[MaxCol, MaxRow];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    Grid[_c, _r] = rawGrid[_r][_c];
                }
            }
        }

        private void Print(Base.Vec2 next = null, Util.Grid2.Dir dir = Util.Grid2.Dir.None)
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
                        sb.Append(At(_c, _r));
                    }
                }
                Core.Log.WriteLine(Core.Log.ELevel.Spam, sb.ToString());
            }
        }
    }

    public class Grid2<T> : IEnumerable
    {
        public T[,] Grid { get; protected set; }
        public int MaxCol { get; protected set; }
        public int MaxRow { get; protected set; }

        public Grid2()
        {
            MaxCol = default;
            MaxRow = default;
            Grid = default;
        }

        public bool Has(Base.Vec2 vec2)
        {
            return vec2.X >= 0 && vec2.X < MaxCol && vec2.Y >= 0 && vec2.Y < MaxRow;
        }

        public bool Has(int col, int row)
        {
            return col >= 0 && col < MaxCol && row >= 0 && row < MaxRow;
        }

        public T At(Base.Vec2 vec2)
        {
            return Grid[vec2.X, vec2.Y];
        }

        public T At(int col, int row)
        {
            return Grid[col, row];
        }

        public void Print(Core.Log.ELevel level)
        {
            StringBuilder sb = new();
            Core.Log.WriteLine(level, $"Printing grid {MaxCol}x{MaxRow}:");
            for (int _r = 0; _r < MaxRow; ++_r)
            {
                sb.Clear();
                sb.Append($"{_r,4}| ");
                sb.Append(string.Join(string.Empty, Enumerable.Range(0, MaxCol).Select(col => Grid[col, _r])));
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