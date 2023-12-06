using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC._2023
{
    class Day06 : Core.Day
    {
        public Day06() { }

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
                Output = "288",
                RawInput =
@"Time:      7  15   30
Distance:  9  40  200"
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

        public long GetSolutionCount(long time, long distance)
        {
            long solutions = 0;
            for (long i = 1; i < time; ++i)
            {
                long remainingTime = time - i;
                if (remainingTime * i > distance)
                {
                    ++solutions;
                }
            }
            return solutions;
        }

        private string SharedSolution(List<string> inputs, Dictionary<string, string> variables)
        {
            List<long> times = inputs[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToList();
            List<long> distances = inputs[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToList();
            long answer = 1;
            for (int i = 0; i < times.Count; ++i)
            {
                answer *= GetSolutionCount(times[i], distances[i]);
            }
            return answer.ToString();
        }

        protected override string RunPart1Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);

        protected override string RunPart2Solution(List<string> inputs, Dictionary<string, string> variables)
            => SharedSolution(inputs, variables);
    }
}