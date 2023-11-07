using System;

namespace AoC.Base
{
    public class Pos2 : IEquatable<Pos2>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Pos2()
        {
            X = default;
            Y = default;
        }

        public Pos2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Pos2(Pos2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Pos2 operator +(Pos2 a, Pos2 b)
        {
            return new Pos2(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2 operator -(Pos2 a, Pos2 b)
        {
            return new Pos2(a.X - b.X, a.Y - b.Y);
        }

        public int Manhattan(Pos2 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Pos2 other)
        {
            return X == other.X && Y == other.Y;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Pos2 objAsPos = obj as Pos2;
            if (objAsPos == null)
            {
                return false;
            }

            return Equals(objAsPos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }

    public class Pos2L : IEquatable<Pos2L>
    {
        public long X { get; set; }
        public long Y { get; set; }

        public Pos2L()
        {
            X = default;
            Y = default;
        }

        public Pos2L(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Pos2L(Pos2L other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Pos2L operator +(Pos2L a, Pos2L b)
        {
            return new Pos2L(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2L operator -(Pos2L a, Pos2L b)
        {
            return new Pos2L(a.X - b.X, a.Y - b.Y);
        }

        public long Manhattan(Pos2L other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Pos2L other)
        {
            return X == other.X && Y == other.Y;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Pos2L objAsPos = obj as Pos2L;
            if (objAsPos == null)
            {
                return false;
            }

            return Equals(objAsPos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }

    public class Pos3 : IEquatable<Pos3>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Pos3()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Pos3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Pos3(Pos3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Pos3 operator +(Pos3 a, Pos3 b)
        {
            return new Pos3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Pos3 operator -(Pos3 a, Pos3 b)
        {
            return new Pos3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public int Manhattan(Pos3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        #region Interfaces
        public bool Equals(Pos3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Pos3 objAsPos = obj as Pos3;
            if (objAsPos == null)
            {
                return false;
            }

            return Equals(objAsPos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }

    public class Pos3L : IEquatable<Pos3L>
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public Pos3L()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Pos3L(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Pos3L(Pos3L other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Pos3L operator +(Pos3L a, Pos3L b)
        {
            return new Pos3L(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Pos3L operator -(Pos3L a, Pos3L b)
        {
            return new Pos3L(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public long Manhattan(Pos3L other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        #region Interfaces
        public bool Equals(Pos3L other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Pos3L objAsPos = obj as Pos3L;
            if (objAsPos == null)
            {
                return false;
            }

            return Equals(objAsPos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
}