namespace AoC.Base
{
    public class Segment
    {
        public Pos2 A { get; set; }
        public Pos2 B { get; set; }

        public Segment(Pos2 a, Pos2 b)
        {
            A = a;
            B = b;
        }

        public Segment Flip()
        {
            return new Segment(B, A);
        }

        public Pos2 GetIntersection(Segment other)
        {
            // if this is X oriented, other is Y oriented
            if (A.X == B.X && other.A.Y == other.B.Y)
            {
                Range yRange = new Range(A.Y, B.Y);
                Range xRange = new Range(other.A.X, other.B.X);
                if ((yRange.HasInc(other.A.Y) || yRange.Flip().HasInc(other.A.Y)) &&
                    (xRange.HasInc(A.X) || xRange.Flip().HasInc(A.X)))
                {
                    return new Pos2(A.X, other.A.Y);
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
                    return new Pos2(other.A.X, A.Y);
                }
            }
            // no other way to get intersection currently
            return null;
        }
    }
}