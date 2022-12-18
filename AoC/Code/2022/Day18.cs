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
                // case Core.Part.One:
                //     return "v1";
                // case Core.Part.Two:
                //     return "v1";
                default:
                    return base.GetSolutionVersion(part);
            }
        }

        public override bool SkipTestData => false;

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
                Output = "",
                RawInput =
@""
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

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
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
            return unique.Where(u => !pos.Contains(u)).Count().ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}