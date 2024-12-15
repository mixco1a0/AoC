using System;
using System.Linq;
using System.Numerics;

namespace AoC.Base
{
    public static class Ray
    {
        public enum Intersection2D
        {
            Parallel,
            Overlap,
            SinglePoint
        }
    }

    #region Ray2
    public class Ray2 : IEquatable<Ray2>
    {
        public Vec2 Pos { get; set; }
        public Vec2 Vel { get; set; }
        private Vec2 Next => Tick(1);

        // Ax + By = C
        // A = Vel.Y (Next.Y - Pos.Y)
        // B = Vel.X * -1 (Pos.X - Next.Y)
        private long A { get => Vel.Y; }
        private long B { get => Vel.X * -1; }
        // C = A * Pos.X + B * Vel.Y
        private bool _cSet = false;
        private long _c = 0;
        private long C
        {
            get
            {
                if (_cSet)
                {
                    return _c;
                }

                _c = A * Pos.X + B * Pos.Y;
                _cSet = true;
                return _c;
            }
        }

        public Ray2()
        {
            Pos = new();
            Vel = new();
        }

        public Vec2 Tick(int ticks)
        {
            return Pos + Vel * ticks; 
        }

        public static Ray2 FromPos(Vec2 pos, Vec2 next)
        {
            return new() { Pos = pos, Vel = next - pos };
        }

        public static Ray2 FromVel(Vec2 pos, Vec2 vel)
        {
            return new() { Pos = pos, Vel = vel };
        }

        public static Ray2 ParsePos(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromPos(new(split[0], split[1]), new(split[3], split[4]));
        }

        public static Ray2 ParseVel(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromVel(new(split[0], split[1]), new(split[3], split[4]));
        }

        public Ray.Intersection2D GetIntersection(Ray2 other, out Vec2 intersection)
        {
            intersection = new();
            // A1x + B1y = C1
            // A2x + B2y = C2

            // (A1x + B1y = C1) * B2
            // (A2x + B2y = C1) * B1
            // ----------------------
            // A1B2x + B1B2y = B2C1
            // A2B1x + B1B2y = B1C2
            // ---------------------
            // A1B2x - A2B1x = B2C1 - B1C2
            // ----------------------------
            // x * (A1B2 - A2B1) = B2C1 - B1C2
            // --------------------------------
            // x = (B2C1 - B1C2) / (A1B2 - A2B1)
            // ----------------------------------

            // (A1x + B1y = C1) * A2
            // (A2x + B2y = C2) * A1
            // ----------------------
            // A1A2x + A2B1y = A2C1
            // A1A2x + A1B2y = A1C2
            // ---------------------
            // A2B1y - A1B2y = A2C1 - A1C2
            // ----------------------------
            // y * (A2B1 - A1B2) = A2C1 - A1C2
            // --------------------------------
            // y = (A2C1 - A1C2) / (A2B1 - A1B2)
            // ----------------------------------
            if (A == other.A && B == other.B)
            {
                return Ray.Intersection2D.Overlap;
            }

            long denominator = A * other.B - other.A * B;
            if (denominator == 0)
            {
                return Ray.Intersection2D.Parallel;
            }

            long x = (other.B * C - B * other.C) / denominator;
            long y = -1 * (other.A * C - A * other.C) / denominator;
            intersection = new((int)x, (int)y);

            return Ray.Intersection2D.SinglePoint;
        }

