using System;

namespace AoC.Base
{
    public class Position<T> : CPair<T, T>
        where T : IComparable
    {
        public T X { get => m_first; set => m_first = value; }
        public T Y { get => m_last; set => m_last = value; }
        public bool SortByX { get { return m_sortByFirst; } set { m_sortByFirst = value; } }

        public Position() : base() { }

        public Position(T x, T y) : base(x, y) { }

        public Position(Position<T> other) : base(other) { }

        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }
    }

    public class Position : Point<int>
    {
        public Position() : base() { }
        public Position(int x, int y) : base(x, y) { }
        public Position(Position other) : base(other) { }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public int Manhattan(Position other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }
    }
}