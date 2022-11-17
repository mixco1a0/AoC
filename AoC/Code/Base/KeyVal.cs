using System;

namespace AoC.Base
{
    public class KeyVal<TKey, TVal> : Pair<TKey, TVal>, IEquatable<KeyVal<TKey, TVal>>
        where TKey : IComparable
    {
        public TKey Key { get => First; set => First = value; }
        public TVal Val { get => Last; set => Last = value; }

        public KeyVal() : base() { }

        public KeyVal(TKey key, TVal val) : base(key, val) { }

        public KeyVal(KeyVal<TKey, TVal> other) : base(other) { }

        public bool Equals(KeyVal<TKey, TVal> other) => base.Equals(other);

        public override string ToString()
        {
            return $"[{Key}={Val}]";
        }
        
        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}