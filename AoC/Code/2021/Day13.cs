using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC._2021
{
    class Day13 : Day
    {
        public Day13() { }

        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
                case Part.Two:
                    return "v1";
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
                Output = "17",
                RawInput =
@"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5"
            });
            testData.Add(new TestDatum
            {
                TestPart = Part.Two,
                Output = "",
                RawInput =
@""
            });
            return testData;
        }

        private class Point : Core.Point<int>
        {
            public Point() : base() { }
            public Point(int x, int y) : base(x, y) { }
            public Point(Point other) : base(other) { }

            public static Point Parse(string input)
            {
                if (!input.Contains(','))
                {
                    return null;
                }

                int[] split = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                return new Point(split[0], split[1]);
            }
        }

        private class Instruction : Core.Pair<bool, int>
        {
            public bool XAxis { get => m_first; }
            public int Index { get => m_last; }
            public Instruction() : base() { }
            public Instruction(bool xAxis, int index) : base(xAxis, index) { }
            public Instruction(Instruction other) : base(other) { }

            public static Instruction Parse(string input)
            {
                string[] split = input.Split(" =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
                return new Instruction(split[2][0] == 'x', int.Parse(split[3]));
            }
        }

        private Point[] Fold(Instruction instruction, Point[] points)
        {
            List<Point> folded = new List<Point>();
            foreach (Point point in points)
            {
                if (instruction.XAxis)
                {
                    if (point.X < instruction.Index)
                    {
                        folded.Add(point);
                    }
                    else if (point.X > instruction.Index)
                    {
                        folded.Add(new Point(instruction.Index - (point.X - instruction.Index), point.Y));
                    }
                }
                else
                {
                    if (point.Y < instruction.Index)
                    {
                        folded.Add(point);
                    }
                    else if (point.Y > instruction.Index)
                    {
                        folded.Add(new Point(point.X, instruction.Index - (point.Y - instruction.Index)));
                    }
                }
            }
            return folded.Distinct().ToArray();
        }

        private string[] GetGlyph(Point[] points)
        {
            List<string> glyph = new List<string>();
            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            for (int y = 0; y <= maxY; ++y)
            {
                StringBuilder sb = new StringBuilder();
                for (int x = 0; x <= maxX; ++x)
                {
                    if (points.Any(p => p.X == x && p.Y == y))
                    {
                        sb.Append(Util.Glyph.On);
                    }
                    else
                    {
                        sb.Append(Util.Glyph.Off);
                    }
                }
                glyph.Add(sb.ToString());
            }
            return glyph.ToArray();
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool oneFold)
        {
            Point[] points = inputs.Select(Point.Parse).Where(p => p != null).ToArray();
            Instruction[] instructions = inputs.Where(i => i.Contains("fold")).Select(Instruction.Parse).ToArray();
            foreach (Instruction instruction in instructions)
            {
                points = Fold(instruction, points);
                if (oneFold)
                {
                    break;
                }
            }
            if (oneFold)
            {
                return points.Count().ToString();
            }
            string[] glyph = GetGlyph(points);
            return Util.GlyphConverter.Process(glyph, Util.GlyphConverter.EType._5x6);
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);
    }
}