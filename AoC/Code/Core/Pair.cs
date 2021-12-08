using System;

namespace AoC.Core
{
    public class Pair<TFirst, TLast> : IEquatable<Pair<TFirst, TLast>>, IComparable
        where TFirst : IComparable
        where TLast : IComparable
    {
        protected bool m_sortByFirst;
        protected TFirst m_first;
        public TFirst First { get { return m_first; } set { m_first = value; } }
        protected TLast m_last;
        public TLast Last { get { return m_last; } set { m_last = value; } }

        #region Constructors
        public Pair()
        {
            m_sortByFirst = true;
            First = default;
            Last = default;
        }

        public Pair(TFirst one, TLast two)
        {
            m_sortByFirst = true;
            First = one;
            Last = two;
        }

        public Pair(Pair<TFirst, TLast> other)
        {
            m_sortByFirst = true;
            First = other.First;
            Last = other.Last;
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

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Pair<TFirst, TLast> objAsPair = obj as Pair<TFirst, TLast>;
            if (objAsPair == null)
            {
                return 1;
            }

            int firstSort = m_first.CompareTo(objAsPair.m_first);
            int lastSort = m_last.CompareTo(objAsPair.m_last);
            if (m_sortByFirst)
            {
                if (firstSort == 0)
                {
                    return lastSort;
                }
                return firstSort;
            }

            if (lastSort == 0)
            {
                return firstSort;
            }
            return lastSort;
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
            return m_first.GetHashCode() ^ m_last.GetHashCode();
        }
        #endregion
    }
}