using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC
{
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
}