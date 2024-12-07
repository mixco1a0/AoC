using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.Base
{

    #region Grid2
    public class Grid2
    {
        #region Direction
        public enum Dir { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest, None }

        public static class Map
        {
            /// <summary>
            /// NorthWest (-1, -1) | North     ( 0, -1) | NorthEast ( 1, -1)
            /// West      (-1,  0) | None      ( 0,  0) | East      ( 1,  0)
            /// SouthWest (-1,  1) | South     ( 0,  1) | SouthEast ( 1,  1)
            /// </summary>
            public static readonly Dictionary<Dir, Base.Vec2> Neighbor = new()
            {
                { Dir.North,        new Base.Vec2( 0, -1) },
                { Dir.NorthEast,    new Base.Vec2( 1, -1) },
                { Dir.East,         new Base.Vec2( 1,  0) },
                { Dir.SouthEast,    new Base.Vec2( 1,  1) },
                { Dir.South,        new Base.Vec2( 0,  1) },
                { Dir.SouthWest,    new Base.Vec2(-1,  1) },
                { Dir.West,         new Base.Vec2(-1,  0) },
                { Dir.NorthWest,    new Base.Vec2(-1, -1) },
                { Dir.None,         new Base.Vec2( 0,  0) },
            };

            public static readonly Dictionary<Dir, char> Arrow = new()
            {
                { Dir.North,        '↑' },
                { Dir.NorthEast,    '↗' },
                { Dir.East,         '→' },
                { Dir.SouthEast,    '↘' },
                { Dir.South,        '↓' },
                { Dir.SouthWest,    '↙' },
                { Dir.West,         '←' },
                { Dir.NorthWest,    '↖' },
                { Dir.None,         '.' },
            };

            public static readonly Dictionary<Dir, Dir> Opposite = new()
            {
                { Dir.North,        Dir.South     },
                { Dir.NorthEast,    Dir.SouthWest },
                { Dir.East,         Dir.West      },
                { Dir.SouthEast,    Dir.NorthWest },
                { Dir.South,        Dir.North     },
                { Dir.SouthWest,    Dir.NorthEast },
                { Dir.West,         Dir.East      },
                { Dir.NorthWest,    Dir.SouthEast },
                { Dir.None,         Dir.None      },
            };

            public static readonly Dictionary<Dir, Dir> RotateCW = new()
            {
                { Dir.North,        Dir.East      },
                { Dir.NorthEast,    Dir.SouthEast },
                { Dir.East,         Dir.South     },
                { Dir.SouthEast,    Dir.SouthWest },
                { Dir.South,        Dir.West      },
                { Dir.SouthWest,    Dir.NorthWest },
                { Dir.West,         Dir.North     },
                { Dir.NorthWest,    Dir.NorthEast },
                { Dir.None,         Dir.None      },
            };

            public static readonly Dictionary<Dir, Dir> RotateCCW = new()
            {
                { Dir.North,        Dir.West      },
                { Dir.NorthEast,    Dir.NorthWest },
                { Dir.East,         Dir.North     },
                { Dir.SouthEast,    Dir.NorthEast },
                { Dir.South,        Dir.East      },
                { Dir.SouthWest,    Dir.SouthEast },
                { Dir.West,         Dir.South     },
                { Dir.NorthWest,    Dir.SouthWest },
                { Dir.None,         Dir.None      },
            };
        };

        public static class Iter
        {
            public static readonly Dir[] All =
            [
                Dir.North,
                Dir.NorthEast,
                Dir.East,
                Dir.SouthEast,
                Dir.South,
                Dir.SouthWest,
                Dir.West,
                Dir.NorthWest
            ];

            public static readonly Dir[] Cardinal =
            [
                Dir.North,
                Dir.East,
                Dir.South,
                Dir.West
            ];

            public static readonly Dir[] Ordinal =
            [
                Dir.NorthEast,
                Dir.SouthEast,
                Dir.SouthWest,
                Dir.NorthWest
            ];
        };
        #endregion

        public char[,] Grid { get; private set; }
        public int MaxCol { get; private set; }
        public int MaxRow { get; private set; }

        public Grid2(List<string> rawGrid)
        {
            MaxCol = rawGrid[0].Length;
            MaxRow = rawGrid.Count;

            Grid = new char[MaxCol, MaxRow];
            for (int _c = 0; _c < MaxCol; ++_c)
            {
                for (int _r = 0; _r < MaxRow; ++_r)
                {
                    Grid[_c, _r] = rawGrid[_r][_c];
                }
            }
        }

        public bool Has(Base.Vec2 vec2)
        {
            return vec2.X >= 0 && vec2.X < MaxCol && vec2.Y >= 0 && vec2.Y < MaxRow;
        }

        public char At(Base.Vec2 vec2)
        {
            return Grid[vec2.X, vec2.Y];
        }
    }
    #endregion
}