using System;
using System.Linq;
using System.Numerics;

namespace AoC.Base
{
    public static class Vec
    {
        public enum Intersection2D
        {
            Parallel,
            Overlap,
            SinglePoint
        }
    }

    #region Vec2
    public class Vec2 : IEquatable<Vec2>
    {
        public Pos2 Pos { get; set; }
        public Pos2 Vel { get; set; }

        private Pos2 _next;
        private Pos2 Next
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

        public Vec2()
        {
            Pos = new Pos2();
            Vel = new Pos2();
        }

        public static Vec2 FromPos(Pos2 pos, Pos2 next)
        {
            return new Vec2() { Pos = pos, Vel = next - pos };
        }

        public static Vec2 FromVel(Pos2 pos, Pos2 vel)
        {
            return new Vec2() { Pos = pos, Vel = vel };
        }

        public static Vec2 ParsePos(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromPos(new Pos2(split[0], split[1]), new Pos2(split[3], split[4]));
        }

        public static Vec2 ParseVel(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromVel(new Pos2(split[0], split[1]), new Pos2(split[3], split[4]));
        }

        public Vec.Intersection2D GetIntersection(Vec2 other, out Pos2 intersection)
        {
            intersection = new Pos2();
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
                return Vec.Intersection2D.Overlap;
            }

            long denominator = A * other.B - other.A * B;
            if (denominator == 0)
            {
                return Vec.Intersection2D.Parallel;
            }

            long x = (other.B * C - B * other.C) / denominator;
            long y = -1 * (other.A * C - A * other.C) / denominator;
            intersection = new Pos2((int)x, (int)y);

            return Vec.Intersection2D.SinglePoint;
        }

        public int GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2);
            double root = Math.Sqrt(squares);
            return (int)root;
        }

        #region Interfaces
        public bool Equals(Vec2 other)
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

            Vec2 objAsVec2 = obj as Vec2;
            if (objAsVec2 == null)
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


    #region Vec2L
    public class Vec2L : IEquatable<Vec2L>
    {
        public Pos2L Pos { get; set; }
        public Pos2L Vel { get; set; }

        private Pos2L _next;
        private Pos2L Next
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

        public Vec2L()
        {
            Pos = new Pos2L();
            Vel = new Pos2L();
        }

        public static Vec2L FromPos(Pos2L pos, Pos2L next)
        {
            return new Vec2L() { Pos = pos, Vel = next - pos };
        }

        public static Vec2L FromVel(Pos2L pos, Pos2L vel)
        {
            return new Vec2L() { Pos = pos, Vel = vel };
        }

        public static Vec2L ParsePos(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new Pos2L(split[0], split[1]), new Pos2L(split[2], split[3]));
        }

        public static Vec2L ParseVel(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new Pos2L(split[0], split[1]), new Pos2L(split[2], split[3]));
        }

        public Vec.Intersection2D GetIntersection(Vec2L other, out Pos2L intersection)
        {
            intersection = new Pos2L();
            if (A == other.A && B == other.B)
            {
                return Vec.Intersection2D.Overlap;
            }

            BigInteger denominator = A * other.B - other.A * B;
            if (denominator == 0)
            {
                return Vec.Intersection2D.Parallel;
            }

            BigInteger x = (other.B * C - B * other.C) / denominator;
            BigInteger y = -1 * (other.A * C - A * other.C) / denominator;
            intersection = new Pos2L((long)x, (long)y);

            return Vec.Intersection2D.SinglePoint;
        }

        public long GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2);
            double root = Math.Sqrt(squares);
            return (long)root;
        }

        #region Interfaces
        public bool Equals(Vec2L other)
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

            Vec2L objAsVec2L = obj as Vec2L;
            if (objAsVec2L == null)
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


    #region Vec3
    public class Vec3 : IEquatable<Vec3>
    {
        public Pos3 Pos { get; set; }
        public Pos3 Vel { get; set; }

        private Pos3 _next;
        private Pos3 Next
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

        public Vec3()
        {
            Pos = new Pos3();
            Vel = new Pos3();
        }

        public static Vec3 FromPos(Pos3 pos, Pos3 next)
        {
            return new Vec3() { Pos = pos, Vel = next - pos };
        }

        public static Vec3 FromVel(Pos3 pos, Pos3 vel)
        {
            return new Vec3() { Pos = pos, Vel = vel };
        }

        public static Vec3 ParsePos(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromPos(new Pos3(split[0], split[1], split[2]), new Pos3(split[3], split[4], split[5]));
        }

        public static Vec3 ParseVel(string input)
        {
            int[] split = Util.Number.Split(input, ", @").ToArray();
            return FromVel(new Pos3(split[0], split[1], split[2]), new Pos3(split[3], split[4], split[5]));
        }

        public int GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2) + Math.Pow(Pos.Z - Next.Z, 2);
            double root = Math.Sqrt(squares);
            return (int)root;
        }

        public Vec2 DropZ()
        {
            return new Vec2() { Pos = Pos.DropZ(), Vel = Vel.DropZ() };
        }

        #region Interfaces
        public bool Equals(Vec3 other)
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

            Vec3 objAsVec3 = obj as Vec3;
            if (objAsVec3 == null)
            {
                return false;
            }

            return Equals(objAsVec3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion


    #region Vec3L
    public class Vec3L : IEquatable<Vec3L>
    {
        public Pos3L Pos { get; set; }
        public Pos3L Vel { get; set; }

        private Pos3L _next;
        private Pos3L Next
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

        public Vec3L()
        {
            Pos = new Pos3L();
            Vel = new Pos3L();
        }

        public static Vec3L FromPos(Pos3L pos, Pos3L next)
        {
            return new Vec3L() { Pos = pos, Vel = next - pos };
        }

        public static Vec3L FromVel(Pos3L pos, Pos3L vel)
        {
            return new Vec3L() { Pos = pos, Vel = vel };
        }

        public static Vec3L ParsePos(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromPos(new Pos3L(split[0], split[1], split[2]), new Pos3L(split[3], split[4], split[5]));
        }

        public static Vec3L ParseVel(string input)
        {
            long[] split = Util.Number.SplitL(input, ", @").ToArray();
            return FromVel(new Pos3L(split[0], split[1], split[2]), new Pos3L(split[3], split[4], split[5]));
        }

        public long GetLength()
        {
            double squares = Math.Pow(Pos.X - Next.X, 2) + Math.Pow(Pos.Y - Next.Y, 2) + Math.Pow(Pos.Z - Next.Z, 2);
            double root = Math.Sqrt(squares);
            return (long)root;
        }

        public Vec2L DropZ()
        {
            return new Vec2L() { Pos = Pos.DropZ(), Vel = Vel.DropZ() };
        }

        #region Interfaces
        public bool Equals(Vec3L other)
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

            Vec3L objAsVec3 = obj as Vec3L;
            if (objAsVec3 == null)
            {
                return false;
            }

            return Equals(objAsVec3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Vel);
        }
        #endregion
    }
    #endregion
}