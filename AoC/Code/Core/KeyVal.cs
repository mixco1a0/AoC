using System;

namespace AoC.Core
{
    public class KeyVal<TKey, TVal> : Pair<TKey, TVal>, IEquatable<KeyVal<TKey, TVal>>
        where TKey : IComparable
        where TVal : IComparable
    {
        public TKey Key { get => First; set => First = value; }
        public TVal Val { get => Last; set => Last = value; }

        public KeyVal() : base() { }

        public KeyVal(TKey key, TVal val) : base(key, val) { }

        public KeyVal(KeyVal<TKey, TVal> other) : base(other) { }

        public bool Equals(KeyVal<TKey, TVal> other)
        {
            if (other == null)
            {
                return false;
            }

            return base.Equals(other);
        }

        public override string ToString()
        {
            return $"[{Key}={Val}]";
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            KeyVal<TKey, TVal> objAsKeyVal = obj as KeyVal<TKey, TVal>;
            if (objAsKeyVal == null)
            {
                return false;
            }

            return Equals(objAsKeyVal);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}