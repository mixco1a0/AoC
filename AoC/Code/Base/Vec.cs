using System;
using System.Linq;
using System.Numerics;

namespace AoC.Base
{
    #region Vec2
    public class Vec2 : IEquatable<Vec2>, IComparable<Vec2>, IComparable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly Vec2 Zero = new();

        public Vec2()
        {
            X = default;
            Y = default;
        }

        public Vec2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vec2(Vec2 other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Vec2 Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            int[] split = Util.String.Split(input, ',').Select(int.Parse).ToArray();
            return new(split[0], split[1]);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2 operator *(Vec2 a, int mult)
        {
            return new(a.X * mult, a.Y * mult);
        }

        public static Vec2 operator /(Vec2 a, int mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult);
        }

        public static Vec2 operator %(Vec2 a, int mod)
        {
            if (mod == 0)
            {
                return new();
            }
            
            Vec2 modded = new(a);
            modded.Mod(mod, mod);
            return modded;
        }

        public void Mod(int xMod, int yMod)
        {
            X %= xMod;
            if (X < 0)
            {
                X += xMod;
            }
            
            Y %= yMod;
            if (Y < 0)
            {
                Y += yMod;
            }
        }

        public int Manhattan(Vec2 other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Vec2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Vec2 other)
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
            if (other is not Vec2 otherAsVec)
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

            if (obj is not Vec2 objAsVec)
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


    #region Pos2L
    public class Vec2L : IEquatable<Vec2L>, IComparable<Vec2L>, IComparable
    {
        public long X { get; set; }
        public long Y { get; set; }
        
        public static readonly Vec2L Zero = new();

        public Vec2L()
        {
            X = default;
            Y = default;
        }

        public Vec2L(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Vec2L(Vec2L other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Vec2L Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            long[] split = Util.String.Split(input, ',').Select(long.Parse).ToArray();
            return new(split[0], split[1]);
        }

        public static Vec2L operator +(Vec2L a, Vec2L b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2L operator -(Vec2L a, Vec2L b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2L operator *(Vec2L a, long mult)
        {
            return new(a.X * mult, a.Y * mult);
        }

        public static Vec2L operator /(Vec2L a, long mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult);
        }

        public long Manhattan(Vec2L other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Vec2L other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Vec2L other)
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
            if (other is not Vec2L otherAsVec)
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

            if (obj is not Vec2L objAsVec)
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


    #region Vec2BI
    public class Vec2BI : IEquatable<Vec2BI>, IComparable<Vec2BI>, IComparable
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        public static readonly Vec2BI Zero = new();

        public Vec2BI()
        {
            X = default;
            Y = default;
        }

        public Vec2BI(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public Vec2BI(Vec2BI other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static Vec2BI Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            BigInteger[] split = Util.String.Split(input, ',').Select(BigInteger.Parse).ToArray();
            return new(split[0], split[1]);
        }

        public static Vec2BI operator +(Vec2BI a, Vec2BI b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2BI operator -(Vec2BI a, Vec2BI b)
        {
            return new(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2BI operator *(Vec2BI a, BigInteger mult)
        {
            return new(a.X * mult, a.Y * mult);
        }

        public static Vec2BI operator /(Vec2BI a, BigInteger mult)
        {
            if (mult == 0)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult);
        }

        public BigInteger Manhattan(Vec2BI other)
        {
            return BigInteger.Abs(X - other.X) + BigInteger.Abs(Y - other.Y);
        }

        #region Interfaces
        public bool Equals(Vec2BI other)
        {
            return X == other.X && Y == other.Y;
        }

        public virtual int CompareTo(Vec2BI other)
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
            if (other is not Vec2BI otherAsVec)
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

            if (obj is not Vec2BI objAsVec)
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

        public Vec2 DropZ()
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

        public Vec2L DropZ()
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
            return BigInteger.Abs(X - other.X) + BigInteger.Abs(Y - other.Y) + BigInteger.Abs(Z - other.Z);
        }

        public Vec2BI DropZ()
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

    #region Vec3D

    public class Vec3D : IEquatable<Vec3D>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static readonly Vec3F Zero = new();

        public Vec3D()
        {
            X = default;
            Y = default;
            Z = default;
        }

        public Vec3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3D(Vec3D other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vec3D Parse(string input)
        {
            if (!input.Contains(','))
            {
                return null;
            }

            double[] split = Util.String.Split(input, ',').Select(double.Parse).ToArray();
            return new(split[0], split[1], split[2]);
        }

        public static Vec3D operator +(Vec3D a, Vec3D b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3D operator -(Vec3D a, Vec3D b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vec3D operator *(Vec3D a, double mult)
        {
            return new(a.X * mult, a.Y * mult, a.Z * mult);
        }

        public static Vec3D operator /(Vec3D a, double mult)
        {
            if (mult == 0.0f)
            {
                return new();
            }
            return new(a.X / mult, a.Y / mult, a.Z / mult);
        }

        public double Manhattan(Vec3D other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
        }

        // public Vec2F DropZ()
        // {
        //     return new(X, Y);
        // }

        #region Interfaces
        public bool Equals(Vec3D other)
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

            if (obj is not Vec3D objAsVec)
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