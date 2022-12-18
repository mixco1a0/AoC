using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2022
{
    class Day18 : Core.Day
    {
        public Day18() { }

        public override string GetSolutionVersion(Core.Part part)
        {
            switch (part)
            {
                case Core.Part.One:
                    return "v1";
                case Core.Part.Two:
                    return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => true;

        protected override List<Core.TestDatum> GetTestData()
        {
            List<Core.TestDatum> testData = new List<Core.TestDatum>();
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "10",
                RawInput =
@"1,1,1
2,1,1"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.One,
                Output = "64",
                RawInput =
@"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "30",
                RawInput =
@"1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3"
            });
            testData.Add(new Core.TestDatum
            {
                TestPart = Core.Part.Two,
                Output = "58",
                RawInput =
@"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5"
            });
            return testData;
        }

        private record Position3(int X, int Y, int Z)
        {
            public static Position3 Parse(string input)
            {
                int[] split = input.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                return new Position3(split[0], split[1], split[2]);
            }

            public static Position3 operator +(Position3 a, Position3 b)
            {
                return new Position3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }
        }

        private readonly List<Position3> Movement = new List<Position3>()
        {
            new Position3(1, 0, 0),
            new Position3(0, 1, 0),
            new Position3(0, 0, 1),
            new Position3(-1, 0, 0),
            new Position3(0, -1, 0),
            new Position3(0, 0, -1),
        };

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables, bool removeAirPockets)
        {
            List<Position3> pos = inputs.Select(Position3.Parse).ToList();

            List<Position3> unique = new List<Position3>();
            foreach (Position3 p in pos)
            {
                foreach (Position3 m in Movement)
                {
                    unique.Add(p + m);
                }
            }

            unique.RemoveAll(u => pos.Contains(u));

            if (!removeAirPockets)
            {
                return unique.Count.ToString();
            }

            int pocketCount = 0;
            foreach (Position3 u in unique)
            {
                List<Position3> xs = pos.Where(p => p.Y == u.Y && p.Z == u.Z).ToList();
                List<Position3> ys = pos.Where(p => p.X == u.X && p.Z == u.Z).ToList();
                List<Position3> zs = pos.Where(p => p.X == u.X && p.Y == u.Y).ToList();
                if (!xs.Any() || !ys.Any() || !zs.Any())
                {
                    continue;
                }

                if (u.X > xs.Min(s => s.X) && u.X < xs.Max(s => s.X) &&
                    u.Y > ys.Min(s => s.Y) && u.Y < ys.Max(s => s.Y) &&
                    u.Z > zs.Min(s => s.Z) && u.Z < zs.Max(s => s.Z))
                {
                    ++pocketCount;
                }
            }
            return (unique.Count - pocketCount).ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, false);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables, true);
    }
}