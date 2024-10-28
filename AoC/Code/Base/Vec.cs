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

            int[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
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

            long[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
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

            BigInteger[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(BigInteger.Parse).ToArray();
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


    #region Pos2F
    public class Pos2F : IEquatable<Pos2F>, IComparable<Pos2F>, IComparable
    {
        public float X { get; set; }
        public float Y { get; set; }

        private static Pos2F _zero;
        public static Pos2F Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos2F();
                return _zero;
            }
        }

        public Pos2F()
        {
            X = default;
            Y = default;
        }

        public Pos2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Pos2F(Pos2F other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Pos2F Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            float[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
            return new Pos2F(split[0], split[1]);
        }

        public static Pos2F operator +(Pos2F a, Pos2F b)
        {
            return new Pos2F(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2F operator -(Pos2F a, Pos2F b)
        {
            return new Pos2F(a.X - b.X, a.Y - b.Y);
        }

        public static Pos2F operator *(Pos2F a, float mult)
        {
            return new Pos2F(a.X * mult, a.Y * mult);
        }

        public static Pos2F operator /(Pos2F a, float mult)
        {
            if (mult == 0.0f)
            {
                return new Pos2F();
            }
            return new Pos2F(a.X / mult, a.Y / mult);
        }

        public float Manhattan(Pos2F other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Pos2F other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Pos2F other)
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
            Pos2F otherAsPos2F = other as Pos2F;
            if (otherAsPos2F == null)
            {
                return -1;
            }
            return otherAsPos2F.CompareTo(other);
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

            Pos2F objAsPos2F = obj as Pos2F;
            if (objAsPos2F == null)
            {
                return false;
            }

            return Equals(objAsPos2F);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion


    #region Pos2D
    public class Pos2D : IEquatable<Pos2D>, IComparable<Pos2D>, IComparable
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }

        private static Pos2D _zero;
        public static Pos2D Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos2D();
                return _zero;
            }
        }

        public Pos2D()
        {
            X = default;
            Y = default;
        }

        public Pos2D(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public Pos2D(Pos2D other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Pos2D Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            decimal[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(decimal.Parse).ToArray();
            return new Pos2D(split[0], split[1]);
        }

        public static Pos2D operator +(Pos2D a, Pos2D b)
        {
            return new Pos2D(a.X + b.X, a.Y + b.Y);
        }

        public static Pos2D operator -(Pos2D a, Pos2D b)
        {
            return new Pos2D(a.X - b.X, a.Y - b.Y);
        }

        public static Pos2D operator *(Pos2D a, decimal mult)
        {
            return new Pos2D(a.X * mult, a.Y * mult);
        }

        public static Pos2D operator /(Pos2D a, decimal mult)
        {
            if (mult == decimal.Zero)
            {
                return new Pos2D();
            }
            return new Pos2D(a.X / mult, a.Y / mult);
        }

        public decimal Manhattan(Pos2D other)
        {
            return decimal.Abs(X - other.X) + decimal.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Pos2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Pos2D other)
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
            Pos2D otherAsPos2D = other as Pos2D;
            if (otherAsPos2D == null)
            {
                return -1;
            }
            return otherAsPos2D.CompareTo(other);
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

            Pos2D objAsPos2D = obj as Pos2D;
            if (objAsPos2D == null)
            {
                return false;
            }

            return Equals(objAsPos2D);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion
    }
    #endregion
    

    #region Pos3
    public class Pos3 : IEquatable<Pos3>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        private static Pos3 _zero;
        public static Pos3 Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos3();
                return _zero;
            }
        }

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

        public static Pos3 Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            int[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            return new Pos3(split[0], split[1], split[2]);
        }

        public static Pos3 operator +(Pos3 a, Pos3 b)
        {
            return new Pos3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Pos3 operator -(Pos3 a, Pos3 b)
        {
            return new Pos3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Pos3 operator *(Pos3 a, int mult)
        {
            return new Pos3(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Pos3 operator /(Pos3 a, int mult)
        {
            if (mult == 0)
            {
                return new Pos3();
            }
            return new Pos3(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public int Manhattan(Pos3 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public Pos2 DropZ()
        {
            return new Pos2(X, Y);
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

            Pos3 objAsPos3 = obj as Pos3;
            if (objAsPos3 == null)
            {
                return false;
            }

            return Equals(objAsPos3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion


    #region Pos2L
    public class Pos3L : IEquatable<Pos3L>
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        private static Pos3L _zero;
        public static Pos3L Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos3L();
                return _zero;
            }
        }

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

        public static Pos3L Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            long[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            return new Pos3L(split[0], split[1], split[2]);
        }

        public static Pos3L operator +(Pos3L a, Pos3L b)
        {
            return new Pos3L(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Pos3L operator -(Pos3L a, Pos3L b)
        {
            return new Pos3L(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Pos3L operator *(Pos3L a, long mult)
        {
            return new Pos3L(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Pos3L operator /(Pos3L a, long mult)
        {
            if (mult == 0)
            {
                return new Pos3L();
            }
            return new Pos3L(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public long Manhattan(Pos3L other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        public Pos2L DropZ()
        {
            return new Pos2L(X, Y);
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

            Pos3L objAsPos3L = obj as Pos3L;
            if (objAsPos3L == null)
            {
                return false;
            }

            return Equals(objAsPos3L);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion


    #region Pos3BI
    public class Pos3BI : IEquatable<Pos3BI>
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }
        public BigInteger Z { get; set; }

        private static Pos3BI _zero;
        public static Pos3BI Zero
        {
            get
            {
                if (_zero != null)
                {
                    return _zero;
                }
                _zero = new Pos3BI();
                return _zero;
            }
        }

        public Pos3BI()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Pos3BI(BigInteger x, BigInteger y, BigInteger z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Pos3BI(Pos3BI other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Pos3BI Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            BigInteger[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(BigInteger.Parse).ToArray();
            return new Pos3BI(split[0], split[1], split[2]);
        }

        public static Pos3BI operator +(Pos3BI a, Pos3BI b)
        {
            return new Pos3BI(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Pos3BI operator -(Pos3BI a, Pos3BI b)
        {
            return new Pos3BI(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Pos3BI operator *(Pos3BI a, BigInteger mult)
        {
            return new Pos3BI(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Pos3BI operator /(Pos3BI a, BigInteger mult)
        {
            if (mult == 0)
            {
                return new Pos3BI();
            }
            return new Pos3BI(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public BigInteger Manhattan(Pos3BI other)
        {
            return Util.Number.Abs(X - other.X) + Util.Number.Abs(Y - other.Y) + Util.Number.Abs(Z - other.Z);
        }

        public Pos2BI DropZ()
        {
            return new Pos2BI(X, Y);
        }

        #region Interfaces
        public bool Equals(Pos3BI other)
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

            Pos3BI objAsPos3BI = obj as Pos3BI;
            if (objAsPos3BI == null)
            {
                return false;
            }

            return Equals(objAsPos3BI);
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

            float[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
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

        public Pos2F DropZ()
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

            if (obj is not Vec3F objAsPos3F)
            {
                return false;
            }

            return Equals(objAsPos3F);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        #endregion
    }
    #endregion
}