using System;

namespace AoC.Base
{
    public class Segment
    {
        public Vec2 A { get; set; }
        public Vec2 B { get; set; }

        public Segment(Vec2 a, Vec2 b)
        {
            A = a;
            B = b;
        }

        public Segment Flip()
        {
            return new Segment(B, A);
        }

        public bool Includes(Vec2 pos)
        {
            int xMin = Math.Min(A.X, B.X);
            int xMax = Math.Max(A.X, B.X);
            int yMin = Math.Min(A.Y, B.Y);
            int yMax = Math.Max(A.Y, B.Y);

            if (pos.X == A.X && A.X == B.X)
            {
                return pos.Y >= yMin && pos.Y <= yMax;
            }
            else if (pos.Y == A.Y && A.Y == B.Y)
            {
                return pos.X >= xMin && pos.X <= xMax;
            }
            return false;
        }

        public Vec2 GetIntersection(Segment other)
        {
            // if this is X oriented, other is Y oriented
            if (A.X == B.X && other.A.Y == other.B.Y)
            {
                Range yRange = new Range(A.Y, B.Y);
                Range xRange = new Range(other.A.X, other.B.X);
                if ((yRange.HasInc(other.A.Y) || yRange.Flip().HasInc(other.A.Y)) &&
                    (xRange.HasInc(A.X) || xRange.Flip().HasInc(A.X)))
                {
                    return new Vec2(A.X, other.A.Y);
                }
            }
            // if this is Y oriented, other is X oriented
            else if (A.Y == B.Y && other.A.X == other.B.X)
            {
                Range xRange = new Range(A.X, B.X);
                Range yRange = new Range(other.A.Y, other.B.Y);
                if ((xRange.HasInc(other.A.X) || xRange.Flip().HasInc(other.A.X)) &&
                    (yRange.HasInc(A.Y) || yRange.Flip().HasInc(A.Y)))
                {
                    return new Vec2(other.A.X, A.Y);
                }
            }
            // no other way to get intersection currently
            return null;
        }

        public override string ToString()
        {
            return $"{A} -> {B}";
        }
    }

    public class SegmentL
    {
        public Vec2L A { get; set; }
        public Vec2L B { get; set; }

        public SegmentL(Vec2L a, Vec2L b)
        {
            A = a;
            B = b;
        }

        public SegmentL Flip()
        {
            return new SegmentL(B, A);
        }

        public bool Includes(Vec2L pos)
        {
            long xMin = Math.Min(A.X, B.X);
            long xMax = Math.Max(A.X, B.X);
            long yMin = Math.Min(A.Y, B.Y);
            long yMax = Math.Max(A.Y, B.Y);

            if (pos.X == A.X && A.X == B.X)
            {
                return pos.Y >= yMin && pos.Y <= yMax;
            }
            else if (pos.Y == A.Y && A.Y == B.Y)
            {
                return pos.X >= xMin && pos.X <= xMax;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{A} -> {B}";
        }
    }
}