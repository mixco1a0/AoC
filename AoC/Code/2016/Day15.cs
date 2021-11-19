using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2016
{
    class Day15 : Day
    {
        public Day15() { }
        public override string GetSolutionVersion(Part part)
        {
            switch (part)
            {
                case Part.One:
                    return "v1";
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
@"Disc #1 has 5 positions; at time=0, it is at position 4.
Disc #2 has 2 positions; at time=0, it is at position 1."
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

        private record Disk(int Id, int InitialPos, int MaxPositions)
        {
            static public Disk Parse(string input)
            {
                string[] split = input.Split(" #=,.".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int[] values = split.Where(s => { int i; return int.TryParse(s, out i); }).Select(int.Parse).ToArray();
                return new Disk(values[0], values[3], values[1]);
            }
            public int GetPosition(int time)
            {
                return (Id + InitialPos + time) % MaxPositions;
            }
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            Disk[] disks = inputs.Select(Disk.Parse).ToArray();
            int time = 0;
            while (true)
            {
                bool fellThrough = true;
                for (int d = 0; fellThrough && d < disks.Count(); ++d)
                {
                    fellThrough = (disks[d].GetPosition(time) == 0);
                }
                if (fellThrough)
                {
                    break;
                }
                ++time;
            }
            return time.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}