        public int GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2);
            double root = Math.Sqrt(squares);
            return (int)root;
        }

        #region Interfaces
        public bool Equals(Ray2 other)
        {
            return Pos.Equals(other.Pos) && Vel.Equals(other.Vel);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"{Pos} @ {Vel}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not Ray2 objAsVec2)
            {
                return false;
            }

            return Equals(objAsVec2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion


    #region Ray2L
    public class Ray2L : IEquatable<Ray2L>
    {
        public Vec2L Pos { get; set; }
        public Vec2L Vel { get; set; }

        private Vec2L _next;
        private Vec2L Next
        {
            get
            {
                if (_next != null)
                {
                    return _next;
                }
                _next = Pos + Vel;
                return _next;
            }
        }

        private BigInteger A { get => Vel.Y; }
        private BigInteger B { get => Vel.X * -1; }
        private bool _cSet = false;
        private BigInteger _c = 0;
        private BigInteger C
        {
            get
            {
                if (_cSet)
                {
                    return _c;
                }

                _c = A * Pos.X + B * Pos.Y;
                _cSet = true;
                return _c;
            }
        }

        public Ray2L()
        {
            Pos = new();
            Vel = new();
        }

        public static Ray2L FromPos(Vec2L pos, Vec2L next)
        {
            return new() { Pos = pos, Vel = next - pos };
        }

        public static Ray2L FromVel(Vec2L pos, Vec2L vel)
        {
            return new() { Pos = pos, Vel = vel };
        }

        public static Ray2L ParsePos(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new(split[0], split[1]), new(split[2], split[3]));
        }

        public static Ray2L ParseVel(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new(split[0], split[1]), new(split[2], split[3]));
        }

        public Ray.Intersection2D GetIntersection(Ray2L other, out Vec2L intersection)
        {
            intersection = new();
            if (A == other.A && B == other.B)
            {
                return Ray.Intersection2D.Overlap;
            }

            BigInteger denominator = A * other.B - other.A * B;
            if (denominator == 0)
            {
                return Ray.Intersection2D.Parallel;
            }

            BigInteger x = (other.B * C - B * other.C) / denominator;
            BigInteger y = -1 * (other.A * C - A * other.C) / denominator;
            intersection = new((long)x, (long)y);

            return Ray.Intersection2D.SinglePoint;
        }

        public long GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2);
            double root = Math.Sqrt(squares);
            return (long)root;
        }

        #region Interfaces
        public bool Equals(Ray2L other)
        {
            return Pos.Equals(other.Pos) && Vel.Equals(other.Vel);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"{Pos} @ {Vel}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not Ray2L objAsVec2L)
            {
                return false;
            }

            return Equals(objAsVec2L);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion


    #region Ray3
    public class Ray3 : IEquatable<Ray3>
    {
        public Vec3 Pos { get; set; }
        public Vec3 Vel { get; set; }

        private Vec3 _next;
        private Vec3 Next
        {
            get
            {
                if (_next != null)
                {
                    return _next;
                }
                _next = Pos + Vel;
                return _next;
            }
        }

        public Ray3()
        {
            Pos = new();
            Vel = new();
        }

        public static Ray3 FromPos(Vec3 pos, Vec3 next)
        {
            return new() { Pos = pos, Vel = next - pos };
        }

        public static Ray3 FromVel(Vec3 pos, Vec3 vel)
        {
            return new() { Pos = pos, Vel = vel };
        }

        public static Ray3 ParsePos(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromPos(new(split[0], split[1], split[2]), new(split[3], split[4], split[5]));
        }

        public static Ray3 ParseVel(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromVel(new(split[0], split[1], split[2]), new(split[3], split[4], split[5]));
        }

        public int GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2) + Math.Pow(Pos.Z - Next.Z, 2);
            double root = Math.Sqrt(squares);
            return (int)root;
        }

        public Ray2 DropZ()
        {
            return new() { Pos = Pos.DropZ(), Vel = Vel.DropZ() };
        }

        #region Interfaces
        public bool Equals(Ray3 other)
        {
            return Pos.Equals(other.Pos) && Vel.Equals(other.Vel);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"{Pos} @ {Vel}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not Ray3 objAsRay3)
            {
                return false;
            }

            return Equals(objAsRay3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion


    #region Ray3L
    public class Ray3L : IEquatable<Ray3L>
    {
        public Vec3L Pos { get; set; }
        public Vec3L Vel { get; set; }

        private Vec3L _next;
        private Vec3L Next
        {
            get
            {
                if (_next != null)
                {
                    return _next;
                }
                _next = Pos + Vel;
                return _next;
            }
        }

        public Ray3L()
        {
            Pos = new();
            Vel = new();
        }

        public static Ray3L FromPos(Vec3L pos, Vec3L next)
        {
            return new() { Pos = pos, Vel = next - pos };
        }

        public static Ray3L FromVel(Vec3L pos, Vec3L vel)
        {
            return new() { Pos = pos, Vel = vel };
        }

        public static Ray3L ParsePos(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new(split[0], split[1], split[2]), new(split[3], split[4], split[5]));
        }

        public static Ray3L ParseVel(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromVel(new(split[0], split[1], split[2]), new(split[3], split[4], split[5]));
        }

        public long GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2) + Math.Pow(Pos.Z - Next.Z, 2);
            double root = Math.Sqrt(squares);
            return (long)root;
        }

        public Ray2L DropZ()
        {
            return new() { Pos = Pos.DropZ(), Vel = Vel.DropZ() };
        }

        #region Interfaces
        public bool Equals(Ray3L other)
        {
            return Pos.Equals(other.Pos) && Vel.Equals(other.Vel);
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"{Pos} @ {Vel}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is not Ray3L objAsRay3L)
            {
                return false;
            }

            return Equals(objAsRay3L);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion
}