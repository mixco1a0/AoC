using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day05 : Day
    {
        public Day05() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v2";
                case Part.Two:
                    return "v2";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        protected override List<TestDatum> GetTestData()
        {
            List<TestDatum> testData = new List<TestDatum>();
            testData.Add(new TestDatum
            {
                TestPart = Part.One,
                Output = "5",
                RawInput =
@"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "12",
                RawInput =
@"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2"
            });
            return testData;
        }

        private class Segment
        {
            public Coords A { get; set; }
            public Coords B { get; set; }

            public static Segment Parse(string input)
            {
                int[] vals = input.Split(", ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                Coords a = new Coords(vals[0], vals[1]);
                Coords b = new Coords(vals[2], vals[3]);
                return new Segment() { A = a, B = b };
            }
        }

        private enum Direction
        {
            NegativeX,
            NegativeY,
            DiagonalUp,
            DiagonalDown,
        }

        private Dictionary<Direction, Coords> Movement = new Dictionary<Direction, Coords>()
        {
            { Direction.NegativeX, new Coords(-1, 0) },
            { Direction.NegativeY, new Coords(0, -1) },
            { Direction.DiagonalUp, new Coords(-1, -1) },
            { Direction.DiagonalDown, new Coords(-1, 1) },
        };

        private void PrintGrid(Dictionary<Coords, int> grid)
        {
            HashSet<Coords> coords = grid.Keys.ToHashSet();
            int maxX = coords.Select(c => c.X).Max();
            int maxY = coords.Select(c => c.Y).Max();
            for (int y = 0; y <= maxY; ++y)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0,3} | ", y);
                for (int x = 0; x <= maxX; ++x)
                {
                    Coords cur = new Coords(x, y);
                    if (grid.ContainsKey(cur))
                    {
                        sb.Append($"{grid[cur],1}");
                    }
                    else
                    {
                        sb.Append($".");
                    }
                }
                DebugWriteLine(sb.ToString());
                sb.Clear();
            }
            DebugWriteLine(string.Empty);
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkDiagonals)
        {
            Dictionary<Coords, int> overlaps = new Dictionary<Coords, int>();
            Action<int, Direction, Coords> CheckCoords = (count, dir, start) =>
            {
                for (int i = 0; i <= count; ++i)
                {
                    if (!overlaps.ContainsKey(start))
                    {
                        overlaps[start] = 0;
                    }
                    ++overlaps[start];
                    start += Movement[dir];
                }
            };

            Segment[] segments = inputs.Select(Segment.Parse).ToArray();
            foreach (Segment segment in segments)
            {
                if (segment.A.X == segment.B.X)
                {
                    CheckCoords(Math.Abs(segment.A.Y - segment.B.Y), Direction.NegativeY, segment.A.Y > segment.B.Y ? segment.A : segment.B);
                }
                else if (segment.A.Y == segment.B.Y)
                {
                    CheckCoords(Math.Abs(segment.A.X - segment.B.X), Direction.NegativeX, segment.A.X > segment.B.X ? segment.A : segment.B);
                }
                else if (checkDiagonals)
                {
                    Coords start = segment.A.X > segment.B.X ? segment.A : segment.B;
                    Coords end = start.Equals(segment.A) ? segment.B : segment.A;
                    if (end.Y < start.Y)
                    {
                        CheckCoords(Math.Abs(segment.A.X - segment.B.X), Direction.DiagonalUp, start);
                    }
                    else
                    {
                        CheckCoords(Math.Abs(segment.A.X - segment.B.X), Direction.DiagonalDown, start);
                    }
                }
            }
            return overlaps.Where(p => p.Value >= 2).Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}