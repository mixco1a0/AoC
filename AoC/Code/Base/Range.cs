using System;

namespace AoC.Base
{
    public class Range : Pos2, IComparable<Range>, IComparable
    {
        public int Min { get => X; set => X = value; }
        public int Max { get => Y; set => Y = value; }
        public Range() : base() { }
        public Range(int min, int max) : base(min, max) { }
        public Range(Range other) : base(other) { }

        public bool HasInc(int value) => Min <= value && value <= Max;
        public bool HasInc(Range other) => HasInc(other.Min) && HasInc(other.Max);
        public bool HasIncOr(Range other) => HasInc(other.Min) || HasInc(other.Max);
        public bool HasIncExc(int value) => Min <= value && value < Max;
        public bool HasExcInc(int value) => Min < value && value <= Max;
        public bool HasExc(int value) => Min < value && value < Max;

        public Range Flip()
        {
            return new Range(Max, Min);
        }

        public int CompareTo(Range other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public new int CompareTo(object other)
        {
            Range otherAsRange = other as Range;
            if (otherAsRange == null)
            {
                return -1;
            }
            return otherAsRange.CompareTo(other);
        }
    }

    public class RangeL : Pos2L, IComparable<RangeL>, IComparable
    {
        public long Min { get => X; set => X = value; }
        public long Max { get => Y; set => Y = value; }
        public RangeL() : base() { }
        public RangeL(long min, long max) : base(min, max) { }
        public RangeL(RangeL other) : base(other) { }

        public bool HasInc(long value) => Min <= value && value <= Max;
        public bool HasInc(RangeL other) => HasInc(other.Min) && HasInc(other.Max);
        public bool HasIncOr(RangeL other) => HasInc(other.Min) || HasInc(other.Max);
        public bool HasIncExc(long value) => Min <= value && value < Max;
        public bool HasExcInc(long value) => Min < value && value <= Max;
        public bool HasExc(long value) => Min < value && value < Max;

        public RangeL Flip()
        {
            return new RangeL(Max, Min);
        }

        public int CompareTo(RangeL other)
        {
            int xCompare = X.CompareTo(other.X);
            if (xCompare != 0)
            {
                return xCompare;
            }
            return Y.CompareTo(other.Y);
        }

        public new int CompareTo(object other)
        {
            RangeL otherAsRange = other as RangeL;
            if (otherAsRange == null)
            {
                return -1;
            }
            return otherAsRange.CompareTo(other);
        }
    }

    public class RangeF : Pos2F
    {
        public float Min { get => X; set => X = value; }
        public float Max { get => Y; set => Y = value; }

        public RangeF() : base() { }
        public RangeF(float min, float max) : base(min, max) { }
        public RangeF(RangeF other) : base(other) { }
    }
}