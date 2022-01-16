using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
    public class MinMax
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public MinMax()
        {
            Min = 0;
            Max = 0;
        }
        public MinMax(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public bool GTE_LTE(int val) { return Min <= val && val <= Max; }
        public bool GT_LTE(int val) { return Min < val && val <= Max; }
        public bool GTE_LT(int val) { return Min <= val && val < Max; }
        public bool GT_LT(int val) { return Min < val && val < Max; }
        public MinMax Flip()
        {
            return new MinMax(Max, Min);
        }
    }


    public class Coords : IEquatable<Coords>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords()
        {
            X = 0;
            Y = 0;
        }

        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coords operator +(Coords a, Coords b)
        {
            return new Coords(a.X + b.X, a.Y + b.Y);
        }

        public Coords(Coords other)
        {
            X = other.X;
            Y = other.Y;
        }

        public bool Equals(Coords other)
        {
            if (other == null)
            {
                return false;
            }

            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Coords objAsCoords = obj as Coords;
            if (objAsCoords == null)
            {
                return false;
            }

            return Equals(objAsCoords);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }

    public class Segment
    {
        public Base.Point A { get; set; }
        public Base.Point B { get; set; }

        public Segment(Base.Point a, Base.Point b)
        {
            A = a;
            B = b;
        }

        public Segment Flip()
        {
            return new Segment(B, A);
        }

        public Base.Point GetIntersection(Segment other)
        {
            // if this is X oriented, other is Y oriented
            if (A.X == B.X && other.A.Y == other.B.Y)
            {
                MinMax yRange = new MinMax(A.Y, B.Y);
                MinMax xRange = new MinMax(other.A.X, other.B.X);
                if ((yRange.GTE_LTE(other.A.Y) || yRange.Flip().GTE_LTE(other.A.Y)) &&
                    (xRange.GTE_LTE(A.X) || xRange.Flip().GTE_LTE(A.X)))
                {
                    return new Base.Point(A.X, other.A.Y);
                }
            }
            // if this is Y oriented, other is X oriented
            else if (A.Y == B.Y && other.A.X == other.B.X)
            {
                MinMax xRange = new MinMax(A.X, B.X);
                MinMax yRange = new MinMax(other.A.Y, other.B.Y);
                if ((xRange.GTE_LTE(other.A.X) || xRange.Flip().GTE_LTE(other.A.X)) &&
                    (yRange.GTE_LTE(A.Y) || yRange.Flip().GTE_LTE(A.Y)))
                {
                    return new Base.Point(other.A.X, A.Y);
                }
            }
            return null;
        }
    }
}