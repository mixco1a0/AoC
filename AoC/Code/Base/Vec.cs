using System;
using System.Linq;
using System.Numerics;

namespace AoC.Base
{
    #region Pos2
    public class Pos2 : IEquatable<Pos2>, IComparable<Pos2>, IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        private static Pos2 _zero;
        public static Pos2 Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos2();
                return _zero;
            }
        }

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

        public static Pos2 Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            int[] split = Util.String.Split(input, ',').Select(int.Parse).ToArray();
            return new Pos2(split[0], split[1]);
        }

        public static Pos2 operator +(Pos2 a, Pos2 b)
        {
            return new Pos2(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2 operator -(Pos2 a, Pos2 b)
        {
            return new Pos2(a.X - b.X, a.Y - b.Y);
        }

        public static Pos2 operator *(Pos2 a, int mult)
        {
            return new Pos2(a.X * mult, a.Y * mult);
        }

        public static Pos2 operator /(Pos2 a, int mult)
        {
            if (mult == 0)
            {
                return new Pos2();
            }
            return new Pos2(a.X / mult, a.Y / mult);
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

        public virtual int CompareTo(Pos2 other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public int CompareTo(object other)
        {
            Pos2 otherAsPos2 = other as Pos2;
            if (otherAsPos2 == null)
            {
                return -1;
            }
            return otherAsPos2.CompareTo(other);
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

            Pos2 objAsPos2 = obj as Pos2;
            if (objAsPos2 == null)
            {
                return false;
            }

            return Equals(objAsPos2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion


    #region Pos2L
    public class Pos2L : IEquatable<Pos2L>, IComparable<Pos2L>, IComparable
    {
        public long X { get; set; }
        public long Y { get; set; }

        private static Pos2L _zero;
        public static Pos2L Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos2L();
                return _zero;
            }
        }

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

        public static Pos2L Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            long[] split = Util.String.Split(input, ',').Select(long.Parse).ToArray();
            return new Pos2L(split[0], split[1]);
        }

        public static Pos2L operator +(Pos2L a, Pos2L b)
        {
            return new Pos2L(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2L operator -(Pos2L a, Pos2L b)
        {
            return new Pos2L(a.X - b.X, a.Y - b.Y);
        }

        public static Pos2L operator *(Pos2L a, long mult)
        {
            return new Pos2L(a.X * mult, a.Y * mult);
        }

        public static Pos2L operator /(Pos2L a, long mult)
        {
            if (mult == 0)
            {
                return new Pos2L();
            }
            return new Pos2L(a.X / mult, a.Y / mult);
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

        public virtual int CompareTo(Pos2L other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public int CompareTo(object other)
        {
            Pos2L otherAsPos2 = other as Pos2L;
            if (otherAsPos2 == null)
            {
                return -1;
            }
            return otherAsPos2.CompareTo(other);
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

            Pos2L objAsPos2L = obj as Pos2L;
            if (objAsPos2L == null)
            {
                return false;
            }

            return Equals(objAsPos2L);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion


    #region Pos2BI
    public class Pos2BI : IEquatable<Pos2BI>, IComparable<Pos2BI>, IComparable
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        private static Pos2BI _zero;
        public static Pos2BI Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos2BI();
                return _zero;
            }
        }

        public Pos2BI()
        {
            X = default;
            Y = default;
        }

        public Pos2BI(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public Pos2BI(Pos2BI other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Pos2BI Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            BigInteger[] split = Util.String.Split(input, ',').Select(BigInteger.Parse).ToArray();
            return new Pos2BI(split[0], split[1]);
        }

        public static Pos2BI operator +(Pos2BI a, Pos2BI b)
        {
            return new Pos2BI(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2BI operator -(Pos2BI a, Pos2BI b)
        {
            return new Pos2BI(a.X - b.X, a.Y - b.Y);
        }

        public static Pos2BI operator *(Pos2BI a, BigInteger mult)
        {
            return new Pos2BI(a.X * mult, a.Y * mult);
        }

        public static Pos2BI operator /(Pos2BI a, BigInteger mult)
        {
            if (mult == 0)
            {
                return new Pos2BI();
            }
            return new Pos2BI(a.X / mult, a.Y / mult);
        }

        public BigInteger Manhattan(Pos2BI other)
        {
            return Util.Number.Abs(X - other.X) + Util.Number.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Pos2BI other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Pos2BI other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public int CompareTo(object other)
        {
            Pos2BI otherAsPos2BI = other as Pos2BI;
            if (otherAsPos2BI == null)
            {
                return -1;
            }
            return otherAsPos2BI.CompareTo(other);
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

            Pos2BI objAsPos2BI = obj as Pos2BI;
            if (objAsPos2BI == null)
            {
                return false;
            }

            return Equals(objAsPos2BI);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion


    #region Vec2F
    public class Vec2F : IEquatable<Vec2F>, IComparable<Vec2F>, IComparable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static readonly Vec2F Zero = new();

        public Vec2F()
        {
            X = default;
            Y = default;
        }

        public Vec2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2F(Vec2F other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Vec2F Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            float[] split = Util.String.Split(input, ',').Select(float.Parse).ToArray();
            return new(split[0], split[1]);
        }

        public static Vec2F operator +(Vec2F a, Vec2F b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2F operator -(Vec2F a, Vec2F b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2F operator *(Vec2F a, float mult)
        {
            return new(a.X * mult, a.Y * mult);
        }

        public static Vec2F operator /(Vec2F a, float mult)
        {
            if (mult == 0.0f)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult);
        }

        public float Manhattan(Vec2F other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Vec2F other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Vec2F other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public int CompareTo(object other)
        {
            if (other is not Vec2F otherAsVec)
            {
                return -1;
            }
            return otherAsVec.CompareTo(other);
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

            if (obj is not Vec2F objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion


    #region Vec2D
    public class Vec2D : IEquatable<Vec2D>, IComparable<Vec2D>, IComparable
    {
        public double X { get; set; }
        public double Y { get; set; }

        private static Vec2D _zero;
        public static readonly Vec2D Zero = new();

        public Vec2D()
        {
            X = default;
            Y = default;
        }

        public Vec2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vec2D(Vec2D other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Vec2D Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            double[] split = Util.String.Split(input, ',').Select(double.Parse).ToArray();
            return new(split[0], split[1]);
        }

        public static Vec2D operator +(Vec2D a, Vec2D b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2D operator -(Vec2D a, Vec2D b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2D operator *(Vec2D a, double mult)
        {
            return new(a.X * mult, a.Y * mult);
        }

        public static Vec2D operator /(Vec2D a, double mult)
        {
            if (mult == 0.0d)
            {
                return new Vec2D();
            }
            return new(a.X / mult, a.Y / mult);
        }

        public double Manhattan(Vec2D other)
        {
            return double.Abs(X - other.X) + double.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Vec2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Vec2D other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public int CompareTo(object other)
        {
            if (other is not Vec2D otherAsVec)
            {
                return -1;
            }
            return otherAsVec.CompareTo(other);
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

            if (obj is not Vec2D objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion
    

    #region Vec3
    public class Vec3 : IEquatable<Vec3>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public static readonly Vec3 Zero = new();

        public Vec3()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Vec3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(Vec3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vec3 Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            int[] split = Util.String.Split(input, ',').Select(int.Parse).ToArray();
            return new(split[0], split[1], split[2]);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vec3 operator *(Vec3 a, int mult)
        {
            return new(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Vec3 operator /(Vec3 a, int mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public int Manhattan(Vec3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public Pos2 DropZ()
        {
            return new(X, Y);
        }

        #region Interfaces
        public bool Equals(Vec3 other)
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

            if (obj is not Vec3 objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion


    #region Vec3L
    public class Vec3L : IEquatable<Vec3L>
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public static readonly Vec3L Zero = new();

        public Vec3L()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Vec3L(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3L(Vec3L other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vec3L Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            long[] split = Util.String.Split(input, ',').Select(long.Parse).ToArray();
            return new(split[0], split[1], split[2]);
        }

        public static Vec3L operator +(Vec3L a, Vec3L b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3L operator -(Vec3L a, Vec3L b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vec3L operator *(Vec3L a, long mult)
        {
            return new(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Vec3L operator /(Vec3L a, long mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public long Manhattan(Vec3L other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public Pos2L DropZ()
        {
            return new(X, Y);
        }

        #region Interfaces
        public bool Equals(Vec3L other)
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

            if (obj is not Vec3L objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion


    #region Vec3BI
    public class Vec3BI : IEquatable<Vec3BI>
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }
        public BigInteger Z { get; set; }

        public static readonly Vec3BI Zero = new();

        public Vec3BI()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Vec3BI(BigInteger x, BigInteger y, BigInteger z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3BI(Vec3BI other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vec3BI Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            BigInteger[] split = Util.String.Split(input, ',').Select(BigInteger.Parse).ToArray();
            return new(split[0], split[1], split[2]);
        }

        public static Vec3BI operator +(Vec3BI a, Vec3BI b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3BI operator -(Vec3BI a, Vec3BI b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vec3BI operator *(Vec3BI a, BigInteger mult)
        {
            return new(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Vec3BI operator /(Vec3BI a, BigInteger mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public BigInteger Manhattan(Vec3BI other)
        {
            return Util.Number.Abs(X - other.X) + Util.Number.Abs(Y - other.Y) + Util.Number.Abs(Z - other.Z);
        }

        public Pos2BI DropZ()
        {
            return new(X, Y);
        }

        #region Interfaces
        public bool Equals(Vec3BI other)
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

            if (obj is not Vec3BI objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion


    #region Vec3F
    public class Vec3F : IEquatable<Vec3F>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static readonly Vec3F Zero = new();

        public Vec3F()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Vec3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3F(Vec3F other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vec3F Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            float[] split = Util.String.Split(input, ',').Select(float.Parse).ToArray();
            return new(split[0], split[1], split[2]);
        }

        public static Vec3F operator +(Vec3F a, Vec3F b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3F operator -(Vec3F a, Vec3F b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vec3F operator *(Vec3F a, float mult)
        {
            return new(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Vec3F operator /(Vec3F a, float mult)
        {
            if (mult == 0.0f)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public float Manhattan(Vec3F other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public Vec2F DropZ()
        {
            return new(X, Y);
        }

        #region Interfaces
        public bool Equals(Vec3F other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format("[{0:00.000}, {1:00.000}, {2:00.000}]", X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not Vec3F objAsVec)
            {
                return false;
            }

            return Equals(objAsVec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion
}