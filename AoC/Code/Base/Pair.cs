using System;
using Newtonsoft.Json;

namespace AoC.Base
{
    [JsonObject]
    public class Pair<TFirst, TLast> : IEquatable<Pair<TFirst, TLast>>, IComparable<Pair<TFirst, TLast>>
    {
        protected TFirst m_first;
        public TFirst First { get => m_first; set => m_first = value; }
        protected TLast m_last;
        public TLast Last { get => m_last; set => m_last = value; }

        #region Constructors
        public Pair()
        {
            First = default;
            Last = default;
        }

        public Pair(TFirst first, TLast last)
        {
            First = first;
            Last = last;
        }

        public Pair(Pair<TFirst, TLast> other)
        {
            First = other.First;
            Last = other.Last;
        }
        #endregion

        #region Serialization
        public virtual bool ShouldSerializeFirst()
        {
            return true;
        }

        public virtual bool ShouldSerializeLast()
        {
            return true;
        }
        #endregion

        #region Interfaces
        public bool Equals(Pair<TFirst, TLast> other)
        {
            if (other == null)
            {
                return false;
            }

            return m_first.Equals(other.m_first) && m_last.Equals(other.m_last);
        }

        public int CompareTo(Pair<TFirst, TLast> other)
        {
            if (other == null)
            {
                return -1;
            }

            Pair<TFirst, TLast> otherAsPair = other as Pair<TFirst, TLast>;
            if (otherAsPair == null)
            {
                return -1;
            }

            return GetHashCode().CompareTo(otherAsPair.GetHashCode());
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"[First={m_first}, Last={m_last}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Pair<TFirst, TLast> objAsPair = obj as Pair<TFirst, TLast>;
            if (objAsPair == null)
            {
                return false;
            }

            return Equals(objAsPair);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_first, m_last);
        }
        #endregion
    }
    
}