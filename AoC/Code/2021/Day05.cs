using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2021
{
    class Day05 : Day
    {
        public Day05() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                // case Part.One:
                //     return "v1";
                // case Part.Two:
                //     return "v1";
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool checkDiagonals)
        {
            Segment[] segments = inputs.Select(Segment.Parse).ToArray();
            Dictionary<Coords, int> overlaps = new Dictionary<Coords, int>();
            Coords negativeY = new Coords(0, -1);
            Coords negativeX = new Coords(-1, 0);
            Coords negativeDL = new Coords(-1, -1);
            Coords negativeDR = new Coords(-1, 1);
            foreach (Segment segment in segments)
            {
                if (segment.A.X == segment.B.X)
                {
                    Coords start = segment.A.Y > segment.B.Y ? segment.A : segment.B;
                    Coords end = segment.A.Y > segment.B.Y ? segment.B : segment.A;
                    Coords cur = start;
                    for (int y = start.Y; y >= end.Y; --y)
                    {
                        if (!overlaps.ContainsKey(cur))
                        {
                            overlaps[cur] = 0;
                        }
                        ++overlaps[cur];
                        cur += negativeY;
                    }
                }
                else if (segment.A.Y == segment.B.Y)
                {
                    Coords start = segment.A.X > segment.B.X ? segment.A : segment.B;
                    Coords end = segment.A.X > segment.B.X ? segment.B : segment.A;
                    Coords cur = start;
                    for (int x = start.X; x >= end.X; --x)
                    {
                        if (!overlaps.ContainsKey(cur))
                        {
                            overlaps[cur] = 0;
                        }
                        ++overlaps[cur];
                        cur += negativeX;
                    }
                }
                else if (checkDiagonals)
                {
                    Coords start = segment.A.X > segment.B.X ? segment.A : segment.B;
                    Coords end = segment.A.X > segment.B.X ? segment.B : segment.A;
                    Coords cur = start;
                    for (int x = start.X; x >= end.X; --x)
                    {
                        if (!overlaps.ContainsKey(cur))
                        {
                            overlaps[cur] = 0;
                        }
                        ++overlaps[cur];
                        if (end.Y < start.Y)
                        {
                            cur += negativeDL;
                        }
                        else
                        {
                            cur += negativeDR;
                        }
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