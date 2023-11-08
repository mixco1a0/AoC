using System;

namespace AoC.Base
{
    public class Range : Pos2
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
    }

    public class RangeL : Pos2L
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