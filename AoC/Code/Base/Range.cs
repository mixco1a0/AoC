using System;

namespace AoC.Base
{
    public class Range<T> : CPair<T, T>
        where T : IComparable
    {
        public T Min { get => m_first; set => m_first = value; }
        public T Max { get => m_last; set => m_last = value; }

        public Range() : base() { }
        public Range(T min, T max) : base(min, max) { }
        public Range(Range<T> other) : base(other) { }

        /// <summary>
        /// [Min, Max]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasInc(T value) => Has(value, true, true);

        /// <summary>
        /// [Min, Max]
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool HasInc(Range<T> range) => HasInc(range.Min) && HasInc(range.Max);

        /// <summary>
        /// [Min, Max]
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool HasIncOr(Range<T> range) => HasInc(range.Min) || HasInc(range.Max);

        /// <summary>
        /// [Min, Max)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasIncExc(T value) => Has(value, true, false);

        /// <summary>
        /// (Min, Max]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasExcInc(T value) => Has(value, false, true);

        /// <summary>
        /// (Min, Max)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasExc(T value) => Has(value, false, false);

        protected bool Has(T value, bool inclusiveMin, bool inclusiveMax)
        {
            int compareMin = Min.CompareTo(value);
            int compareMax = Max.CompareTo(value);
            if (inclusiveMin && inclusiveMax)
            {
                return compareMin <= 0 && compareMax >= 0;
            }
            else if (inclusiveMin && !inclusiveMax)
            {
                return compareMin <= 0 && compareMax > 0;
            }
            else if (!inclusiveMin && inclusiveMax)
            {
                return compareMin < 0 && compareMax >= 0;
            }
            return compareMin < 0 && compareMax > 0;
        }

        public Range<T> Flip()
        {
            return new Range<T>(Max, Min);
        }
    }

    public class Range : Range<int>
    {
        public Range() : base() { }
        public Range(int min, int max) : base(min, max) { }
        public Range(Range other) : base(other) { }

        public override bool HasInc(int value) => Min <= value && value <= Max;
        public override bool HasIncExc(int value) => Min <= value && value < Max;
        public override bool HasExcInc(int value) => Min < value && value <= Max;
        public override bool HasExc(int value) => Min < value && value < Max;
    }

    public class LongRange : Range<long>
    {
        public LongRange() : base() { }
        public LongRange(long min, long max) : base(min, max) { }
        public LongRange(LongRange other) : base(other) { }

        public override bool HasInc(long value) => Min <= value && value <= Max;
        public override bool HasIncExc(long value) => Min <= value && value < Max;
        public override bool HasExcInc(long value) => Min < value && value <= Max;
        public override bool HasExc(long value) => Min < value && value < Max;
    }
}