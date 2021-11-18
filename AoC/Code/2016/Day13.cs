using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day13 : Day
    {
        public Day13() { }
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
                Variables = new Dictionary<string, string>() { { "targetX", "7" }, { "targetY", "4" } },
                Output = "11",
                RawInput =
@"10"
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

        private ulong GetId(int x, int y)
        {
            ulong id = (uint)y;
            id = id << 32;
            return id | (ulong)(uint)x;
        }

        private bool IsOpen(uint x, uint y, uint magicNumber)
        {
            uint isWall = (x * x) + (3 * x) + (2 * x * y) + y + (y * y) + magicNumber;
            int bits = 0;
            for (uint i = 0, bit = 1; i < 32; ++i, bit = bit << 1)
            {
                bits += ((isWall & bit) == 0 ? 0 : 1);
            }
            return bits % 2 == 0;
        }

        private record PointWalk(Point Point, uint Distance){}

        private uint WalkPath(Queue<PointWalk> points, Point target, uint magicNumber)
        {
            HashSet<ulong> visited = new HashSet<ulong>();
            while (points.Count > 0)
            {
                PointWalk pointWalk = points.Dequeue();
                Point point = pointWalk.Point;

                // check if this is the target
                if (point.X == target.X && point.Y == target.Y)
                {
                    return pointWalk.Distance;
                }

                if (!IsOpen((uint)point.X, (uint)point.Y, magicNumber))
                {
                    continue;
                }

                if (visited.Contains(GetId(point.X, point.Y)))
                {
                    continue;
                }
                visited.Add(GetId(point.X, point.Y));

                // add new points
                points.Enqueue(new PointWalk(new Point(point.X + 1, point.Y), pointWalk.Distance + 1));
                points.Enqueue(new PointWalk(new Point(point.X, point.Y + 1), pointWalk.Distance + 1));
                if (point.X > 0)
                {
                    points.Enqueue(new PointWalk(new Point(point.X - 1, point.Y), pointWalk.Distance + 1));
                }
                if (point.Y > 0)
                {
                    points.Enqueue(new PointWalk(new Point(point.X, point.Y - 1), pointWalk.Distance + 1));
                }

            }
            return uint.MaxValue;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            int targetX = 31, targetY = 39;
            if (variables != null && variables.ContainsKey(nameof(targetX)))
            {
                targetX = int.Parse(variables[nameof(targetX)]);
            }
            if (variables != null && variables.ContainsKey(nameof(targetY)))
            {
                targetY = int.Parse(variables[nameof(targetY)]);
            }

            uint magicNumber = uint.Parse(inputs.First());
            Queue<PointWalk> points = new Queue<PointWalk>();
            points.Enqueue(new PointWalk(new Point(1, 1), 0));
            return WalkPath(points, new Point(targetX, targetY), magicNumber).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}